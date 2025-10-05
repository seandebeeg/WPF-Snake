using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WindowsSnake
{
  /// <summary>
  /// Interaction logic for SettingsPage.xaml
  /// </summary>
  public partial class SettingsPage : Page
  {
    private MainWindow _parentWindow;
    private readonly GameSettings _gameSettings;
    private readonly ObservableCollection<SettingsItem> _settings;
    public SettingsPage(MainWindow parentWindow)
    {
      _parentWindow = parentWindow;
      _parentWindow.Title = "Settings";
      _gameSettings = new GameSettings
      {
        Settings = []
      };
      _settings =
      [
        new() {Name = "Dark Mode", IsEnabled = false, Type = "Preference"},
        new() {Name = "Manual Score Save", IsEnabled = false, Type = "Preference"},
        new() {Name = "Invert Up and Down", IsEnabled = false, Type="Controls"},
        new() {Name = "Invert Right and Left", IsEnabled = false, Type="Controls"}
      ];

      InitializeComponent();
      var view = CollectionViewSource.GetDefaultView(_settings);
      view.GroupDescriptions.Clear();
      view.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
      SettingsListView.ItemsSource = view;
      LoadSettings();
    }

    private void LoadSettings()
    {
      try
      {
        var settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Snake",
            "settings.json"
        );

        if (File.Exists(settingsPath))
        {
          var loadedSettings = JsonSerializer.Deserialize<GameSettings>(
              File.ReadAllText(settingsPath));

          if (loadedSettings?.Modifiers != null)
          {
            foreach (var setting in _settings)
            {
              var savedSettings = loadedSettings.Settings
                  .FirstOrDefault(m => m.Name == setting.Name);
              if (savedSettings != null)
              {
                setting.IsEnabled = savedSettings.IsEnabled;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Snake",
            "settings.json"
        );

        GameSettings existingSettings = File.Exists(settingsPath)
            ? JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(settingsPath))
            : new GameSettings();

        existingSettings.Settings = _settings.Where(m => m.IsEnabled).ToList();
        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
        File.WriteAllText(settingsPath, JsonSerializer.Serialize(existingSettings));

      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void MainMenu_Click(object sender, RoutedEventArgs e)
    {
      _parentWindow.MainNavigation.Navigate(new MainMenu(_parentWindow));
    }
  }
}
