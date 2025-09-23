using System.Windows;
using System.Windows.Controls;

namespace WindowsSnake
{
  /// <summary>
  /// Interaction logic for SettingsPage.xaml
  /// </summary>
  public partial class SettingsPage : Page
  {
    private MainWindow _parentWindow;
    public SettingsPage(MainWindow parentWindow)
    {
      _parentWindow = parentWindow;
      _parentWindow.Title = "Settings";
      InitializeComponent();
    }

    private void MainMenu_Click(object sender, RoutedEventArgs e)
    {
      _parentWindow.MainNavigation.Navigate(new MainMenu(_parentWindow));
    }
  }
}
