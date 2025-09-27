using System.Windows.Controls;
using System.Text.Json;
using System.IO;
using System.Windows;

namespace WindowsSnake
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private MainWindow _parentWindow;
        public GamePage(MainWindow parentWindow)
        {
            _parentWindow = parentWindow;
            _parentWindow.Title = "Game";
            InitializeComponent();
            RunPreGameFunctions();
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
              Modifiers = null,
              Multiplier = 1,
              SelectedSettings = null
            };
            File.WriteAllText(settingsPath, JsonSerializer.Serialize(defaultSettings));
            return defaultSettings;
          }
        }

      private void MakeBoard()
      {
        const int DefaultRows = 20;
        const int DefaultColumns = 20;
        double BoardMultiplier = 1.00;
        int RowNumber = DefaultRows;
        int ColumnNumber = DefaultColumns;
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

          for (int i = 0; i < RowNumber; i++)
          {
            GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
          }

          for (int j = 0; j < ColumnNumber; j++)
          {
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
          }

          BoardArray = new int[RowNumber, ColumnNumber];
        }
      }

      private void DetermineSpeed()
      {
        double speedMultiplier = 1;
        double defaultSpeed = 3; // 3 columns/rows a second 
        if(currentSettings.Modifiers != null)
        {
          foreach(ModifierItem Modifier in currentSettings.Modifiers)
          {
            if(Modifier.Name.Contains("Speed") || Modifier.Name.Contains("Slow"))
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
          }
        }
      } 
      private readonly GameSettings currentSettings = LoadSettings();

      private void RunPreGameFunctions()
      {
        MakeBoard();
        DetermineSpeed();
      }
    }
}
