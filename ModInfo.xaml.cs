using System.Windows;
using System.Windows.Controls;

namespace WindowsSnake
{
  public partial class ModInfo : Page
  {
    MainWindow _parentWindow;
    public ModInfo(MainWindow parentWindow)
    {
      _parentWindow = parentWindow;
      InitializeComponent();
    }

    private void Modifiers_Click(object sender, RoutedEventArgs e)
    {
      _parentWindow.MainNavigation.Navigate(new ModifiersPage(_parentWindow));
    }
  }
}
