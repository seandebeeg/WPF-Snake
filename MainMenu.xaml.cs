using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
