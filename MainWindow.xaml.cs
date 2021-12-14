using System;
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
            page.Ready2OpenNotebookEvent += OpenNotebookPage;

            Content = page;
        }

        public void OpenNotebookPage(int ID)
        {
            var page = new myPages.NotebookPage(ID);
            page.Ready2OpenMainMenuEvent += OpenMainPage;

            Content = page;
        }
    }
}
