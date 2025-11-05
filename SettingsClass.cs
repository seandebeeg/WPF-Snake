namespace WindowsSnake
{
  public class GameSettings
  {
    public int HighScore { get; set; }
    public List<Score>? ScoreList { get; set; }
    public List<ModifierItem>? Modifiers { get; set; }
    public string? CurrentColor { get; set; }
    public double ScoreMultiplier { get; set; }
  }

  public class ModifierItem : IEquatable<ModifierItem>
  {
    public required string Name { get; set; }
    public double Multiplier { get; set; }
    public bool IsEnabled { get; set; }
    public required string Difficulty { get; set; }

    public bool Equals(ModifierItem? other)
    {
        if (other is null) return false;
        return Name == other.Name &&
               Multiplier == other.Multiplier &&
               IsEnabled == other.IsEnabled &&
               Difficulty == other.Difficulty;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals(obj as ModifierItem);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Multiplier, IsEnabled, Difficulty);
    }
  }
  
  public class Score
  {
    public bool IsHighScore { get; set; }
    public double ScoreNumber { get; set; }
    public required string TimeObtained { get; set; }
  }
}
