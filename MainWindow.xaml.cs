using System.Windows;

namespace myStore
{
	public partial class MainWindow : Window
    {
        public MainWindow()
        {
			InitializeComponent();

            OpenMainPage();
        }

        void OpenMainPage()
        {
            var page = new myPages.MainMenu();
            Content = page;

            page.Ready2OpenNotebookEvent += OpenNotebookPage;
        }

        public void OpenNotebookPage(int ID)
        {
            var page = new myPages.NotebookPage(ID);
            Content = page;

            page.Ready2OpenMainMenuEvent += OpenMainPage;
        }
    }
}
