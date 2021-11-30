using System.Windows;
using System.Windows.Controls;

namespace myStore
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            OpenMainPage();
        }

        void OpenMainPage(int p = 0)
        {
            var page = new myPages.MainMenu(p);
            Content = page;

            page.Ready2OpenNotebookEvent += OpenNotebookPage;
        }

        public void OpenNotebookPage(int ID)
        {
            var page = new myPages.NotebookPage(ID, (Content as myPages.MainMenu).CurPage);
            Content = page;

            page.Ready2OpenMainMenuEvent += OpenMainPage;
        }
    }
}
