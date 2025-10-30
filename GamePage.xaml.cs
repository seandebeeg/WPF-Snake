using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsSnake
{
  /// <summary>
  /// Interaction logic for GamePage.xaml
  /// </summary>
  public partial class GamePage : Page
  {
    private MainWindow _parentWindow;
    private PlayerClass _player;
    private double cellSize;
    public GamePage(MainWindow parentWindow)
    {
      _parentWindow = parentWindow;
      _parentWindow.Title = "Game";
      _parentWindow.MainNavigation.Focus();
      InitializeComponent();
      this.Loaded += (s, e) => this.Focus();
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
        },
        Body = new()
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
      int startX = (BoardArray.GetLength(0) / 2) - 4;
      int startY = (BoardArray.GetLength(1) / 2);
      _player.X = startX;
      _player.Y = startY;

      Grid.SetColumn(_player.Head, _player.X);
      Grid.SetRow(_player.Head, _player.Y);
      GameGrid.Children.Add(_player.Head);
      BoardArray[_player.X, _player.Y] = 1;

      var segment1 = new System.Windows.Shapes.Rectangle
      {
        Height = cellSize,
        Width = cellSize,
        Fill = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(currentSettings.CurrentColor)
      };

      Grid.SetColumn(segment1, _player.X - 1);
      Grid.SetRow(segment1, _player.Y);
      _player.Body.Add(segment1);
      GameGrid.Children.Add(_player.Body[0]);
      BoardArray[Grid.GetColumn(_player.Body[0]), Grid.GetRow(_player.Body[0])] = 1;

      var segment2 = new System.Windows.Shapes.Rectangle
      {
        Height = cellSize,
        Width = cellSize,
        Fill = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(currentSettings.CurrentColor)
      };

      Grid.SetColumn(segment2, _player.X - 2);
      Grid.SetRow(segment2, _player.Y);
      _player.Body.Add(segment2);
      GameGrid.Children.Add(_player.Body[1]);
      BoardArray[Grid.GetColumn(_player.Body[1]), Grid.GetRow(_player.Body[1])] = 1;

      var apple = new Image
      {
        Width = cellSize,
        Height = cellSize,
        Source = new BitmapImage(new Uri("\\Assets\\Apple.png", UriKind.Relative))
      };

      Grid.SetColumn(apple, _player.X + 7);
      Grid.SetRow(apple, _player.Y);
      GameGrid.Children.Add(apple);
    }

    private void TurnPlayer(string Direction)
    {
      switch (Direction)
      {
        case "Right":
          _player.Direction = 90;
        break;
        case "Down":
          _player.Direction = 180;
        break;
        case "Left":
          _player.Direction = 270;
        break;
        case "Up":
          _player.Direction = 0;
        break;
      }
    }

    private void ProcessKeyStrokes(object sender, KeyEventArgs e)
    {
      var PressedKey = e.Key;
      if (PressedKey == Key.Right && _player.Direction != 270)
      {
        TurnPlayer("Right");
      }
      else if (PressedKey == Key.Down && _player.Direction != 0)
      {
        TurnPlayer("Down");
      }
      else if (PressedKey == Key.Left && _player.Direction != 90)
      {
        TurnPlayer("Left");
      }
      else if (PressedKey == Key.Up && _player.Direction != 180)
      {
        TurnPlayer("Up");
      }
    }

    private void RunPreGameFunctions()
    {
      MakeBoard();
      DetermineSpeed();
      LoadStart();
    }
  }
}

