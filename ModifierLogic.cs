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
            RowNumber = ((int)(DefaultRows * BoardMultiplier)); // will make the perimeter the death zone
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
          var TestText = new TextBlock() { Text = i.ToString() };
          Grid.SetRow(TestText, i);
          GameGrid.Children.Add(TestText);

        }

        for (int j = 0; j < ColumnNumber; j++)
        {
          GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize) });
          var TestText = new TextBlock() { Text = j.ToString() };
          Grid.SetColumn(TestText, j);
          GameGrid.Children.Add(TestText);

        }

        BoardArray = new int[ColumnNumber, RowNumber];
      }
      _parentWindow.ResizeMode = ResizeMode.NoResize;

      GameBorder.Height = (RowNumber * cellSize) + 10;
      GameBorder.Width = (ColumnNumber * cellSize) + 10;
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
  }
}
