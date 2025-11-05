using System.Windows;
using System.Windows.Controls;

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
                speedMultiplier = 1;
              break;
              case "Slowness":
                speedMultiplier = 0.5;
              break;
              case "Fast Speed":
                speedMultiplier = 2;
              break;
              case "Super Speed":
                speedMultiplier = 4;
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
        double NewSpeed = _moveTimer.Interval.TotalMilliseconds - 50;
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
        double NewSpeed = _moveTimer.Interval.TotalMilliseconds - 50;
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
  }
}
