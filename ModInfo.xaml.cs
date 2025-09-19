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
