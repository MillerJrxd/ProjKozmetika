using MySqlConnector;
using System.Collections.ObjectModel;
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
        ObservableCollection<Szolgaltatas> szolgatatasok = new ObservableCollection<Szolgaltatas>();
        ObservableCollection<Dolgozo> dolgozok = new ObservableCollection<Dolgozo>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_LoadedAsync;
            ServiceComboBox.ItemsSource = szolgatatasok;
            EmployeeComboBox.ItemsSource = dolgozok;
            this.DataContext = this;
        }

        private async void MainWindow_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //await ConnectionAsync();
            await GetServicesAsync();
            await GetWorkersAsync();
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
            return Task.CompletedTask;
        }
        private async Task GetServicesAsync()
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

            string query = "SELECT szolgaltatasID, szolgaltatasKategoria, szolgaltatasIdotartam, szolgaltatasAr FROM Szolgáltatás";

            var command = new MySqlCommand(query, conn);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                szolgatatasok.Add(new Szolgaltatas(reader.GetByte(0), reader.GetString(1), reader.GetTimeSpan(2), reader.GetInt32(3)));
            }
            await conn.CloseAsync();
        }

        private async Task GetWorkersAsync()
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

            string query = "SELECT dolgozoID, dolgozoFirstName, dolgozoLastName, dolgozoTel, dolgozoEmail, statusz, szolgáltatasa FROM dolgozók";

            var command = new MySqlCommand(query, conn);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.GetBoolean(5) == false)
                {
                    continue;
                }
                else dolgozok.Add(new Dolgozo(reader.GetByte(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetBoolean(5), reader.GetByte(6)));
            }
            await conn.CloseAsync();
        }

        private void IsCorrectWorker()
        {
            Dolgozo chosenWorker = (Dolgozo)EmployeeComboBox.SelectedItem;
            Szolgaltatas chosenService = (Szolgaltatas)ServiceComboBox.SelectedItem;

            if (chosenWorker == null || chosenService == null)
            {
                return;
            }

            var worker = dolgozok.ToList().Find(x => x.DolgozoFullName == $"{chosenWorker.DolgLastName} {chosenWorker.DolgFirstName}");
            var service = szolgatatasok.ToList().Find(x => x.SzolgKategoria == chosenService.SzolgKategoria);

            if (worker == null || service == null)
            {
                MessageBox.Show("A kiválasztott dolgozó vagy szolgáltatás nem található.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (worker.Szolgaltatas != service.SzolgID)
            {
                MessageBox.Show("Figyelem! Az általad válaszott dolgozó nem biztosítja a válaszott szolgáltatás!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmployeeComboBox.SelectedItem = null;
                ServiceComboBox.SelectedItem = null;
            }

        }
        private  void EmployeeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeComboBox.SelectedItem != null && ServiceComboBox.SelectedItem != null)
            {
                IsCorrectWorker();
            }
        }
        private async void ServiceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeComboBox.SelectedItem != null && ServiceComboBox.SelectedItem != null)
            {
                IsCorrectWorker();
            }
        }
    }
}
