namespace WindowsSnake
{
  public class GameSettings
  {
    public int HighScore { get; set; }
    public List<Score>? ScoreEntries { get; set; }
    public List<ModifierItem>? Modifiers { get; set; }
    public List<SettingsItem>? Settings { get; set; }
    public string? CurrentColor { get; set; }
    public double ScoreMultiplier { get; set; }
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
    public required string TimeObtained { get; set; }
  }

  public class SettingsItem
  {
    public bool IsEnabled { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
  }
}
