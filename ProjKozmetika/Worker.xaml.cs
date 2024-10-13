using MySqlConnector;
using ProjKozmetika.Classes;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjKozmetika
{
    /// <summary>
    /// Interaction logic for Worker.xaml
    /// </summary>
    public partial class Worker : Window
    {
        MySqlConnection conn;
        ObservableCollection<Szolgaltatas> services = new ObservableCollection<Szolgaltatas>();
        private bool editMode;
        private byte editWorkerId;

        public Worker(bool editBool, params byte[] id)
        {
            InitializeComponent();
            cbWorkCategory.ItemsSource = services;
            this.DataContext = this;
            Loaded += Worker_LoadedAsync;
            this.editMode = editBool;
            
            if (id.Length != 1)
            {
                return;
            }
            else this.editWorkerId = id[0];
            if (editMode == true)
            {
                chkWorkerStatus.IsEnabled = true;
                btnDolgSubmit.Content = "Módosítás";
            }
        }
        private async void Worker_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await GetExsistingServices();
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
        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
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
        private async Task<Task> GetExsistingServices()
        {
            using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
            {
                await conn.OpenAsync();

                string query = @"SELECT szolgaltatasID, szolgaltatasKategoria, szolgaltatasIdotartam, szolgaltatasAr FROM szolgáltatás";

                using (var command = new MySqlCommand(query, conn))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            services.Add(new Szolgaltatas(
                                    reader.GetByte(0),
                                    reader.GetString(1),
                                    reader.GetTimeSpan(2),
                                    reader.GetInt32(3)));
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }
        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            if (editMode == false)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.Content = "Dolgozó fevételénél nem lehet módosítani a dolgozó státuszát!";
                recBouding.ToolTip = tooltip;

                ToolTipService.SetInitialShowDelay(recBouding, 0);
                ToolTipService.SetShowDuration(recBouding, 2500); 
                ToolTipService.SetToolTip(recBouding, tooltip);
            }
        }
        private void recBouding_MouseLeave(object sender, MouseEventArgs e)
        {
            recBouding.ToolTip = null;
        }
        private void txtDolgPhone_MouseEnter(object sender, MouseEventArgs e)
        {
            ToolTip tooltip = new ToolTip();
            tooltip.Content = "Elvárt telefonszám formátum: '06301234567'.";
            txtDolgPhone.ToolTip = tooltip;

            ToolTipService.SetInitialShowDelay(txtDolgPhone, 0);
            ToolTipService.SetShowDuration(txtDolgPhone, 2500);
            ToolTipService.SetToolTip(txtDolgPhone, tooltip);
        }
        private void txtDolgPhone_MouseLeave(object sender, MouseEventArgs e)
        {
            txtDolgPhone.ToolTip= null;
        }

        private async void btnDolgSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (editMode == false)
            {
                if (string.IsNullOrEmpty(txtDolgFirstName.Text) == true)
                {
                    MessageBox.Show("Hibás vagy hiányzó vezetéknév!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDolgFirstName.Clear();
                    txtDolgFirstName.Focus();
                }
                else if (string.IsNullOrEmpty(txtDolgLastName.Text) == true)
                {
                    MessageBox.Show("Hibás vagy hiányzó keresztnév!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDolgLastName.Clear();
                    txtDolgLastName.Focus();
                }
                else if (string.IsNullOrEmpty(txtDolgEmail.Text) == true || IsValidEmail(txtDolgEmail.Text) == false)
                {
                    MessageBox.Show("Hibás vagy hiányzó email cím!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDolgEmail.Clear();
                    txtDolgEmail.Focus();
                }
                else if (string.IsNullOrEmpty(txtDolgPhone.Text) == true || txtDolgPhone.Text.Count() != 11)
                {
                    MessageBox.Show("Hibás vagy hiányzó telefonszám!\nAjánlott formátum: 06301234567", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDolgPhone.Clear();
                    txtDolgPhone.Focus();
                }
                else if (cbWorkCategory.SelectedIndex == -1)
                {
                    MessageBox.Show("Szolgáltatás kiválasztás nélkül nem lehet dolgozót felvenni!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var selectedService = cbWorkCategory.SelectedItem as Szolgaltatas;

                    using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
                    {
                        await conn.OpenAsync();

                        string checkWorkerquery = @"SELECT dolgozoId FROM dolgozók 
                                   WHERE dolgozoFirstName = @dolgozoFirstName 
                                   AND dolgozoLastName = @dolgozoLastName 
                                   AND dolgozoTel = @dolgozoTel 
                                   AND dolgozoEmail = @dolgozoEmail";
                        int dolgozoID;

                        using (var checkWorkerCommand = new MySqlCommand(checkWorkerquery, conn))
                        {
                            checkWorkerCommand.Parameters.AddWithValue("@dolgozoFirstName", txtDolgFirstName.Text.Trim());
                            checkWorkerCommand.Parameters.AddWithValue("@dolgozoLastName", txtDolgLastName.Text.Trim());
                            checkWorkerCommand.Parameters.AddWithValue("@dolgozoTel", txtDolgPhone.Text.Trim());
                            checkWorkerCommand.Parameters.AddWithValue("@dolgozoEmail", txtDolgEmail.Text.Trim());

                            var result = await checkWorkerCommand.ExecuteScalarAsync();

                            if (result != null)
                            {
                                dolgozoID = Convert.ToInt32(result);
                            }
                            else
                            {
                                string addWorkerQuery = @"INSERT INTO dolgozók (dolgozoFirstName, dolgozoLastName, dolgozoTel, dolgozoEmail, statusz, szolgáltatasa) 
                                         VALUES (@dolgozoFirstName, @dolgozoLastName, @dolgozoTel, @dolgozoEmail, @statusz, @szolgáltatasa)";

                                using (var addWorkerCommand = new MySqlCommand(addWorkerQuery, conn))
                                {
                                    addWorkerCommand.Parameters.AddWithValue("@dolgozoFirstName", txtDolgFirstName.Text.Trim());
                                    addWorkerCommand.Parameters.AddWithValue("@dolgozoLastName", txtDolgLastName.Text.Trim());
                                    addWorkerCommand.Parameters.AddWithValue("@dolgozoTel", txtDolgPhone.Text.Trim());
                                    addWorkerCommand.Parameters.AddWithValue("@dolgozoEmail", txtDolgEmail.Text.Trim());
                                    addWorkerCommand.Parameters.AddWithValue("@statusz", chkWorkerStatus.IsChecked);
                                    addWorkerCommand.Parameters.AddWithValue("@szolgáltatasa", selectedService.SzolgID);

                                    await addWorkerCommand.ExecuteNonQueryAsync();
                                }

                                using (var getWorkerIDCommand = new MySqlCommand("SELECT LAST_INSERT_ID();", conn))
                                {
                                    dolgozoID = Convert.ToInt32(await getWorkerIDCommand.ExecuteScalarAsync());
                                }
                                MessageBox.Show("Sikeres dolgozó felvétel!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                            }
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtDolgFirstName.Text) == true)
                {
                    MessageBox.Show("Hibás vagy hiányzó vezetéknév!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDolgFirstName.Clear();
                    txtDolgFirstName.Focus();
                }
                else if (string.IsNullOrEmpty(txtDolgLastName.Text) == true)
                {
                    MessageBox.Show("Hibás vagy hiányzó keresztnév!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDolgLastName.Clear();
                    txtDolgLastName.Focus();
                }
                else if (string.IsNullOrEmpty(txtDolgEmail.Text) == true || IsValidEmail(txtDolgEmail.Text) == false)
                {
                    MessageBox.Show("Hibás vagy hiányzó email cím!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDolgEmail.Clear();
                    txtDolgEmail.Focus();
                }
                else if (string.IsNullOrEmpty(txtDolgPhone.Text) == true || txtDolgPhone.Text.Count() != 11)
                {
                    MessageBox.Show("Hibás vagy hiányzó telefonszám!\nAjánlott formátum: 06301234567", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDolgPhone.Clear();
                    txtDolgPhone.Focus();
                }
                else if (cbWorkCategory.SelectedIndex == -1)
                {
                    MessageBox.Show("Szolgáltatás kiválasztás nélkül nem lehet dolgozót módosítani!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var selectedService = cbWorkCategory.SelectedItem as Szolgaltatas;

                    using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
                    {
                        await conn.OpenAsync();

                        string updateWorkerQuery = @"UPDATE dolgozók 
                                         SET dolgozoFirstName = @dolgozoFirstName, 
                                             dolgozoLastName = @dolgozoLastName, 
                                             dolgozoTel = @dolgozoTel, 
                                             dolgozoEmail = @dolgozoEmail, 
                                             statusz = @statusz, 
                                             szolgáltatasa = @szolgáltatasa 
                                         WHERE dolgozoID = @dolgozoID";

                        using (var updateWorkerCommand = new MySqlCommand(updateWorkerQuery, conn))
                        {
                            updateWorkerCommand.Parameters.AddWithValue("@dolgozoFirstName", txtDolgFirstName.Text.Trim());
                            updateWorkerCommand.Parameters.AddWithValue("@dolgozoLastName", txtDolgLastName.Text.Trim());
                            updateWorkerCommand.Parameters.AddWithValue("@dolgozoTel", txtDolgPhone.Text.Trim());
                            updateWorkerCommand.Parameters.AddWithValue("@dolgozoEmail", txtDolgEmail.Text.Trim());
                            updateWorkerCommand.Parameters.AddWithValue("@statusz", chkWorkerStatus.IsChecked);
                            updateWorkerCommand.Parameters.AddWithValue("@szolgáltatasa", selectedService.SzolgID);
                            updateWorkerCommand.Parameters.AddWithValue("@dolgozoID", editWorkerId);  

                            await updateWorkerCommand.ExecuteNonQueryAsync();
                            MessageBox.Show("Sikeres dolgozó módosítás!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        this.Close();
                    }
                }
            }
        }
    }
}
