using System.Windows.Controls;
using System.Text.Json;
using System.IO;
using System.Windows;

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
      RunPreGameFunctions();
    }

    private int[,] BoardArray;
    private double PlayerSpeed;

    private static GameSettings LoadSettings()
    {
      var settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "Snake",
        "settings.json"
      );

      if (File.Exists(settingsPath))
      {
        GameSettings currentSettings = JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(settingsPath));
        return currentSettings;
      }
      else
      {
        GameSettings defaultSettings = new GameSettings 
        { 
          CurrentColor = "#FF0000FF",
          Modifiers = null,
          Multiplier = 1,
          Settings = null
        };
        File.WriteAllText(settingsPath, JsonSerializer.Serialize(defaultSettings));
        return defaultSettings;
      }
    }

    private readonly GameSettings currentSettings = LoadSettings();

    private void RunPreGameFunctions()
    {
      MakeBoard();
      DetermineSpeed();
    }
  }
}
