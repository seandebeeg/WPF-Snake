using System.Windows;
using System.Windows.Controls;

namespace WindowsSnake
{
    public partial class MainMenu : Page
    {
        private MainWindow _parentWindow;
        public MainMenu(MainWindow parentWindow)
        {
            _parentWindow = parentWindow;
            _parentWindow.Title = "Main Menu";
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          Button ClickedButton = (Button)e.Source;
          string ClickedButtonContent = ClickedButton.Content as string;

          switch (ClickedButtonContent)
          {
            default:
              _parentWindow.MainNavigation.Navigate(new MainMenu(_parentWindow));
            break;

            case "Start":
              _parentWindow.MainNavigation.Navigate(new GamePage(_parentWindow));
            break;

            case "Colors":
              _parentWindow.MainNavigation.Navigate(new ColorsPage(_parentWindow));
            break;

            case "Settings":
              _parentWindow.MainNavigation.Navigate(new SettingsPage(_parentWindow));
            break;

            case "Modifiers":
              _parentWindow.MainNavigation.Navigate(new ModifiersPage(_parentWindow));
            break;

            case "Scores":
              _parentWindow.MainNavigation.Navigate(new ScoresPage(_parentWindow));
            break;
          }
        }
    }
}
