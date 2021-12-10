using myStore.entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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


        public delegate void Ready2OpenNotebookEventHandler(short sort_key_idx, int ID);
        public event Ready2OpenNotebookEventHandler Ready2OpenNotebookEvent;

        private const int limit = 50;

        private int cur_page = 0;
        public int CurPage
        {
            get { return cur_page; }
            set
            {
                cur_page = value;
                OnPropertyChanged("CurPage");
            }
        }


        
        private string[] sort_direction_word = { "ASC", "DESC"};
        private bool sort_asc = true;
        
        private string[] sort_keys = { "notebook_id", "price", "rate", "num_of_rates" };
        private short sort_active_idx;
        
        public object sort_states
        {
            get => Enumerable.Range(1, 3).Select(i => (i == sort_active_idx)? ((sort_asc)? '▼' : '▲') : ' ').ToArray();
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
                }

                OnPropertyChanged("sort_states");
            }
        }

        public RelayCommand ChangeSortStateCommand { get; }


        public MainMenu(short sort_key_idx, int page)
        {
            sort_states = sort_key_idx;
            CurPage = page;

            notebooks = new ObservableCollection<NotebookView>();


            ChangeSortStateCommand = new RelayCommand(i => { sort_states = i; ChangePageCommand.Execute(-2); });

            CreateNotebookCommand = new RelayCommand(_ => Ready2OpenNotebookEvent(sort_active_idx, -1));
            GetAllDataCommand = new RelayCommand(
                obj => Ready2OpenNotebookEvent(sort_active_idx, (int)obj),
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
                    int v = (int)obj;

                    int new_page = (v == -2) ? 0 : CurPage + v;

                    IsRemovable = false;
                    bool IsClearable = true;

                    var order_by = sort_keys[sort_active_idx];
                    var diraction = sort_direction_word[sort_asc ? 1 : 0];

                    string sql = $"SELECT * FROM notebook_view ORDER BY {order_by} {diraction} LIMIT {limit} OFFSET {new_page * limit}";

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
                },
                obj =>
                {
                    if (obj is null) return false;

                    return CurPage + (int)obj >= 0 || notebooks.Count < limit || notebooks.Count == 0 && (int)obj > 0;
                }
            );


            InitializeComponent();

            ChangePageCommand.Execute(0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
