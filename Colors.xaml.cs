using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.Json;
using System.IO;


namespace WindowsSnake
{
  public partial class ColorsPage : Page
  {
    private MainWindow _parentWindow;
    private GameSettings _settings;
    public ColorsPage(MainWindow parentWindow)
    {
      _parentWindow = parentWindow;
      _parentWindow.Title = "Colors";
      InitializeComponent();

      var configPath = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
          "Snake", "settings.json"
      );
      if (File.Exists(configPath))
      {
          var json = File.ReadAllText(configPath);
          _settings = JsonSerializer.Deserialize<GameSettings>(json) ?? new GameSettings();
      }
      else
      {
          _settings = new GameSettings();
      }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrEmpty(_settings.CurrentColor))
      {
        try
        {
          ColorSelector.SelectedColor = (Color)ColorConverter.ConvertFromString(_settings.CurrentColor);
        }
        catch
        {
          ColorSelector.SelectedColor = Colors.Black;
        }
      }
      else
      {
        ColorSelector.SelectedColor = Colors.Black;
      }
    }

    private readonly List<Color> PresetColors = new(){ Colors.Blue, Colors.Red, Colors.Black, Colors.Green, Colors.LightGreen, Colors.Pink, Colors.Purple }; 

    private void Previous_Click(object sender, RoutedEventArgs e)
    {
      int CurrentIndex = PresetColors.IndexOf(ColorSelector.SelectedColor.Value);
      if(CurrentIndex == -1 || CurrentIndex == 0)
      {
        ColorSelector.SelectedColor = PresetColors[6];
      }
      else
      {
        ColorSelector.SelectedColor = PresetColors[CurrentIndex - 1];
      }
    }
    private void Next_Click(object sender, RoutedEventArgs e)
    {
      int CurrentIndex = PresetColors.IndexOf(ColorSelector.SelectedColor.Value);
      if(CurrentIndex >= 6)
      {
        ColorSelector.SelectedColor = PresetColors[0];
      }
      else
      {
        ColorSelector.SelectedColor = PresetColors[CurrentIndex + 1];
      }
      _settings.CurrentColor = ColorSelector.SelectedColor.Value.ToString();
    }

    private void MainMenu_Click(object sender, RoutedEventArgs e)
    {
      _parentWindow.MainNavigation.Navigate(new MainMenu(_parentWindow));
    }
    private void Save_Click(object sender, RoutedEventArgs e)
    {
      var SettingsDirectory = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
          "Snake"
      );
      Directory.CreateDirectory(SettingsDirectory);
      var configPath = Path.Combine(SettingsDirectory, "settings.json");
      _settings.CurrentColor = ColorSelector.SelectedColor.Value.ToString();
      var json = JsonSerializer.Serialize(_settings);
      File.WriteAllText(configPath, json);
    }

    

  }
}
