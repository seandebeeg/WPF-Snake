using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.IO;

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
      LoadSettings();
    }

    private void LoadSettings()
    {
      var settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "Snake",
        "settings.json"
      );
      if (!File.Exists(settingsPath))
      {
        Directory.CreateDirectory(settingsPath);
      }
      else
      {
        var currentSettings = JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(settingsPath));
      }

    }

    private void MainMenu_Click(object sender, RoutedEventArgs e)
    {
      _parentWindow.MainNavigation.Navigate(new MainMenu(_parentWindow));
    }
  }
}
