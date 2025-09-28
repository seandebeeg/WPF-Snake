using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WindowsSnake
{
    public partial class ModifiersPage : Page
    {
        private readonly MainWindow _parentWindow;
        private readonly GameSettings _settings;
        private readonly ObservableCollection<ModifierItem> _modifiers;

        public ModifiersPage(MainWindow parentWindow)
        {
            _parentWindow = parentWindow ?? throw new ArgumentNullException(nameof(parentWindow));
            _parentWindow.Title = "Modifiers";
            
            _settings = new GameSettings
            {
                Modifiers = new List<ModifierItem>(),
                SelectedSettings = new Dictionary<string, string>()
            };

            _modifiers = new ObservableCollection<ModifierItem>
            {
                new ModifierItem { Name = "Invincibility", Multiplier = -1000, IsEnabled = false, Difficulty = "Easy"},
                new ModifierItem { Name = "Slowness", Multiplier = -0.5, IsEnabled = false, Difficulty = "Easy" },
                new ModifierItem { Name = "Overlap", Multiplier = -0.25, IsEnabled = false, Difficulty = "Easy" },
                new ModifierItem { Name = "Bigger Board", Multiplier = -0.35, IsEnabled = false, Difficulty = "Easy" },
                new ModifierItem { Name = "Multiple Apples", Multiplier = -0.2, IsEnabled = false, Difficulty = "Easy" },

                new ModifierItem { Name = "Fast Speed", Multiplier = 0.35, IsEnabled = false , Difficulty = "Hard"},
                new ModifierItem { Name = "Small Board", Multiplier = 0.25, IsEnabled = false, Difficulty = "Hard" },
                new ModifierItem { Name = "Decoy Apples", Multiplier = 0.35, IsEnabled = false, Difficulty = "Hard" },
                new ModifierItem { Name = "D.W.I", Multiplier = 0.5, IsEnabled = false, Difficulty = "Hard" },
                new ModifierItem { Name = "Speed Increase Every 5 Points", Multiplier = 0.45, IsEnabled = false, Difficulty = "Hard" },

                new ModifierItem { Name = "Super Speed", Multiplier = 0.85, IsEnabled = false, Difficulty = "Insane" },
                new ModifierItem { Name = "Tiny Board", Multiplier = 0.5, IsEnabled = false, Difficulty = "Insane" },
                new ModifierItem { Name = "Poison Apples", Multiplier = 0.5, IsEnabled = false, Difficulty = "Insane" },
                new ModifierItem { Name = "Double Growth", Multiplier = 0.75, IsEnabled = false, Difficulty = "Insane" },
                new ModifierItem { Name = "D.U.I", Multiplier = 1.0, IsEnabled = false, Difficulty = "Insane" },
                new ModifierItem { Name = "Speed Increase Every Point", Multiplier = 0.9, IsEnabled = false, Difficulty = "Insane" }
            };

            InitializeComponent();
            var view = CollectionViewSource.GetDefaultView(_modifiers);
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("Difficulty"));
            ModifiersListView.ItemsSource = view;
            LoadSettings();
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            _parentWindow.MainNavigation.Navigate(new MainMenu(_parentWindow));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Snake",
                    "settings.json");

                GameSettings existingSettings = File.Exists(settingsPath) 
                    ? JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(settingsPath))
                    : new GameSettings();

                existingSettings.Modifiers = _modifiers.Where(m => m.IsEnabled).ToList();

                double totalMultiplier = 100.0;
                int invincibilityMultiplier = -1000;
                foreach (ModifierItem item in existingSettings.Modifiers)
                {
                    if(item.Multiplier == invincibilityMultiplier || totalMultiplier <= 0)
                    {
                        totalMultiplier = 0;
                        break;
                    }
                    totalMultiplier += (item.Multiplier * 100);
                }
                totalMultiplier /= 100;
                existingSettings.Multiplier = totalMultiplier;
                ScoreMultiplierText.Text = totalMultiplier.ToString() + "x";
                Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
                File.WriteAllText(settingsPath, JsonSerializer.Serialize(existingSettings));

      }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
          _parentWindow.MainNavigation.Navigate(new ModInfo(_parentWindow));
        }

        private void LoadSettings()
        {
            try
            {
                var settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Snake",
                    "settings.json");

                if (File.Exists(settingsPath))
                {
                    var loadedSettings = JsonSerializer.Deserialize<GameSettings>(
                        File.ReadAllText(settingsPath));

                    if (loadedSettings?.Modifiers != null)
                    {
                        foreach (var modifier in _modifiers)
                        {
                            var savedModifier = loadedSettings.Modifiers
                                .FirstOrDefault(m => m.Name == modifier.Name);
                            if (savedModifier != null)
                            {
                                modifier.IsEnabled = savedModifier.IsEnabled;
                                modifier.Multiplier = savedModifier.Multiplier;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
