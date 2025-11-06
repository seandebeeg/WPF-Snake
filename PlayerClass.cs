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
      public Rectangle Head { get; set; }
      public List<Rectangle> Body = new List<Rectangle>();

      public PlayerClass(GameSettings currentSettings)
      {
        Head = new Rectangle
        {
          Fill = (Brush)new BrushConverter().ConvertFromString(currentSettings.CurrentColor)
        };
      }
    }
  }
}
