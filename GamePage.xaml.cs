using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Diagnostics;

namespace WindowsSnake
{
  public partial class GamePage : Page
  {
    private GameSettings currentSettings;
    private MainWindow _parentWindow;
    private PlayerClass _player;

    private System.Windows.Threading.DispatcherTimer _moveTimer;
    private System.Windows.Threading.DispatcherTimer _duiTimer;

    public int ApplesEaten = 0;
    public double Score;
    private double cellSize;

    public TextBlock ScoreText;
    public List<Image> ExtraApples = new();
    private Queue<string> TurningQueue = new();

    public bool IsGrowing = false;
    public bool IsTurning = false;
    public bool IsMoving = false;

    public GamePage(MainWindow parentWindow)
    {
      _parentWindow = parentWindow;
      _parentWindow.Title = "Game";
      _parentWindow.MainNavigation.Focus();
      _parentWindow.WindowState = System.Windows.WindowState.Normal;

      currentSettings = LoadSettings();

      InitializeComponent();

      this.Loaded += (s, e) => this.Focus();

      PlayerClass player = new(currentSettings)
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
        if(TurningQueue.Count > 0) TurnPlayer(TurningQueue.Dequeue());
      };

      ProcessDUI();
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
          Modifiers = [],
          ScoreMultiplier = 1,
          ScoreList = [],
          HighScore = 0
        };
        File.WriteAllText(settingsPath, JsonSerializer.Serialize(defaultSettings));
        return defaultSettings;
      }
    }

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

      var DisplayApple = new Image
      {
        Width = 40,
        Height = 40,
        Source = new BitmapImage(new Uri("\\Assets\\Apple.png", UriKind.Relative)),
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Center
      };

      _parentWindow.ScorePanel.Children.Add(DisplayApple);

      ScoreText = new TextBlock
      {
        Text =": 0",
        FontSize = 35,
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Center
      };

      AddExtraApples();

      _parentWindow.ScorePanel.Children.Add(ScoreText);
    }

    private void StartGame(object sender, KeyEventArgs e)
    {
      PageElement.KeyDown -= StartGame;
      PageElement.KeyDown += ProcessKeyStrokes;
      _moveTimer.Start();
    }

    private void TurnPlayer(string Direction)
    {
      while (!IsTurning && !IsMoving) 
      {
        IsTurning = true;
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
        IsTurning = false;
        return;
      }
    }

    private DateTime _lastTurnTime = DateTime.MinValue;
    private Key? _bufferedInput;

    private void ProcessKeyStrokes(object sender, KeyEventArgs e)
    {
      var PressedKey = e.Key;
      var currentTime = DateTime.Now;
      var timeSinceLastTurn = (currentTime - _lastTurnTime).TotalSeconds;

      if (IsValidTurn(PressedKey, _player.Direction))
      {
        _bufferedInput = PressedKey;
      }
      if (_bufferedInput.HasValue)
      {
        ProcessTurn(_bufferedInput.Value);
        _bufferedInput = null;
      }
    }

    private bool IsValidTurn(Key pressedKey, int currentDirection)
    {
      return pressedKey switch
      {
        Key.Right => currentDirection != 270,
        Key.Down => currentDirection != 0,
        Key.Left => currentDirection != 90,
        Key.Up => currentDirection != 180,
        _ => false
      };
    }

    private void ProcessTurn(Key pressedKey)
    {
      if (pressedKey == Key.Right)
      {
        TurningQueue.Enqueue("Right");
        _lastTurnTime = DateTime.Now;
      }
      else if (pressedKey == Key.Down)
      {
        TurningQueue.Enqueue("Down");
        _lastTurnTime = DateTime.Now;
      }
      else if (pressedKey == Key.Left)
      {
        TurningQueue.Enqueue("Left");
        _lastTurnTime = DateTime.Now;
      }
      else if (pressedKey == Key.Up)
      {
        TurningQueue.Enqueue("Up");
        _lastTurnTime = DateTime.Now;
      }
    }

    private void MoveBody()
    {
      while (!IsTurning)
      {
        IsMoving = true;

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
          IsMoving = false;
          return;
        }
        catch (Exception)
        {
          IsMoving = false;
          return;
        }
      }
    }

    private void HandleGrowth()
    {
      IsGrowing = true;
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
      BoardArray[LastBodySegmentX, LastBodySegmentY] = 1;

      if (currentSettings.Modifiers.Contains(new ModifierItem
      {
        Name = "Double Growth",
        Multiplier = 0.75,
        IsEnabled = true,
        Difficulty = "Insane"
      }))
      {
        ProcessDoubleGrowth();
      }
      IsGrowing = false;
    }

    private void CheckForCollision() //multi, poison, and decoy yet to be implemented
    {
      int OccupiedBoardSpaces = 0;
      int HeadX = _player.X;
      int HeadY = _player.Y;
      while (!IsGrowing)
      {
        try
        {
          if (BoardArray[HeadX, HeadY] == 3)//player is on apple
          {
            HandleGrowth();
            ReplaceApple();
            return;
          }
          else if (BoardArray[HeadX, HeadY] == 4
            || BoardArray[HeadX, HeadY] == 5
            || BoardArray[HeadX, HeadY] == 6)
          {
            HandleGrowth();
            ReplaceSpecialApple();
            return;
          }

          foreach (int Space in BoardArray)
          {
            if (Space == 1)
            {
              OccupiedBoardSpaces++;
            }
          }

          ModifierItem Overlap = new ModifierItem
          {
            Name = "Overlap",
            Multiplier = -0.25,
            IsEnabled = true,
            Difficulty = "Easy"
          };
          ModifierItem Invincibility = new ModifierItem
          {
            Name = "Invincibility",
            Multiplier = -1000,
            IsEnabled = true,
            Difficulty = "Easy"
          };
          if (OccupiedBoardSpaces < _player.Body.Count
            && !currentSettings.Modifiers.Contains(Overlap)
            && !currentSettings.Modifiers.Contains(Invincibility))
          {
            ModifierItem DWI = new ModifierItem
            {
              Name = "D.W.I",
              Difficulty = "Hard",
              Multiplier = 0.5,
              IsEnabled = true
            };
            ModifierItem DUI = new ModifierItem
            {
              Name = "D.U.I",
              Difficulty = "Insane",
              Multiplier = 1.0,
              IsEnabled = true
            };

            if (currentSettings.Modifiers.Contains(DWI) || currentSettings.Modifiers.Contains(DUI)) _duiTimer.Stop();
            _moveTimer.Stop();
            HandleLoss();
            return;
          }
          return;
        }
        catch (Exception)
        {

          ModifierItem Invincibility = new ModifierItem
          {
            Name = "Invincibility",
            Multiplier = -1000,
            IsEnabled = true,
            Difficulty = "Easy"
          };
          if (!currentSettings.Modifiers.Contains(Invincibility))
          {
            ModifierItem DWI = new ModifierItem
            {
              Name = "D.W.I",
              Difficulty = "Hard",
              Multiplier = 0.5,
              IsEnabled = true
            };
            ModifierItem DUI = new ModifierItem
            {
              Name = "D.U.I",
              Difficulty = "Insane",
              Multiplier = 1.0,
              IsEnabled = true
            };

            if (currentSettings.Modifiers.Contains(DWI) || currentSettings.Modifiers.Contains(DUI)) _duiTimer.Stop();
            _moveTimer.Stop();
            HandleLoss();
            return;
          }
          else
          {
            HandleInvincibility();
            return;
          }
        }
      }
    }

    private void ReplaceApple()
    {
      ApplesEaten++;
      Score += Math.Round(currentSettings.ScoreMultiplier , 2);

      AddSpeedOnPoint();

      ScoreText.Text = ((float)Score).ToString();

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
        ProjectedAppleX = RandomNumber.Next(0, GameGrid.ColumnDefinitions.Count);
        ProjectedAppleY = RandomNumber.Next(0, GameGrid.RowDefinitions.Count);
      }
      while (BoardArray[ProjectedAppleX, ProjectedAppleY] > 0 );

      Grid.SetColumn(AppleElement, ProjectedAppleX);
      Grid.SetRow(AppleElement, ProjectedAppleY);

      Apple = AppleElement;

      GameGrid.Children.Add(Apple);
      BoardArray[ProjectedAppleX, ProjectedAppleY] += 2;
    }

    private void HandleLoss()
    {
      List<double> CurrentScores = new();

      var settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "Snake",
        "settings.json"
      );

      foreach (Score Entry in currentSettings.ScoreList)
      {
        CurrentScores.Add(Entry.ScoreNumber);
      }

      if (CurrentScores.Count == 0 || Score >= CurrentScores.Max() )
      {
        currentSettings.ScoreList.Add(new Score { IsHighScore = true, ScoreNumber = Score, TimeObtained = DateTime.Today.ToString("d") });
      }
      else
      {
        currentSettings.ScoreList.Add(new Score { IsHighScore = false, ScoreNumber = Score, TimeObtained = DateTime.Today.ToString("d") });
      }
      JsonSerializerOptions WriteOptions = new JsonSerializerOptions { WriteIndented = true };

      var NewScores = JsonSerializer.Serialize<GameSettings>(currentSettings, WriteOptions);
      File.WriteAllText(settingsPath, NewScores);

      GameOverScoreText.Text = $"You got {Score} Point(s)";
      LoserPopup.IsOpen = true;

    }

    private void PlayAgain(object sender, RoutedEventArgs e)
    {
      _parentWindow.ScorePanel.Children.Clear();
      _parentWindow.MainNavigation.Navigate(new GamePage(_parentWindow));
    }

    private void GoToMainMenu(object sender, RoutedEventArgs e)
    {
      _parentWindow.ScorePanel.Children.Clear();
      _parentWindow.MainNavigation.Navigate(new MainMenu(_parentWindow));
    }

    private void RunPreGameFunctions()
    {
      MakeBoard();
      DetermineSpeed();
      LoadStart();
    }
  }
}