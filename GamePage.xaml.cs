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
            MakeBoard();
        }

        private int[,] BoardArray;

        private static GameSettings LoadSettings()
        {
          var modifierPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Snake",
            "settings.json"
          );
          GameSettings currentModifiers = JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(modifierPath));
          return currentModifiers;
        }

      private void MakeBoard()
      {
        const int DefaultRows = 30;
        const int DefaultColumns = 30;
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
      
      private readonly GameSettings currentSettings = LoadSettings();
    }
}
