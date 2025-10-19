using System.Windows.Media;
using System.Windows.Shapes;

namespace WindowsSnake
{
  public partial class GamePage
  {
    public class PlayerClass
    {
      public required int X { get; set; }
      public required int Y { get; set; }
      public required int Direction { get; set; }
      public required Rectangle Head = new Rectangle { Fill = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString(currentSettings.CurrentColor) };
    }
  }
}
