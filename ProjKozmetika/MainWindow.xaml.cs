using MySqlConnector;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            EmployeeComboBox.IsEnabled = false;
        }

        private async void MainWindow_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await GetServicesAsync();
            //await GetWorkersAsync();
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

        private async Task GetWorkersAsync(byte ID)
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

            string query = $"SELECT dolgozoID, dolgozoFirstName, dolgozoLastName, dolgozoTel, dolgozoEmail, statusz, szolgáltatasa FROM dolgozók WHERE szolgáltatasa = {ID} ";

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

        /// <summary>
        /// !!NEM HASZNÁLT KÓD!! 
        /// Ellenőrzi, hogy a kiválaszott szolgáltatáshoz a megfelelő dolgozó van-e kiválasztva,
        /// </summary>
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
        private async void ServiceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EmployeeComboBox.IsEnabled = true;
            Szolgaltatas selectedItem = (Szolgaltatas)ServiceComboBox.SelectedItem;
            dolgozok.Clear();
            await GetWorkersAsync(selectedItem.SzolgID);
            EmployeeComboBox.SelectedIndex = 0;
        }
        private void EmployeeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var debug = EmployeeComboBox.SelectedItem;
        }

        private void PreviewTextRegex(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z\S\P{IsBasicLatin}[(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff]]]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PreviewAppCommandsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
