using System.Windows.Controls;
using System.Text.Json;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace WindowsSnake
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
  public partial class GamePage : Page
  {
    private MainWindow _parentWindow;
    private PlayerClass _player;
    private static double cellSize;
    public GamePage(MainWindow parentWindow)
    {
      _parentWindow = parentWindow;
      _parentWindow.Title = "Game";
      InitializeComponent();
      PlayerClass player = new()
      {
        Direction = 90,
        X = 0,
        Y = 0,
        Head = new()
        {
          Width = cellSize,
          Height = cellSize,
          Fill = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(currentSettings.CurrentColor)
        }
      };
      _player = player;
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

    private readonly static GameSettings currentSettings = LoadSettings();
    

    private void LoadStart()
    {
      int startY = (BoardArray.GetLength(0) / 2) - 4;
      int startX = BoardArray.GetLength(1) / 2;
      _player.X = startX;
      _player.Y = startY;
      Grid.SetRow(_player.Head, _player.X);
      Grid.SetColumn(_player.Head, _player.Y);
      GameGrid.Children.Add(_player.Head);
      BoardArray[_player.X, _player.Y] = 1;
    }
    private void RunPreGameFunctions()
    {
      MakeBoard();
      DetermineSpeed();
      LoadStart();
    }
  }
}
