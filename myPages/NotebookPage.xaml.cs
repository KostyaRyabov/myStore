
using FastMember;
using myStore.entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace myStore.myPages
{
    public partial class NotebookPage : UserControl, INotifyPropertyChanged
    {
        public delegate void Ready2OpenNotebookEventHandler(int ID);
        public event Ready2OpenNotebookEventHandler Ready2OpenMainMenuEvent;

        private VerifieldObject<Notebook> vNotebook = new VerifieldObject<Notebook>();
        public Notebook notebook {
            get {
                return vNotebook.obj;
            }
            set {
                vNotebook.obj = value;
                OnPropertyChanged("notebook");
            }
        }

        private DataState current_state = DataState.Empty;

        private enum DataState
        {
            Empty = 0,
            DataLoaded = 1,
            DictionariesLoaded = 2
        }




        public ObservableCollection<OS> os_list { get; set; } = new ObservableCollection<OS>();
        public ObservableCollection<Producer> producer_list { get; set; } = new ObservableCollection<Producer>();

        public ObservableCollection<Cpu> cpu_list { get; set; } = new ObservableCollection<Cpu>();
        public ObservableCollection<Gpu> gpu_list { get; set; } = new ObservableCollection<Gpu>();

        public ObservableCollection<string> List_interfaces_wifi { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> List_interfaces_memory_cards { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<Comment> comments { get; set; } = new ObservableCollection<Comment>();




        private bool _isEditable = false;
        public bool isEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
                OnPropertyChanged("isEditable");
            }
        }

        public RelayCommand ComboboxSelectionChangedCommand { get; }

        public RelayCommand OpenMainMenuCommand { get; }
        public RelayCommand SaveChangesCommand { get; }
        public RelayCommand RemoveNotebookCommand { get; }
        public RelayCommand CancelChangesCommand { get; }

        private delegate void UpdateStateEventHandler(DataState state);
        private event UpdateStateEventHandler UpdateStateEvent;

        private void bindDictionaries()
        {
            //cpu = cpu_list.Where(el => el.cpu_id == notebook.cpu_id).First();
            //gpu = gpu_list.Where(el => el.gpu_id == notebook.gpu_id).First();
        }

        public NotebookPage(int ID = -1, int page = 0)
        {
            ComboboxSelectionChangedCommand = new RelayCommand(
                objs =>
                {
                    var vals = (object[])objs;

                    var old_val = vals[0];
                    var cm = vals[1] as ComboBox;

                    if (cm.SelectedValue == null)
                        cm.SelectedValue = old_val;
                });

            UpdateStateEvent += state =>
            {
                if (state == DataState.Empty) current_state = DataState.Empty;
                else {
                    current_state ^= state;

                    if (current_state.HasFlag(DataState.DataLoaded) && current_state.HasFlag(DataState.DictionariesLoaded))
                        bindDictionaries();
                }
            };

            OpenMainMenuCommand = new RelayCommand(_ => Ready2OpenMainMenuEvent(page));

            CancelChangesCommand = new RelayCommand(_ =>
            {
                UpdateStateEvent(DataState.Empty);
                comments.Clear();

                LoadNotebook(ID);
                LoadComments(ID);
                LoadDictionaries();
            },
            _ => ID > 0
            );

            SaveChangesCommand = new RelayCommand(_ =>
            {
                
            });

            RemoveNotebookCommand = new RelayCommand(_ =>
            {

            });

            InitializeComponent();

            if (ID > 0)
            {
                CancelChangesCommand.Execute(1);
            }
        }

        private async void LoadDictionaries()
        {
            string sql;

            {
                List_interfaces_wifi.Clear();
                sql = $"SELECT DISTINCT unnest(interfaces_wifi) FROM notebooks";
                await foreach (var el in Database.EnumerateList<string>(sql))
                {
                    List_interfaces_wifi.Add(el);
                }
            }

            {
                List_interfaces_memory_cards.Clear();
                sql = $"SELECT DISTINCT unnest(interfaces_memory_cards) FROM notebooks";
                await foreach (var el in Database.EnumerateList<string>(sql))
                {
                    List_interfaces_memory_cards.Add(el);
                }
            }

            {
                os_list.Clear();
                sql = $"SELECT * FROM oss";
                await foreach (var el in Database.Enumerate<OS>(sql))
                {
                    os_list.Add(el);
                }
            }

            // .OrderBy(el => el.name)

            {
                producer_list.Clear();
                sql = $"SELECT * FROM producers";
                await foreach (var el in Database.Enumerate<Producer>(sql))
                {
                    producer_list.Add(el);
                }
            }

            {
                cpu_list.Clear();
                sql = $"SELECT * FROM cpus";
                await foreach (var el in Database.Enumerate<Cpu>(sql))
                {
                    cpu_list.Add(el);
                }
            }

            {
                gpu_list.Clear();
                sql = $"SELECT * FROM gpus";
                await foreach (var el in Database.Enumerate<Gpu>(sql))
                {
                    gpu_list.Add(el);
                }
            }

            UpdateStateEvent(DataState.DictionariesLoaded);
        }
        private async void LoadNotebook(int ID)
        {
            var sql = $"SELECT * FROM notebooks WHERE notebook_id = {ID}";
            notebook = await Database.GetObject<Notebook>(sql);

            UpdateStateEvent(DataState.DataLoaded);
        }
        private async void LoadComments(int ID)
        {
            var sql = $"SELECT * FROM comments WHERE notebook_id = {ID} ORDER BY comment_id";

            await foreach (var c in Database.Enumerate<Comment>(sql))
            {
                comments.Add(c);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));


        //Dictionary<string, SolidColorBrush> colors = new Dictionary<string, SolidColorBrush>() {
        //    { "Orange", new SolidColorBrush(Colors.Orange) },
        //    { "White", new SolidColorBrush(Colors.White) },
        //    { "Gray", new SolidColorBrush(Colors.Gray) }
        //};

        //TypeAccessor acessor = TypeAccessor.Create(typeof(Notebook));

        //private void ValueChanged(object sender, EventArgs e)
        //{
        //    if (sender is Control control)
        //    {
        //        var val = acessor[notebook, control.Name];

        //        if (val == null || val == "")
        //        {
        //            Console.WriteLine("Gray");
        //            Dispatcher.Invoke(() => control.Background = colors["Gray"]);
        //        }
        //        else if (val == acessor[old_notebook, control.Name])
        //        {
        //            Console.WriteLine("White");
        //            Dispatcher.Invoke(() => control.Background = colors["White"]);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Orange");
        //            Dispatcher.Invoke(() => control.Background = colors["Orange"]);
        //        }
        //    }
        //}
    }
}
