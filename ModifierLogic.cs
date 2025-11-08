using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsSnake
{
  public partial class GamePage
  {
    private void MakeBoard()
    {
      const int DefaultRows = 20;
      const int DefaultColumns = 20;

      double BoardMultiplier = 1.00;
      int RowNumber = DefaultRows;
      int ColumnNumber = DefaultColumns;

      _parentWindow.Width = _parentWindow.Height;

      if (currentSettings.Modifiers != null)
      {
        foreach (ModifierItem Modifier in currentSettings.Modifiers)
        {
          if (Modifier.Name.Contains("Board"))
          {
            switch (Modifier.Name)
            {
              default:
                BoardMultiplier = 1.00;
                break;
              case "Bigger Board":
                BoardMultiplier = 1.75;
                break;
              case "Small Board":
                BoardMultiplier = 0.5;
                break;
              case "Tiny Board":
                BoardMultiplier = 0.25;
                break;
            }
            RowNumber = ((int)(DefaultRows * BoardMultiplier));
            ColumnNumber = ((int)(DefaultColumns * BoardMultiplier));
            break;
          }
          else
          {
            RowNumber = DefaultRows;
            ColumnNumber = DefaultColumns;
          }
        }
        GameGrid.RowDefinitions.Clear();
        GameGrid.ColumnDefinitions.Clear();

        double availableWidth = _parentWindow.Width;
        double availableHeight = _parentWindow.Height - 40; // Account for title bar
        cellSize = Math.Floor(Math.Min(availableWidth / ColumnNumber, availableHeight / RowNumber));

        _player.Head.Height = cellSize;
        _player.Head.Width = cellSize;

        for (int i = 0; i < RowNumber; i++)
        {
          GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize) });
        }

        for (int j = 0; j < ColumnNumber; j++)
        {
          GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize) });
        }

        BoardArray = new int[ColumnNumber, RowNumber];
      }
      _parentWindow.ResizeMode = ResizeMode.NoResize;

      GameBorder.Height = (RowNumber * cellSize) + 10;
      GameBorder.Width = (ColumnNumber * cellSize) + 10;

      _parentWindow.Height += 20;
    }

    private void DetermineSpeed()
    {
      double speedMultiplier = 1;
      double defaultSpeed = 3; // 3 columns/rows a second 
      PlayerSpeed = defaultSpeed;

      if (currentSettings.Modifiers != null)
      {
        foreach (ModifierItem Modifier in currentSettings.Modifiers)
        {
          if (Modifier.Name.Contains("Speed") || Modifier.Name.Contains("Slow"))
          {
            switch (Modifier.Name)
            {
              default:
                speedMultiplier += 0;
              break;
              case "Slowness":
                speedMultiplier -= 0.5;
              break;
              case "Fast Speed":
                speedMultiplier += 2;
              break;
              case "Super Speed":
                speedMultiplier += 4;
              break;
            }
            PlayerSpeed = speedMultiplier * defaultSpeed;
          }
          else
          {
            PlayerSpeed = defaultSpeed;
          }
        }
      }
    }

    private void AddSpeedOnPoint()
    {
      const int LowestInterval = 50;

      if (currentSettings.Modifiers.Contains(new ModifierItem
      {
        Name = "Speed Increase Every 5 Points",
        Multiplier = 0.45,
        IsEnabled = true,
        Difficulty = "Hard"
      })
          && ApplesEaten % 5 == 0
          && _moveTimer.Interval > TimeSpan.FromMilliseconds(LowestInterval))
      {
        double NewSpeed = _moveTimer.Interval.TotalMilliseconds - 10;
        _moveTimer.Interval = TimeSpan.FromMilliseconds(NewSpeed);
        _moveTimer.Stop();
        _moveTimer.Start();
      }
      if (currentSettings.Modifiers.Contains(new ModifierItem //allows the two modifiers to stack
      {
        Name = "Speed Increase Every Point",
        Multiplier = 0.9,
        IsEnabled = true,
        Difficulty = "Insane"
      })
        && _moveTimer.Interval > TimeSpan.FromMilliseconds(LowestInterval))
      {
        double NewSpeed = _moveTimer.Interval.TotalMilliseconds - 10;
        _moveTimer.Interval = TimeSpan.FromMilliseconds(NewSpeed);
        _moveTimer.Stop();
        _moveTimer.Start();
      }
    }

    private void ProcessDUI()
    {
      if (currentSettings.Modifiers.Contains(new ModifierItem
      {
        Name = "D.W.I",
        Difficulty = "Hard",
        Multiplier = 0.5,
        IsEnabled = true
      }))
      {
        Random randomTime = new();

        int time = randomTime.Next(5, 10);
        _duiTimer = new();
        _duiTimer.Interval = TimeSpan.FromSeconds(time);
        _duiTimer.Tick += (s, e) =>
        {
          DrunkTurn();
          ProcessDUI();
        };
        _duiTimer.Stop();
        _duiTimer.Start();
      }
      else if (currentSettings.Modifiers.Contains(new ModifierItem
      {
        Name = "D.U.I",
        Difficulty = "Insane",
        Multiplier = 1.0,
        IsEnabled = true
      }))
      {
        Random randomTime = new();

        int time = randomTime.Next(1, 4);
        _duiTimer = new();
        _duiTimer.Interval = TimeSpan.FromSeconds(time);
        _duiTimer.Tick += (s, e) =>
        {
          DrunkTurn();
          ProcessDUI();
        };
        _duiTimer.Stop();
        _duiTimer.Start();
      }
    }

    private void DrunkTurn()
    {
      Random TurnNumber = new();

      int Turn = TurnNumber.Next(1, 4);

      if (Turn == 1 && _player.Direction != 270)
      {
        TurnPlayer("Right");
      }
      else if (Turn == 2 && _player.Direction != 0)
      {
        TurnPlayer("Down");
      }
      else if (Turn == 3 && _player.Direction != 90)
      {
        TurnPlayer("Left");
      }
      else if (Turn == 4 && _player.Direction != 180)
      {
        TurnPlayer("Up");
      }
    }

    private void ProcessDoubleGrowth()
    {
      int LastBodySegmentX = Grid.GetColumn(_player.Body.Last());
      int LastBodySegmentY = Grid.GetRow(_player.Body.Last());

      var BodySegment2 = new System.Windows.Shapes.Rectangle()
      {
        Height = cellSize,
        Width = cellSize,
        Fill = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(currentSettings.CurrentColor)
      };

      if (LastBodySegmentX == 0 && LastBodySegmentY == 0)
      {
        Grid.SetColumn(BodySegment2, LastBodySegmentX + 1);
        Grid.SetRow(BodySegment2, LastBodySegmentY);
      }
      else if (LastBodySegmentX == GameGrid.ColumnDefinitions.Count && LastBodySegmentY > GameGrid.RowDefinitions.Count)
      {
        Grid.SetColumn(BodySegment2, LastBodySegmentX - 1);
        Grid.SetRow(BodySegment2, LastBodySegmentY);
      }
      else if (LastBodySegmentX == GameGrid.ColumnDefinitions.Count && LastBodySegmentY == 0)
      {
        Grid.SetColumn(BodySegment2, LastBodySegmentX - 1);
        Grid.SetRow(BodySegment2, LastBodySegmentY);
      }
      else if (LastBodySegmentX == 0 && LastBodySegmentY == GameGrid.RowDefinitions.Count)
      {
        Grid.SetColumn(BodySegment2, LastBodySegmentX + 1);
        Grid.SetRow(BodySegment2, LastBodySegmentY);
      }
      else if (LastBodySegmentX == 0)
      {
        LastBodySegmentY--;
      }
      else if (LastBodySegmentY == 0)
      {
        LastBodySegmentX--;
      }
      else if (LastBodySegmentX == GameGrid.ColumnDefinitions.Count)
      {
        LastBodySegmentY++;
      }
      else if (LastBodySegmentY == GameGrid.RowDefinitions.Count)
      {
        LastBodySegmentX++;
      }
      else
      {
        switch (_player.Direction)
        {
          case 0:
            LastBodySegmentY++;
          break;
          case 90:
            LastBodySegmentX--;
          break;
          case 180:
            LastBodySegmentY--;
          break;
          case 270:
            LastBodySegmentX++;
          break;
        }
        Grid.SetColumn(BodySegment2, LastBodySegmentX);
        Grid.SetRow(BodySegment2, LastBodySegmentY);
      }
      _player.Body.Add(BodySegment2);
      GameGrid.Children.Add(BodySegment2);
      BoardArray[LastBodySegmentX, LastBodySegmentY] = 1;
    }

    private void HandleInvincibility()
    {
      if (_player.Direction == 0) _player.Y = GameGrid.RowDefinitions.Count - 1;
      else if (_player.Direction == 90) _player.X = 0;
      else if (_player.Direction == 180) _player.Y = 0;
      else if (_player.Direction == 270) _player.X = GameGrid.ColumnDefinitions.Count - 1;

      BoardArray[_player.X, _player.Y] = 1;
      Grid.SetColumn(_player.Head, _player.X);
      Grid.SetRow(_player.Head, _player.Y);
      GameGrid.Children.Add(_player.Head);
    }

    ModifierItem MultipleApples = new ModifierItem
    {
      Name = "Multiple Apples",
      Multiplier = -0.2,
      IsEnabled = true,
      Difficulty = "Easy"
    };
    ModifierItem DecoyApples = new ModifierItem
    {
      Name = "Decoy Apples",
      Multiplier = 0.35,
      IsEnabled = true,
      Difficulty = "Hard"
    };
    ModifierItem PoisonApples = new ModifierItem
    {
      Name = "Poison Apples",
      Multiplier = 0.5,
      IsEnabled = true,
      Difficulty = "Insane"
    };

    private void AddExtraApples()
    {
     
      if (currentSettings.Modifiers.Contains(MultipleApples)
        || currentSettings.Modifiers.Contains(DecoyApples)
        || currentSettings.Modifiers.Contains(PoisonApples))
      {
        int ExtraAppleBoardCode = 0;

        foreach (ModifierItem Mod in currentSettings.Modifiers)
        {
          if (Mod.Name == "Multiple Apples") ExtraAppleBoardCode = 3;
          else if (Mod.Name == "Decoy Apples") ExtraAppleBoardCode = 4;
          else if (Mod.Name == "Poison Apples") ExtraAppleBoardCode = 5;
        }
        for (int i = 0; i < 2; i++)
        {
          var AppleElement = new Image
          {
            Width = cellSize,
            Height = cellSize,
            Source = new BitmapImage(new Uri("\\Assets\\Apple.png", UriKind.Relative))
          };

          Grid.SetColumn(AppleElement, _player.X + ((int)(7 * (GameGrid.ColumnDefinitions.Count / 20)) + (i + 1)));
          Grid.SetRow(AppleElement, _player.Y);

          BoardArray[Grid.GetColumn(AppleElement), Grid.GetRow(AppleElement)] += ExtraAppleBoardCode;
          GameGrid.Children.Add(AppleElement);
          ExtraApples.Add(AppleElement);
        }
      }
    }

    private void ReplaceSpecialApple()
    {
      if (currentSettings.Modifiers.Contains(MultipleApples)
        || currentSettings.Modifiers.Contains(DecoyApples)
        || currentSettings.Modifiers.Contains(PoisonApples))
      {
        int ExtraAppleBoardCode = 0;

        foreach (ModifierItem Mod in currentSettings.Modifiers)
        {
          if (Mod.Name == "Multiple Apples") ExtraAppleBoardCode = 3;
          else if (Mod.Name == "Decoy Apples") ExtraAppleBoardCode = 4;
          else if (Mod.Name == "PoisonApples") ExtraAppleBoardCode = 5;
        }

        if (ExtraAppleBoardCode == 3)
        {
          Score += Math.Round((currentSettings.ScoreMultiplier * 100) / 100, 2);
          ApplesEaten++;
        }
        else if (ExtraAppleBoardCode == 5)
        {
          Score -= Math.Round((currentSettings.ScoreMultiplier * 100) / 100, 2);
        }

        ScoreText.Text = ((float)Score).ToString();

        var EatenApple = ExtraApples.Find((Image Apple) =>
          Grid.GetColumn(Apple) == _player.X
          && Grid.GetRow(Apple) == _player.Y)
        ;

        BoardArray[Grid.GetColumn(EatenApple), Grid.GetRow(EatenApple)] = 1;
        GameGrid.Children.Remove(EatenApple);

        var AppleElement = new Image
        {
          Width = cellSize,
          Height = cellSize,
          Source = new BitmapImage(new Uri("\\Assets\\Apple.png", UriKind.Relative))
        };

        int ProjectedAppleX;
        int ProjectedAppleY;

        Random RandomNumber = new();

        do
        {
          ProjectedAppleX = RandomNumber.Next(0, GameGrid.ColumnDefinitions.Count - 1);
          ProjectedAppleY = RandomNumber.Next(0, GameGrid.RowDefinitions.Count - 1);
        }
        while (BoardArray[ProjectedAppleX, ProjectedAppleY] == 1
          || BoardArray[ProjectedAppleX, ProjectedAppleY] == 2
          || BoardArray[ProjectedAppleX, ProjectedAppleY] == 3
          || BoardArray[ProjectedAppleX, ProjectedAppleY] == 4
          || BoardArray[ProjectedAppleX, ProjectedAppleY] == 5
        );
        Grid.SetColumn(AppleElement, ProjectedAppleX);
        Grid.SetRow(AppleElement, ProjectedAppleY);

        BoardArray[Grid.GetColumn(AppleElement), Grid.GetRow(AppleElement)] = ExtraAppleBoardCode;
        GameGrid.Children.Add(AppleElement);
        ExtraApples.Add(AppleElement);
      }
    }
  }
}
