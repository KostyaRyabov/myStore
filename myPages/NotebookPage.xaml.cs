
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

        private Notebook _notebook = new Notebook();
        private Notebook old_notebook = new Notebook();
        public Notebook notebook {
            get { 
                return _notebook;
            }
            set {
                _notebook = value;
                old_notebook = (Notebook)_notebook.Clone();
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

        private List<OS> _os_list;
        public List<OS> os_list
        {
            get
            {
                return _os_list;
            }
            set
            {
                _os_list = value;
                OnPropertyChanged("os_list");
            }
        }
        private List<Producer> _producer_list;
        public List<Producer> producer_list
        {
            get
            {
                return _producer_list;
            }
            set
            {
                _producer_list = value;
                OnPropertyChanged("producer_list");
            }
        }
        public ObservableCollection<string> List_interfaces_wifi { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> List_interfaces_memory_cards { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<Comment> comments { get; set; } = new ObservableCollection<Comment>();

        public RelayCommand ComboboxSelectionChangedCommand { get; }

        public RelayCommand OpenMainMenuCommand { get; }
        public RelayCommand SaveChangesCommand { get; }
        public RelayCommand RemoveNotebookCommand { get; }
        public RelayCommand CancelChangesCommand { get; }

        private delegate void UpdateStateEventHandler(DataState state);
        private event UpdateStateEventHandler UpdateStateEvent;

        private void bindDictionaries()
        {
            //foreach (var el in interfaces_wifi)
            //{
            //    el.IsSelected = notebook.interfaces_wifi.Any(val => val == el.value);
            //}

            //foreach (var el in interfaces_memory_cards)
            //{
            //    el.IsSelected = notebook.interfaces_memory_cards.Any(val => val == el.value);
            //}
        }

        public NotebookPage(int ID = -1)
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

            OpenMainMenuCommand = new RelayCommand(_ => Ready2OpenMainMenuEvent(ID));

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

            //GetBindingObjects();
        }

        private void GetBindingObjects() // слишком трудозатратно - лучше вручную проставить чекеры
        {
            List<Tuple<Binding, DependencyObject>> bindingList = new List<Tuple<Binding, DependencyObject>>();
            DependencySearcher.GetBindingsRecursive(this, bindingList);

            TypeAccessor accessor = TypeAccessor.Create(typeof(Notebook));
            var fields = accessor.GetMembers().Select(m => m.Name);

            bindingList.Where(b => fields.Contains(b.Item1.Path.Path)).Select(b => b.Item2);

            foreach (var b in bindingList)
            {
                if (b.Item2 is TextBox tb)
                {
                    tb.Background = new SolidColorBrush(Colors.Red);
                }


                //Console.WriteLine($"{b.Item1.Path.Path}  {b.Item2.GetType().FullName}");
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
                _os_list = new List<OS>();
                sql = $"SELECT * FROM oss";
                await foreach (var el in Database.Enumerate<OS>(sql))
                {
                    _os_list.Add(el);
                }
                os_list = _os_list.OrderBy(el => el.name).ToList();
            }

            {
                _producer_list = new List<Producer>();
                sql = $"SELECT * FROM producers";
                await foreach (var el in Database.Enumerate<Producer>(sql))
                {
                    _producer_list.Add(el);
                }
                producer_list = _producer_list.OrderBy(el => el.name).ToList();
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


        Dictionary<string, SolidColorBrush> colors = new Dictionary<string, SolidColorBrush>() {
            { "Red", new SolidColorBrush(Colors.Red) },
            { "White", new SolidColorBrush(Colors.White) }
        };

        private void ValueChanged(object sender, EventArgs e)
        {
            var a = this.Resources;

            if (sender is Control control)
            {
                if (notebook[control.Name].Equals(old_notebook[control.Name]))
                {
                    Dispatcher.Invoke(() => control.BorderBrush = control.Background = colors["White"]);
                }
                else
                {
                    Dispatcher.Invoke(() => control.BorderBrush = control.Background = colors["Red"]);
                }
            }
        }
    }
}
