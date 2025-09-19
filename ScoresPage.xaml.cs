using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Text.Json;

namespace WindowsSnake
{
  /// <summary>
  /// Interaction logic for ScoresPage.xaml
  /// </summary>
  public partial class ScoresPage : Page
  {
    private MainWindow _parentWindow;
    public ScoresPage(MainWindow parentWindow)
    {
      _parentWindow = parentWindow;
      _parentWindow.Title = "Scores";
      InitializeComponent();
      LoadScores();
    }

    private void LoadScores()
    {
      try
      {
        var settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Snake", 
            "settings.json"
        );
        
        if (File.Exists(settingsPath))
        {
            GameSettings currentScores = JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(settingsPath));
            if (currentScores?.ScoreEntries != null)
            {
                var sortedScores = currentScores.ScoreEntries
                    .OrderByDescending(s => s.ScoreNumber)
                    .ToList();
                
                ScoresListView.ItemsSource = sortedScores;
            }
            else
            {
                ScoresListView.ItemsSource = new List<Score>();
            }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error loading scores: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
    private void MainMenu_Click(object sender, RoutedEventArgs e)
    {
      _parentWindow.MainNavigation.Navigate(new MainMenu(_parentWindow));
    }
  }
}
