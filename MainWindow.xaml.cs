using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace myStore
{
	public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<string> messages = new ObservableCollection<string>();
        public ObservableCollection<string> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged("Messages");
            }
        }

        private UserControl cur_page;
        public UserControl CurPage
        {
            get => cur_page;
            set
            {
                cur_page = value;
                OnPropertyChanged("CurPage");
            }
        }


        public MainWindow()
        {
			InitializeComponent();

            Database.ThrowMessage += str =>
            {
                messages.Add(str);
                var t = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(60000) };
                t.Tick += (o, e) => { t.Stop(); messages.Remove(str); };
                t.Start();
            };

            OpenMainPage();
        }

        void OpenMainPage()
        {
            var page = new myPages.MainMenu();
            page.Ready2OpenNotebookEvent += OpenNotebookPage;

            CurPage = page;
        }

        public void OpenNotebookPage(int ID)
        {
            var page = new myPages.NotebookPage(ID);
            page.Ready2OpenMainMenuEvent += OpenMainPage;

            CurPage = page;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private void TextBlock_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            messages.RemoveAt(0);
        }
    }
}
