using MySqlConnector;
using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjKozmetika
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection conn;
        private readonly string connectionString = "server=localhost;port=3306;uid=root;database=kozmetika";
        
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_LoadedAsync;
        }
        
        private async void MainWindow_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await ConnectionAsync();
        }

        private async Task<Task> ConnectionAsync()
        {
            try
            {
                conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (conn.State == ConnectionState.Open)
            {
                MessageBox.Show($"Az alkalmazás sikeresen csatlakozott a(z) {conn.Database} nevű adatbázishoz.", "Sikeres csatlakozás!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return Task.CompletedTask;
        }
    }
}
