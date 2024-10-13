using MySqlConnector;
using ProjKozmetika.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Security.AccessControl;
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
    /// Interaction logic for Service.xaml
    /// </summary>
    public partial class Service : Window
    {
        MySqlConnection conn;
        ObservableCollection<TimeSpan> Times {  get; set; } = new ObservableCollection<TimeSpan>();
        private bool editMode;
        private byte editServiceId;

        public Service(bool editBool, params byte[] id)
        {
            InitializeComponent();
            cbServiceTime.ItemsSource = Times;
            this.DataContext = this;
            this.editMode = editBool;
            Loaded += Service_Loaded;

            if (id.Length != 1)
            {
                return;
            }
            else this.editServiceId = id[0];

        }
        private void Service_Loaded(object sender, RoutedEventArgs e)
        {
            TimeFill();
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
        private void PreviewNumInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^\d]");
            e.Handled |= regex.IsMatch(e.Text);
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
        private async void btnServiceSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (editMode == false)
            {
                if (string.IsNullOrEmpty(txtServiceName.Text) == true)
                {
                    MessageBox.Show("Hibás vagy hiányzó szolgáltatás név!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServiceName.Clear();
                    txtServiceName.Focus();
                }
                else if (string.IsNullOrEmpty(txtServicePrice.Text) == true || txtServicePrice.Text.StartsWith("0"))
                {
                    MessageBox.Show("Hibás vagy hiányzó szolgáltatás ár!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServicePrice.Clear();
                    txtServicePrice.Focus();
                }
                else if (cbServiceTime.SelectedIndex == -1)
                {
                    MessageBox.Show("Nem lehet szolgáltatást rögzíteni a hossza nélkül!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {

                    using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
                    {
                        await conn.OpenAsync();

                        string checkServiceQuery = @"SELECT szolgaltatasID FROM szolgáltatás 
                                   WHERE szolgaltatasKategoria = @szolgaltatasKategoria 
                                   AND szolgaltatasIdotartam = @szolgaltatasIdotartam 
                                   AND szolgaltatasAr = @szolgaltatasAr";
                        int serviceID;

                        using (var checkServiceCommand = new MySqlCommand(checkServiceQuery, conn))
                        {
                            checkServiceCommand.Parameters.AddWithValue("@szolgaltatasKategoria ", txtServiceName.Text.Trim());
                            checkServiceCommand.Parameters.AddWithValue("@szolgaltatasIdotartam", txtServicePrice.Text.Trim());
                            checkServiceCommand.Parameters.AddWithValue("@szolgaltatasAr", cbServiceTime.SelectedItem);

                            var result = await checkServiceCommand.ExecuteScalarAsync();

                            if (result != null)
                            {
                                serviceID = Convert.ToInt32(result);
                            }
                            else
                            {
                                string addWorkerQuery = @"INSERT INTO szolgáltatás (szolgaltatasKategoria, szolgaltatasIdotartam, szolgaltatasAr) 
                                         VALUES (@szolgaltatasKategoria, @szolgaltatasIdotartam, @szolgaltatasAr)";

                                using (var addServiceCommand = new MySqlCommand(addWorkerQuery, conn))
                                {
                                    addServiceCommand.Parameters.AddWithValue("@szolgaltatasKategoria", txtServiceName.Text.Trim());
                                    addServiceCommand.Parameters.AddWithValue("@szolgaltatasIdotartam", txtServicePrice.Text.Trim());
                                    addServiceCommand.Parameters.AddWithValue("@szolgaltatasAr", cbServiceTime.SelectedItem);

                                    await addServiceCommand.ExecuteNonQueryAsync();
                                }

                                using (var getServiceIDCommand = new MySqlCommand("SELECT LAST_INSERT_ID();", conn))
                                {
                                    serviceID = Convert.ToInt32(await getServiceIDCommand.ExecuteScalarAsync());
                                }
                                MessageBox.Show("Sikeres szolgáltatás felvétel!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                            }
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtServiceName.Text) == true)
                {
                    MessageBox.Show("Hibás vagy hiányzó szolgáltatás név!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServiceName.Clear();
                    txtServiceName.Focus();
                }
                else if (string.IsNullOrEmpty(txtServicePrice.Text) == true || txtServicePrice.Text.StartsWith("0") || int.Parse(txtServicePrice.Text) <  1000)
                {
                    MessageBox.Show("Hibás vagy hiányzó szolgáltatás ár!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServicePrice.Clear();
                    txtServicePrice.Focus();
                }
                else if (cbServiceTime.SelectedIndex == -1)
                {
                    MessageBox.Show("Nem lehet szolgáltatást rögzíteni a hossza nélkül!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {

                    using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
                    {
                        await conn.OpenAsync();

                        string updateServiceQuery = @"UPDATE szolgáltatás
                                         SET szolgaltatasKategoria = @szolgaltatasKategoria, 
                                             szolgaltatasIdotartam = @szolgaltatasIdotartam, 
                                             szolgaltatasAr = @szolgaltatasAr, 
                                         WHERE szolgaltatasID = @szolgaltatasID";

                        using (var updateWorkerCommand = new MySqlCommand(updateServiceQuery, conn))
                        {
                            updateWorkerCommand.Parameters.AddWithValue("@szolgaltatasKategoria", txtServiceName.Text.Trim());
                            updateWorkerCommand.Parameters.AddWithValue("@szolgaltatasIdotartam", txtServicePrice.Text.Trim());
                            updateWorkerCommand.Parameters.AddWithValue("@szolgaltatasAr", cbServiceTime.SelectedItem);
                            updateWorkerCommand.Parameters.AddWithValue("@szolgaltatasID", editServiceId);

                            await updateWorkerCommand.ExecuteNonQueryAsync();
                            MessageBox.Show("Sikeres szolgáltatás módosítás!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        this.Close();
                    }
                }
            }
        }
        private void TimeFill()
        {
            TimeSpan startTime = new TimeSpan(0, 15, 0);
            TimeSpan endTime = new TimeSpan(4, 45, 0);
            TimeSpan interval = new TimeSpan(0, 15, 0);

            for (TimeSpan time = startTime; time + interval <= endTime; time += interval)
            {
                Times.Add(time);
            }
        }
        private void txtServicePrice_MouseEnter(object sender, MouseEventArgs e)
        {

            ToolTip tooltip = new ToolTip();
            tooltip.Content = "Egy szolgáltatás minimum össszege nem lehet kisebb mint 1000 forint.";
            txtServicePrice.ToolTip = tooltip;

            ToolTipService.SetInitialShowDelay(txtServicePrice, 0);
            ToolTipService.SetShowDuration(txtServicePrice, 2500);
            ToolTipService.SetToolTip(txtServicePrice, tooltip);
        }
        private void txtServicePrice_MouseLeave(object sender, MouseEventArgs e)
        {
            txtServicePrice.ToolTip = null;
        }
    }
}