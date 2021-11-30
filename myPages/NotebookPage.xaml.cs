
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




        public ObservableCollection<OS> os_list { get; set; } = new ObservableCollection<OS>();
        public ObservableCollection<Producer> producer_list { get; set; } = new ObservableCollection<Producer>();

        public ObservableCollection<Cpu> cpu_list { get; set; } = new ObservableCollection<Cpu>();
        public ObservableCollection<Gpu> gpu_list { get; set; } = new ObservableCollection<Gpu>();

        public ObservableCollection<string> List_interfaces_wifi { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> List_interfaces_memory_cards { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<VerifieldObject<Comment>> comments { get; set; } = new ObservableCollection<VerifieldObject<Comment>>();




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

        public RelayCommand EditCommand { get; }
        public RelayCommand OpenMainMenuCommand { get; }
        public RelayCommand SaveChangesCommand { get; }
        public RelayCommand RemoveNotebookCommand { get; }
        public RelayCommand CancelChangesCommand { get; }


        public NotebookPage(int ID = -1, int page = 0)
        {
            EditCommand = new RelayCommand(
                _ => isEditable = true,
                _ => !isEditable
                );

            OpenMainMenuCommand = new RelayCommand(_ => Ready2OpenMainMenuEvent(page));

            CancelChangesCommand = new RelayCommand(
                async _ =>
                {
                    isEditable = false;
                    vNotebook.Reload();
                    foreach (var c in comments) c.Reload();
                    OnPropertyChanged("notebook");
                },
                _ => isEditable && ID > 0
            );

            SaveChangesCommand = new RelayCommand(
                async _ =>
                {
                    isEditable = false;

                    {
                        var changes = vNotebook.GetAndSaveChanges();
                        if (changes.Count > 0)
                        {
                            var sql_template = $"UPDATE notebooks SET {{0}} WHERE notebook_id = {ID}";
                            Database.Execute(sql_template, changes);

                            MessageBox.Show("Updated 1!!!");
                        }
                    }

                    foreach (var comment in comments)
                    {
                        var changes = comment.GetAndSaveChanges();
                        if (changes.Count > 0)
                        {
                            var sql_template = $"UPDATE comments SET {{0}} WHERE comment_id = {comment.obj.comment_id}";
                            Database.Execute(sql_template, changes);

                            MessageBox.Show("Updated 2!!!");
                        }
                    }
                },
            _ => isEditable
            );

            RemoveNotebookCommand = new RelayCommand(
                _ => Database.Execute($"DELETE FROM notebooks WHERE notebook_id = {ID}"),
                _ => isEditable
            );

            InitializeComponent();

            if (ID > 0)
            {
                LoadNotebook(ID);
                LoadComments(ID);
                LoadDictionaries();
            }
        }

        private async void LoadDictionaries()
        {
            string sql;

            {
                List_interfaces_wifi.Clear();
                sql = $"SELECT DISTINCT unnest(interfaces_wifi) FROM notebooks";
                await foreach (var el in Database.SimpleEnumerate<string>(sql))
                {
                    List_interfaces_wifi.Add(el);
                }
            }

            {
                List_interfaces_memory_cards.Clear();
                sql = $"SELECT DISTINCT unnest(interfaces_memory_cards) FROM notebooks";
                await foreach (var el in Database.SimpleEnumerate<string>(sql))
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
        }
        private async void LoadNotebook(int ID)
        {
            var sql = $"SELECT * FROM notebooks WHERE notebook_id = {ID}";
            vNotebook.Load(await Database.GetObject<Notebook>(sql));
            OnPropertyChanged("notebook");
        }
        private async void LoadComments(int ID)
        {
            var sql = $"SELECT * FROM comments WHERE notebook_id = {ID} ORDER BY comment_id";

            await foreach (var c in Database.Enumerate<Comment>(sql))
            {
                comments.Add(new VerifieldObject<Comment>(c));
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
