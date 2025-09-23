using System.Windows.Controls;

namespace WindowsSnake
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private MainWindow _parentWindow;
        public GamePage(MainWindow parentWindow)
        {
            _parentWindow = parentWindow;
            _parentWindow.Title = "Game";
            InitializeComponent();
        }
    }
}
