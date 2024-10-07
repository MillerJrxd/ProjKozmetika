using MySqlConnector;
using ProjKozmetika.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjKozmetika
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Reservation : Window
    {
        MySqlConnection conn;
        private readonly string connectionString = "server=localhost;port=3306;uid=root;database=kozmetika";
        ObservableCollection<Szolgaltatas> szolgatatasok = new ObservableCollection<Szolgaltatas>();
        ObservableCollection<Dolgozo> dolgozok = new ObservableCollection<Dolgozo>();
        ObservableCollection<TimeSpan> times = new ObservableCollection<TimeSpan>();

        public Reservation()
        {
            InitializeComponent();
            Loaded += Reservation_LoadedAsync;
            this.DataContext = this;
            cbDate.ItemsSource = times;
            cbService.ItemsSource = szolgatatasok;
            cbWorker.ItemsSource = dolgozok;
            cbWorker.IsEnabled = false;
            cbDate.IsEnabled = false;
        }
        
        private async void Reservation_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await GetServicesAsync();
            await TimeFillAsync();
        }

        private async Task<Task> GetServicesAsync()
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
            return Task.CompletedTask;
        }
        private async Task<Task> GetWorkersAsync(byte ID)
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
            return Task.CompletedTask;
        }

        private async void cbService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbWorker.IsEnabled = true;
            Szolgaltatas selectedItem = (Szolgaltatas)cbService.SelectedItem;
            dolgozok.Clear();
            await GetWorkersAsync(selectedItem.SzolgID);
            cbDate.IsEnabled = true;
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
        private void btnUsrSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsrFirstName.Text) == true)
            {
                MessageBox.Show("Hibás vagy hiányzó vezetéknév!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsrFirstName.Clear();
                txtUsrFirstName.Focus();
            }
            else if (string.IsNullOrEmpty(txtUsrLastName.Text) == true)
            {
                MessageBox.Show("Hibás vagy hiányzó keresztnév!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsrLastName.Clear();
                txtUsrLastName.Focus();
            }
            else if (string.IsNullOrEmpty(txtUsrEmail.Text) == true && !txtUsrEmail.Text.Contains('@'))
            {
                MessageBox.Show("Hibás vagy hiányzó email cím!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsrEmail.Clear();
                txtUsrEmail.Focus();
            }
            else if (string.IsNullOrEmpty(txtUsrPhone.Text) == true && txtUsrPhone.Text.Count() != 11)
            {
                MessageBox.Show("Hibás vagy hiányzó telefonszám!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsrPhone.Clear();
                txtUsrPhone.Focus();
            }
            else
            {

            }
        }
        private async Task<Task> TimeFillAsync()
        {
            TimeSpan openingTime = new TimeSpan(8, 0, 0);  // 08:00
            TimeSpan closingTime = new TimeSpan(17, 0, 0); // 17:00
            TimeSpan interval = new TimeSpan(0, 15, 0);


            for (TimeSpan currentTime = openingTime; currentTime.Add(interval) <= closingTime; currentTime = currentTime.Add(interval))
            {
                times.Add(currentTime);
            }
            return Task.CompletedTask;
        }

        private void PreviewTextInputString(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^\p{L}]+$");
            e.Handled = regex.IsMatch(e.Text);

            foreach (char c in e.Text)
            {
                int unicodeValue = char.ConvertToUtf32(e.Text, 0);

                if ((unicodeValue >= 0x1F600 && unicodeValue <= 0x1F64F) ||   // Arcok
                  (unicodeValue >= 0x1F300 && unicodeValue <= 0x1F5FF) ||   // Piktogramok
                  (unicodeValue >= 0x1F680 && unicodeValue <= 0x1F6FF) ||   // Szállítás és térképek
                  (unicodeValue >= 0x2600 && unicodeValue <= 0x26FF) ||    // Szimbólumok
                  (unicodeValue >= 0x2700 && unicodeValue <= 0x27BF))       // Dingbatok
                {
                    e.Handled = true; // Megakadályozzuk a bevitel
                    return;
                }
            }
        }
    }
}