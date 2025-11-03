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
      _parentWindow.WindowState = System.Windows.WindowState.Normal;

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
        CheckForCollision();
        MoveBody();
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

    private Image Apple = new();

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

      Apple = new Image
      {
        Width = cellSize,
        Height = cellSize,
        Source = new BitmapImage(new Uri("\\Assets\\Apple.png", UriKind.Relative))
      };

      Grid.SetColumn(Apple, _player.X + ((int)(7 * (GameGrid.ColumnDefinitions.Count / 20))));
      Grid.SetRow(Apple, _player.Y);

      BoardArray[Grid.GetColumn(Apple), Grid.GetRow(Apple)] += 2;

      GameGrid.Children.Add(Apple);
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

      switch (_player.Direction)
      {
        case 0:
          HeadY--;
          _player.Y--;
        break;
        case 90:
          HeadX++;
          _player.X++;
        break;
        case 180:
          HeadY++;
          _player.Y++;
        break;
        case 270:
          HeadX--;
          _player.X--;
        break;
      }

      try
      { 
        BoardArray[HeadX, HeadY] += 1;

        Grid.SetColumn(_player.Head, HeadX);
        Grid.SetRow(_player.Head, HeadY);

        foreach (var BodySegment in _player.Body)
        {
          BodySegmentsX.Add(Grid.GetColumn(BodySegment));
          BodySegmentsY.Add(Grid.GetRow(BodySegment));

          if (index == 0)
          {
            BoardArray[BodySegmentsX[index], BodySegmentsY[index]] = 0;
            BoardArray[OldHeadX, OldHeadY] += 1;

            Grid.SetColumn(BodySegment, OldHeadX);
            Grid.SetRow(BodySegment, OldHeadY);
          }
          else
          {
            BoardArray[BodySegmentsX[index], BodySegmentsY[index]] = 0;
            BoardArray[BodySegmentsX[index - 1], BodySegmentsY[index - 1]] += 1;

            Grid.SetColumn(BodySegment, BodySegmentsX[index - 1]);
            Grid.SetRow(BodySegment, BodySegmentsY[index - 1]);
          }

          index++;
        }
      } 
      catch(Exception)
      {
        if(!currentSettings.Modifiers.Contains(new ModifierItem() 
          { 
            Difficulty = "Easy",
            Name = "Invincibility",
            IsEnabled = true,
            Multiplier = -1000
          }))
        {
          _moveTimer.Stop();
          //rest of loser logic goes here
        }
        else
        {
          //invincibility logic goes here
        }
      }
    }

    private void CheckForCollision() //apple logic to be implemented
    {
      List<int> BodySegmentsX = new();
      List<int> BodySegmentsY = new();
      int OccupiedBoardSpaces = 0;

      int HeadX = _player.X;
      int HeadY = _player.Y;

      if (BoardArray[HeadX, HeadY] >= 3)//player is on apple
      {
          int LastBodySegmentX = Grid.GetColumn(_player.Body.Last());
          int LastBodySegmentY = Grid.GetRow(_player.Body.Last());

          var BodySegment = new System.Windows.Shapes.Rectangle()
          {
            Height = cellSize,
            Width = cellSize,
            Fill = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(currentSettings.CurrentColor)
          };

          Grid.SetColumn(BodySegment, LastBodySegmentX);
          Grid.SetRow(BodySegment, LastBodySegmentY);

          _player.Body.Add(BodySegment);
          GameGrid.Children.Add(BodySegment);

          ReplaceApple();
          return;
      }

      foreach (int Space in BoardArray)
      {
          if (Space == 1)
          {
              OccupiedBoardSpaces++;
          }
      }

      if (OccupiedBoardSpaces < _player.Body.Count)
      {
        _moveTimer.Stop();
        //rest of loser logic goes here
      }
    }

    private void ReplaceApple()
    {
      var AppleElement = new Image
      {
        Width = cellSize,
        Height = cellSize,
        Source = new BitmapImage(new Uri("\\Assets\\Apple.png", UriKind.Relative))
      };

      GameGrid.Children.Remove(Apple);

      BoardArray[Grid.GetColumn(Apple), Grid.GetRow(Apple)] -= 2;

      Random RandomNumber = new();

      int ProjectedAppleX;
      int ProjectedAppleY;

      do
      {
        ProjectedAppleX = RandomNumber.Next(0, GameGrid.ColumnDefinitions.Count - 1);
        ProjectedAppleY = RandomNumber.Next(0, GameGrid.RowDefinitions.Count - 1);
      }while (BoardArray[ProjectedAppleX, ProjectedAppleY] == 1 || BoardArray[ProjectedAppleX, ProjectedAppleY] == 2);

      Grid.SetColumn(AppleElement, ProjectedAppleX);
      Grid.SetRow(AppleElement, ProjectedAppleY);

      Apple = AppleElement;

      GameGrid.Children.Add(Apple);
      BoardArray[ProjectedAppleX, ProjectedAppleY] += 2;
    }

    private void RunPreGameFunctions()
    {
      MakeBoard();
      DetermineSpeed();
      LoadStart();
    }
  }
}