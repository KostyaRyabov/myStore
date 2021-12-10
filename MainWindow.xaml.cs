using System.Windows;

namespace myStore
{
	public partial class MainWindow : Window
    {
        public MainWindow()
        {
			InitializeComponent();

            OpenMainPage(0,0);
        }

        void OpenMainPage(short sort_key_idx, int p)
        {
            var page = new myPages.MainMenu(sort_key_idx, p);
            Content = page;

            page.Ready2OpenNotebookEvent += OpenNotebookPage;
        }

        public void OpenNotebookPage(short sort_key_idx, int ID)
        {
            var page = new myPages.NotebookPage(ID, sort_key_idx, (Content as myPages.MainMenu).CurPage);
            Content = page;

            page.Ready2OpenMainMenuEvent += OpenMainPage;
        }
    }
}
