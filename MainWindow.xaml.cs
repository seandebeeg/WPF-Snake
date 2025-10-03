using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WindowsSnake
{
    public partial class MainWindow : Window
    {
        private bool isMaximized = false;
        private double originalWidth;
        private double originalHeight;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainNavigation.Navigate(new MainMenu(this));
            originalWidth = this.Width;
            originalHeight = this.Height;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
          if (!isMaximized)
          {
            this.WindowState = WindowState.Maximized;
            isMaximized = true;
            MaximizeButton.Content = new Image { Source = new BitmapImage(new Uri("\\Assets\\Snake-Window-Button.png", UriKind.Relative)) };
          }
          else
          {
            this.WindowState = WindowState.Normal;
            isMaximized = false;
            MaximizeButton.Content = new Image { Source = new BitmapImage(new Uri("\\Assets\\Snake-FullScreen-Button.png", UriKind.Relative)) };
          }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(100)
            };
            fadeOut.Completed += (s, a) => this.Close();
            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
          if(e.ChangedButton == MouseButton.Left) 
          {
            this.DragMove();
          }
        }
    }
}