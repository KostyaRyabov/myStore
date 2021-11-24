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
        public RelayCommand ChangePageCommand { get; }


        public delegate void Ready2OpenNotebookEventHandler(int ID);
        public event Ready2OpenNotebookEventHandler Ready2OpenNotebookEvent;

        private const int limit = 20;

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

        public MainMenu(int page = 0)
        {
            CurPage = page;
            
            notebooks = new ObservableCollection<NotebookView>();

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


            ChangePageCommand = new RelayCommand(
                async obj =>
                {
                    var v = (int)obj;

                    int new_page = (v == -2) ? 0: CurPage + v;

                    IsRemovable = false;
                    bool IsClearable = true;

                    string sql = $"SELECT * FROM notebook_view ORDER BY notebook_id LIMIT {limit} OFFSET {new_page * limit}";
                    
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

                    return (CurPage + (int)obj >= 0 || notebooks.Count < limit);
                }
                );


            InitializeComponent();

            ChangePageCommand.Execute(0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
