using myStore.entities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Linq;
using System.Windows;
using System.IO;
using myStore.converters;
using Microsoft.Win32;

namespace myStore.myPages
{
    public partial class NotebookPage : UserControl, INotifyPropertyChanged
    {
        public delegate void Ready2OpenMainMenuEventHandler(short sort_key_idx, int ID);
        public event Ready2OpenMainMenuEventHandler Ready2OpenMainMenuEvent;

        
        private VerifyObject<Notebook> vNotebook;
        public Notebook notebook {
            get {
                return vNotebook.obj;
            }
            set {
                vNotebook.obj = value;
                OnPropertyChanged("notebook");
            }
        }
        public ObservableCollection<VerifyObject<Comment>> comments { get; set; } = new ObservableCollection<VerifyObject<Comment>>();
        public ObservableCollection<Comment> new_comments { get; set; } = new ObservableCollection<Comment>();



        public ObservableCollection<OS> os_list { get; set; } = new ObservableCollection<OS>();
        public ObservableCollection<Producer> producer_list { get; set; } = new ObservableCollection<Producer>();

        public ObservableCollection<Cpu> cpu_list { get; set; } = new ObservableCollection<Cpu>();
        public ObservableCollection<Gpu> gpu_list { get; set; } = new ObservableCollection<Gpu>();

        public ObservableCollection<string> List_interfaces_wifi { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> List_interfaces_memory_cards { get; set; } = new ObservableCollection<string>();




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
        public RelayCommand CancelChangesCommand { get; }
        public RelayCommand RemoveNotebookCommand { get; }
        public RelayCommand CreateCommentCommand { get; }
        public RelayCommand RemoveCommentCommand { get; }
        public RelayCommand CancelCommentRemovingCommand { get; }
        public RelayCommand CancelCommentCreatingCommand { get; }




        

        public RelayCommand SelectImageInFileExplorerCommand { get; }
        public RelayCommand PasteImageFileCommand { get; }
        public RelayCommand CopyImageCommand { get; }
        public RelayCommand CutImageCommand { get; }
        public RelayCommand ClearImageCommand { get; }




        public NotebookPage(int ID, short sort_key_idx, int page)
        {
            vNotebook = new VerifyObject<Notebook>(("notebook_id", ID));

            

            SelectImageInFileExplorerCommand = new RelayCommand(
                _ => 
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Images (*.bmp, *.png, *.jpg)|*.bmp;*.png;*.jpg|All Files(*.*)|*.*" };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        notebook.image = File.ReadAllBytes(openFileDialog.FileName);
                    }
                },
                _ => true
                );
            PasteImageFileCommand = new RelayCommand(
                _ => notebook.image = Clipboard.GetImage(),
                _ => Clipboard.ContainsImage()
                );
            CopyImageCommand = new RelayCommand(_ => Clipboard.SetImage(ImageConvertor.bArray2BmpSource((byte[])notebook.image)));
            CutImageCommand = new RelayCommand(
                _ =>
                {
                    Clipboard.SetImage(ImageConvertor.bArray2BmpSource((byte[])notebook.image));
                    notebook.image = null;
                },
                _ => notebook.image != null
                );
            ClearImageCommand = new RelayCommand(
                _ => notebook.image = null,
                _ => notebook.image != null
                );



            
            RemoveCommentCommand = new RelayCommand(
                obj => (obj as VerifyObject<Comment>).IsPreparedForRemoving = true,
                _ => isEditable
                );
            CancelCommentRemovingCommand = new RelayCommand(
                obj => (obj as VerifyObject<Comment>).IsPreparedForRemoving = false,
                _ => isEditable
                );
            CancelCommentCreatingCommand = new RelayCommand(
                obj => new_comments.Remove((Comment)obj),
                _ => isEditable
                );
            CreateCommentCommand = new RelayCommand(
                _ => new_comments.Add(new Comment(vNotebook.obj.notebook_id)),
                _ => isEditable
                );
            EditCommand = new RelayCommand(
                _ => isEditable = true,
                _ => !isEditable
                );

            OpenMainMenuCommand = new RelayCommand(_ => Ready2OpenMainMenuEvent(sort_key_idx, page));

            CancelChangesCommand = new RelayCommand(
                _ =>
                {
                    isEditable = false;
                    vNotebook.Reload();

                    interfaces_wifi.SelectedItems = new ObservableCollection<string>() { };
                    interfaces_wifi.SelectedItems = (ObservableCollection<string>)vNotebook.obj.interfaces_wifi;

                    interfaces_memory_cards.SelectedItems = new ObservableCollection<string>() { };
                    interfaces_memory_cards.SelectedItems = (ObservableCollection<string>)vNotebook.obj.interfaces_memory_cards;

                    foreach (var c in comments) c.Reload();

                    OnPropertyChanged("notebook");
                },
                _ => isEditable && vNotebook.obj.notebook_id >= 0
            );

            SaveChangesCommand = new RelayCommand(
                _ =>
                {
                    isEditable = false;

                    if (vNotebook.obj.notebook_id < 0)
                    {
                        CreateNotebook();
                    }
                    else
                    {
                        UpdateNotebook();
                        RemoveComents();
                        UpdateComments();
                    }

                    CreateComments();
                },
                _ => isEditable
            );

            RemoveNotebookCommand = new RelayCommand(
                _ => {
                    RemoveThisNotebook();
                    Ready2OpenMainMenuEvent(sort_key_idx, page);
                },
                _ => isEditable && vNotebook.obj.notebook_id >= 0
            );

            if (ID > 0)
            {
                LoadNotebook(ID);
                LoadComments(ID);
            }
            else
            {
                isEditable = true;
            }

            LoadDictionaries();

            InitializeComponent();
        }

        private async void CreateNotebook()
        {
            vNotebook.obj.notebook_id = await Database.Insert("notebooks", new[] { vNotebook.obj }, "notebook_id");
            vNotebook.SaveChanges(vNotebook.GetChanges());
        }
        private void UpdateNotebook()
        {
            var changes = vNotebook.GetChanges();
            if (changes.Count > 0)
            {
                var sql_template = $"UPDATE notebooks SET {{0}} WHERE notebook_id = { vNotebook.obj.notebook_id }";
                Database.ExecuteOne(sql_template, changes);

                List_interfaces_wifi.Update("interfaces_wifi", changes);
                List_interfaces_memory_cards.Update("interfaces_memory_cards", changes);
            }
            vNotebook.SaveChanges(changes);
        }
        private void RemoveThisNotebook()
        {
            Database.ExecuteOne($"DELETE FROM notebooks WHERE notebook_id = { vNotebook.obj.notebook_id }");
        }


        private async void CreateComments()
        {
            if (new_comments.Count > 0)
            {
                foreach (var c in new_comments.Reverse())
                {
                    comments.Insert(0, new VerifyObject<Comment>(c));
                }

                await Database.Insert("comments", new_comments, "comment_id");

                new_comments.Clear();
            }
        }
        private void UpdateComments()
        {
            foreach (var comment in comments)
            {
                var changes = comment.GetChanges();
                if (changes.Count > 0)
                {
                    var sql_template = $"UPDATE comments SET {{0}} WHERE comment_id = {comment.obj.comment_id}";
                    Database.ExecuteOne(sql_template, changes);
                }
                comment.SaveChanges(changes);
            }
        }

        public void RemoveComents()
        {
            var selected = comments.Where(c => c.IsPreparedForRemoving);

            Database.ExecuteOne($"DELETE FROM comments WHERE comment_id IN ({ string.Join(",", selected.Select(c => c.obj.comment_id)) })");

            foreach (var c in selected)
            {
                comments.Remove(c);
            }
        }
        
        

        private async void LoadDictionaries()
        {
            string sql;

            {
                List_interfaces_wifi.Clear();
                sql = $"SELECT DISTINCT unnest(interfaces_wifi) FROM notebooks";
                await foreach (var el in Database.EnumerateArray<string>(sql))
                {
                    List_interfaces_wifi.Add(el);
                }
            }

            {
                List_interfaces_memory_cards.Clear();
                sql = $"SELECT DISTINCT unnest(interfaces_memory_cards) FROM notebooks";
                await foreach (var el in Database.EnumerateArray<string>(sql))
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
                comments.Add(new VerifyObject<Comment>(c));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));





        private double spec_pos;
        public double SpecPos
        {
            get
            {
                if (spec_pos < 0) return 0;
                else return spec_pos;
            }
            set
            {
                spec_pos = value;
                OnPropertyChanged("SpecPos");
            }
        }

        private double comms_pos;
        public double CommsPos
        {
            get
            {
                if (comms_pos < 0) return 0;
                else return comms_pos;
            }
            set
            {
                comms_pos = value;
                OnPropertyChanged("CommsPos");
            }
        }


        private void MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scrollable_area.ScrollToVerticalOffset(scrollable_area.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void scrollable_area_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            SpecPos = spec_section.TransformToVisual(scrollable_area).Transform(new System.Windows.Point(0, 0)).Y;
            CommsPos = comms_section.TransformToVisual(scrollable_area).Transform(new System.Windows.Point(0, 0)).Y;
        }
    }
}
