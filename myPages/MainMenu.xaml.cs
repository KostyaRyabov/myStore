using myStore.entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace myStore.myPages
{
    public partial class MainMenu : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<NotebookView> notebooks { get; set; }

        private bool IsRemovable = false;

        public RelayCommand RemoveCommand { get; }
        public RelayCommand СancelRemovingCommand { get; }
        public RelayCommand PrepareRemovingCommand { get; }
        public RelayCommand ToggleRemovableCommand { get; }


        public RelayCommand CreateNotebookCommand { get;  }
        public RelayCommand GetAllDataCommand { get; }
        public RelayCommand ChangePageCommand { get; }


        public delegate void Ready2OpenNotebookEventHandler(int ID = -1);
        public event Ready2OpenNotebookEventHandler Ready2OpenNotebookEvent;

        private const int limit = 50;

        private static int cur_page = 0;
        public int CurPage
        {
            get { return cur_page; }
            set
            {
                cur_page = value;
                OnPropertyChanged("CurPage");
            }
        }


        
        private bool sort_asc = true;
        
        private string[] sort_keys = { "notebook_id", "price", "rate", "num_of_rates" };
        private static short sort_active_idx = 0;
        
        public object sort_state
        {
            get => Enumerable.Range(1, 3).Select(i => (i == sort_active_idx)? (sort_asc? '▲' : '▼') : ' ').ToArray();
            set
            {
                var idx = (short)value;

                if (idx != sort_active_idx)
                {
                    sort_active_idx = idx;
                    sort_asc = true;
                }
                else
                {
                    sort_asc ^= true;
                    if (sort_asc) sort_active_idx = 0;
                }

                OnPropertyChanged("sort_state");
            }
        }

        private static string _search_string = "";
        public string search_string
        {
            get => _search_string;
            set
            {
                _search_string = value;
                OnPropertyChanged("search_string");
            }
        }

        public RelayCommand ChangeSortStateCommand { get; }


        public MainMenu()
        {
            notebooks = new ObservableCollection<NotebookView>();


            ChangeSortStateCommand = new RelayCommand(i => { sort_state = i; ChangePageCommand.Execute(null); });

            CreateNotebookCommand = new RelayCommand(_ => Ready2OpenNotebookEvent());
            GetAllDataCommand = new RelayCommand(
                obj => Ready2OpenNotebookEvent((int)obj),
                _ => !IsRemovable
            );
            PrepareRemovingCommand = new RelayCommand(
                _ => IsRemovable = true,
                _ => !IsRemovable
            );
            RemoveCommand = new RelayCommand(
                _ =>
                {
                    var IDs = notebooks.Where(n => n.Removable).Select(n => n.notebook_id);
                    Database.ExecuteOne($"DELETE FROM notebooks WHERE notebook_id IN ({ string.Join(", ", IDs) })");
                    ChangePageCommand.Execute(0);
                },
                _ => IsRemovable && notebooks.Count(n => n.Removable) > 0
            );
            СancelRemovingCommand = new RelayCommand(
                _ => {
                    notebooks.Where(n => n.Removable).ToList().ForEach(n => n.Removable = false);
                    IsRemovable = false;
                },
                _ => IsRemovable
            );
            ToggleRemovableCommand = new RelayCommand(
                obj => (obj as NotebookView).Removable ^= true,
                _ => IsRemovable
            );
            ChangePageCommand = new RelayCommand(
                async obj =>
                {
                    int new_page, vector = 0;

                    if (obj is null) {
                        new_page = 0;
                    }
                    else
                    {
                        vector = (int)obj;
                        new_page = CurPage + vector;
                    }

                    IsRemovable = false;
                    bool IsClearable = true;

                    var order_by = sort_keys[sort_active_idx];
                    var diraction = sort_asc ? "ASC" : "DESC";

                    var sql = $@"WITH all_stats AS (
	                                SELECT comments.notebook_id, comments.comment_id, comments.screen_rate AS rate FROM comments
	                                UNION ALL
	                                SELECT comments.notebook_id, comments.comment_id, comments.power_rate          FROM comments
	                                UNION ALL
	                                SELECT comments.notebook_id, comments.comment_id, comments.work_duration_rate  FROM comments
                                )

                                SELECT
	                                notebooks.notebook_id,
	                                notebooks.price,
	                                notebooks.name,
	                                notebooks.image, 
	                                count(all_stats.comment_id) / 3 AS num_of_rates,
	                                CASE
		                                WHEN count(all_stats.rate) <> 0
		                                THEN avg(all_stats.rate)
		                                ELSE 0::numeric
	                                END AS rate

	                                FROM notebooks LEFT JOIN all_stats USING(notebook_id)
	                                WHERE
		                                notebooks.name  ~* '{search_string}' OR
		                                notebooks.software_os_id    IN (SELECT os_id  		FROM oss 		WHERE name 		    ~* '{search_string}') OR
		                                notebooks.cpu_id 		 	IN (SELECT cpu_id 		FROM cpus 		WHERE processor     ~* '{search_string}'  OR
							 									  				                                  producer 	    ~* '{search_string}') OR
		                                notebooks.gpu_id 			IN (SELECT gpu_id 		FROM gpus 		WHERE video_card    ~* '{search_string}') OR
		                                notebooks.other_producer_id IN (SELECT producer_id 	FROM producers 	WHERE name 		    ~* '{search_string}')
	                                GROUP BY notebooks.notebook_id
                                    ORDER BY {order_by} {diraction}
                                    LIMIT {limit} OFFSET {new_page * limit}";

                    await foreach (NotebookView item in Database.Enumerate<NotebookView>(sql))
                    {
                        if (IsClearable)
                        {
                            IsClearable = false;
                            CurPage = new_page;
                            notebooks.Clear();
                        }

                        notebooks.Add(item);
                    }

                    if (IsClearable && new_page == 0 && search_string.Count() > 0) notebooks.Clear();

                    if (obj == null) ScrollableMenu.ScrollToVerticalOffset(0);
                    else if (vector != 0) ScrollableMenu.ScrollToVerticalOffset(999999);
                },
                obj =>
                {
                    return obj == null || (int)obj >= 0 || CurPage > 0;
                }
            );


            InitializeComponent();

            ChangePageCommand.Execute(0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
