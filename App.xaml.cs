using System.Windows;
using System.IO;
using System.Text.Json;

namespace WindowsSnake
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public GameSettings LoadSettings()
        {
          var settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Snake",
            "settings.json"
          );

          if (File.Exists(settingsPath))
          {
            var currentSettings = JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(settingsPath));
            return currentSettings;
          }
          else
          {
            return new GameSettings { CurrentColor = "#FF00FF00", Modifiers = [], HighScore = 0, Settings = [], Multiplier = 1, ScoreEntries = [] };
          }
        }

        public void ApplyDarkTheme()
        {
          GameSettings settings = LoadSettings();
          if (settings.Settings.Contains(new() { Name = "Dark Mode", Type = "Preference", IsEnabled = true }))
          {
            for (int i = Resources.MergedDictionaries.Count - 1; i >= 0; i--)
            {
              var dict = Resources.MergedDictionaries[i];
              if (dict.Source != null && dict.Source.OriginalString.Contains("Theme"))
                Resources.MergedDictionaries.RemoveAt(i);
            }

            var darkTheme = new ResourceDictionary
            {
              Source = new Uri("pack://application:,,,/DarkTheme.xaml")
            };
            Resources.MergedDictionaries.Add(darkTheme);
          }
        }
    }

}
