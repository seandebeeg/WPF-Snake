using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WindowsSnake
{
  public class GameSettings
  {
    public int HighScore { get; set; }
    public List<Score>? ScoreEntries { get; set; }
    public List<ModifierItem>? Modifiers { get; set; }
    public string? CurrentColor { get; set; }
    public Dictionary<string, string>? SelectedSettings { get; set; }
    public double Multiplier { get; set; }
  }

  public class ModifierItem
  {
    public required string Name { get; set; }
    public double Multiplier { get; set; }
    public bool IsEnabled { get; set; }
    public required string Difficulty { get; set; }
  }
  
  public class Score
  {
    public bool IsHighScore { get; set; }
    public int ScoreNumber { get; set; }
    public string TimeObtained { get; set; }
  }
}
