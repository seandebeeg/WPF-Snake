using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsSnake
{
  public partial class GamePage : Page
  {
    private MainWindow _parentWindow;
    private PlayerClass _player;
    private double cellSize;
    private System.Windows.Threading.DispatcherTimer _moveTimer;
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

      _moveTimer = new();
      _moveTimer.Interval = TimeSpan.FromSeconds(1 / PlayerSpeed);
      _moveTimer.Tick += (s, e) => 
      {
        MoveBody();
        CheckForCollision();
      };
      _moveTimer.Start();
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
      int startX = (BoardArray.GetLength(0) / 2) - ((int)(4 * (GameGrid.ColumnDefinitions.Count / 20)));
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

      Grid.SetColumn(apple, _player.X + ((int)(7 * (GameGrid.ColumnDefinitions.Count / 20))));
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

    private void MoveBody()
    {
      List<int> BodySegmentsX = new();
      List<int> BodySegmentsY = new();
      
      int HeadX = Grid.GetColumn(_player.Head);
      int HeadY = Grid.GetRow(_player.Head);
      int OldHeadX = HeadX;
      int OldHeadY = HeadY;
      int index = 0;

      BoardArray[ HeadX, HeadY ] = 0;

      switch (_player.Direction)
      {
        case 0:
          HeadY--;
        break;
        case 90:
          HeadX++;
        break;
        case 180:
          HeadY++;
        break;
        case 270:
          HeadX--;
        break;
      }

      BoardArray[ HeadX, HeadY ] = 1;

      Grid.SetColumn(_player.Head, HeadX);
      Grid.SetRow(_player.Head, HeadY);

      foreach (var BodySegment in _player.Body)
      {
        BodySegmentsX.Add(Grid.GetColumn(BodySegment));
        BodySegmentsY.Add(Grid.GetRow(BodySegment));

        if (index == 0)
        {
          BoardArray[BodySegmentsX[index], BodySegmentsY[index]] = 0;
          BoardArray[OldHeadX, OldHeadY] = 1;

          Grid.SetColumn(BodySegment, OldHeadX);
          Grid.SetRow(BodySegment, OldHeadY);
        }
        else
        {
          BoardArray[BodySegmentsX[index], BodySegmentsY[index]] = 0;
          BoardArray[BodySegmentsX[index - 1], BodySegmentsY[index - 1]] = 1;

          Grid.SetColumn(BodySegment, BodySegmentsX[index - 1]);
          Grid.SetRow(BodySegment, BodySegmentsY[index - 1]);
        }
        index++;
      }
    }

    private void CheckForCollision()
    {
      List<int> BodySegmentsX = new();
      List<int> BodySegmentsY = new();
      int OccupiedBoardSpaces = 0;

      int HeadX = Grid.GetColumn(_player.Head);
      int HeadY = Grid.GetRow(_player.Head);

      foreach (var BodySegment in _player.Body)
      {
        BodySegmentsX.Add(Grid.GetColumn(BodySegment));
        BodySegmentsY.Add(Grid.GetRow(BodySegment));
      }

      foreach(int Space in BoardArray)
      {
        if(Space == 1)
        {
          OccupiedBoardSpaces++;
        }
      }

      if(
        OccupiedBoardSpaces > _player.Body.Count + 1 
        && !currentSettings.Modifiers.Contains(new ModifierItem { Name = "Overlap", Multiplier = -0.25, IsEnabled = false, Difficulty = "Easy" }) 
        && !currentSettings.Modifiers.Contains(new ModifierItem { Name = "Invincibility", Multiplier = -1000, IsEnabled = false, Difficulty = "Easy" })
      )
      {
        _moveTimer.Stop();
      } 
      else if (HeadX < 0 || HeadY < 0 || HeadX > GameGrid.ColumnDefinitions.Count || HeadY > GameGrid.RowDefinitions.Count)
      {
        _moveTimer.Stop();
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