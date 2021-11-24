using myStore.entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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

        
        public RelayCommand GetAllDataCommand { get; }
        public RelayCommand ReloadCommand { get; }


        public delegate void BottomReachedEventHandler();
        public event BottomReachedEventHandler BottomReachedEvent;


        public delegate void Ready2OpenNotebookEventHandler(int ID);
        public event Ready2OpenNotebookEventHandler Ready2OpenNotebookEvent;

        public MainMenu(int ID = -1)
        {
            notebooks = new ObservableCollection<NotebookView>();

            BottomReachedEvent += async () => {
                int id = notebooks.LastOrDefault()?.notebook_id ?? 0;
                
                var sql = $"SELECT * FROM notebook_view WHERE notebook_id > {id} ORDER BY notebook_id limit 50";

                await foreach (NotebookView item in Database.Enumerate<NotebookView>(sql))
                {
                    notebooks.Add(item);
                }
            };


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
                    notebooks.Where(n => n.Removable).ToList().ForEach(n => notebooks.Remove(n));
                    IsRemovable = false;
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


            ReloadCommand = new RelayCommand(
                async _ =>
                {
                    IsRemovable = false;
                    notebooks.Clear();

                    string sql;

                    if (ID < 0) sql = $"SELECT * FROM notebook_view ORDER BY notebook_id limit 50";
                    else sql = $"SELECT * FROM notebook_view WHERE notebook_id < {ID+25} ORDER BY notebook_id";

                    await foreach (NotebookView item in Database.Enumerate<NotebookView>(sql))
                    {
                        notebooks.Add(item);
                    }

                    ScrollToNotebook(ID);
                });


            InitializeComponent();

            ReloadCommand.Execute(0);
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        public async void ScrollToNotebook(int ID)
        {
            if (ID <= 0) return;

            NotebookView notebook_view = notebooks.Where(n => n.notebook_id == ID).FirstOrDefault();

            var a = gList.Items[0] as StackPanel;

            await Task.Delay(100);

            var idx = notebooks.IndexOf(notebook_view);

            var items = FindVisualChildren<StackPanel>(gList).ToList();

            if (items.Count < idx) return;

            var item = items[idx];

            Console.WriteLine($"{ID} -> {idx}");

            var currentScrollPosition = ScrollableMenu.VerticalOffset;
            var point = new Point(0, currentScrollPosition);
            var targetPosition = item.TransformToVisual(ScrollableMenu).Transform(point);
            ScrollableMenu.ScrollToVerticalOffset(targetPosition.Y);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset > (sender as ScrollViewer).ScrollableHeight - 500) { BottomReachedEvent(); }
        }


        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
