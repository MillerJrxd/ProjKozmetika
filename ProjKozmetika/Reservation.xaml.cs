﻿using MySqlConnector;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Reservation : Window
    {
        MySqlConnection conn;
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
            dpDate.SelectedDate = DateTime.Now;
        }
        private async void Reservation_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await GetServicesAsync();
        }
        private async Task<Task> GetServicesAsync()
        {
            try
            {
                conn = new MySqlConnection(MainWindow.ConnectionString());
                await conn.OpenAsync();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            string query = @"SELECT DISTINCT Szolgáltatás.szolgaltatasID, Szolgáltatás.szolgaltatasKategoria, 
                            Szolgáltatás.szolgaltatasIdotartam, Szolgáltatás.szolgaltatasAr
                     FROM Szolgáltatás
                     JOIN Dolgozók ON Dolgozók.szolgáltatasa = Szolgáltatás.szolgaltatasID
                     WHERE Dolgozók.statusz = 1"; 

            var command = new MySqlCommand(query, conn);
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                szolgatatasok.Add(new Szolgaltatas(
                    reader.GetByte(0),   
                    reader.GetString(1), 
                    reader.GetTimeSpan(2),
                    reader.GetInt32(3))); 
            }
            await conn.CloseAsync();
            return Task.CompletedTask;
        }
        private async Task<Task> GetWorkersAsync(byte ID)
        {
            try
            {
                conn = new MySqlConnection(MainWindow.ConnectionString());
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
        private async void btnUsrSubmit_Click(object sender, RoutedEventArgs e)
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
            else if (string.IsNullOrEmpty(txtUsrEmail.Text) == true || IsValidEmail(txtUsrEmail.Text) == false)
            {
                MessageBox.Show("Hibás vagy hiányzó email cím!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsrEmail.Clear();
                txtUsrEmail.Focus();
            }
            else if (string.IsNullOrEmpty(txtUsrPhone.Text) == true || txtUsrPhone.Text.Count() != 11)
            {
                MessageBox.Show("Hibás vagy hiányzó telefonszám!\nAjánlott formátum: 06301234567", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsrPhone.Clear();
                txtUsrPhone.Focus();
            }
            else if (cbService.SelectedIndex == -1)
            {
                MessageBox.Show("Szolgáltatás kiválasztás nélkül nem lehet időpontot foglalni!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (cbWorker.SelectedIndex == -1)
            {
                MessageBox.Show("Dolgozó választása nélkül nem lehet időpontot foglalni!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (cbDate.SelectedIndex == -1)
            {
                MessageBox.Show("Időpont választása nélkül nem lehet időpontot foglalni!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (dpDate.SelectedDate + (TimeSpan)cbDate.SelectedItem < DateTime.UtcNow.ToLocalTime())
            {
                MessageBox.Show("Nem lehet múltbeli időpontra foglalni!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var selectedWorker = cbWorker.SelectedItem as Dolgozo;
                var selectedService = cbService.SelectedItem as Szolgaltatas;


                using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
                {
                    await conn.OpenAsync();

                    string checkCustomerQuery = @"SELECT ugyfelID FROM Ügyfél 
                                   WHERE ugyfelFirstName = @ugyfelFirstName 
                                   AND ugyfelLastName = @ugyfelLastName 
                                   AND ugyfelTel = @ugyfelTel 
                                   AND ugyfelEmail = @ugyfelEmail";

                    int ugyfelID;

                    using (var checkCustomerCommand = new MySqlCommand(checkCustomerQuery, conn))
                    {
                        checkCustomerCommand.Parameters.AddWithValue("@ugyfelFirstName", txtUsrFirstName.Text.Trim());
                        checkCustomerCommand.Parameters.AddWithValue("@ugyfelLastName", txtUsrLastName.Text.Trim());
                        checkCustomerCommand.Parameters.AddWithValue("@ugyfelTel", txtUsrPhone.Text.Trim());
                        checkCustomerCommand.Parameters.AddWithValue("@ugyfelEmail", txtUsrEmail.Text.Trim());

                        var result = await checkCustomerCommand.ExecuteScalarAsync();

                        if (result != null)
                        {
                            ugyfelID = Convert.ToInt32(result);
                        }
                        else
                        {
                            string addCustomerQuery = @"INSERT INTO Ügyfél (ugyfelFirstName, ugyfelLastName, ugyfelTel, ugyfelEmail, ugyfelPontok) 
                                         VALUES (@ugyfelFirstName, @ugyfelLastName, @ugyfelTel, @ugyfelEmail, @ugyfelPontok)";

                            using (var addCustomerCommand = new MySqlCommand(addCustomerQuery, conn))
                            {
                                addCustomerCommand.Parameters.AddWithValue("@ugyfelFirstName", txtUsrFirstName.Text.Trim());
                                addCustomerCommand.Parameters.AddWithValue("@ugyfelLastName", txtUsrLastName.Text.Trim());
                                addCustomerCommand.Parameters.AddWithValue("@ugyfelTel", txtUsrPhone.Text.Trim());
                                addCustomerCommand.Parameters.AddWithValue("@ugyfelEmail", txtUsrEmail.Text.Trim());
                                addCustomerCommand.Parameters.AddWithValue("@ugyfelPontok", GenerateRandomPoints());

                                await addCustomerCommand.ExecuteNonQueryAsync();
                            }

                            using (var getCustomerIDCommand = new MySqlCommand("SELECT LAST_INSERT_ID();", conn))
                            {
                                ugyfelID = Convert.ToInt32(await getCustomerIDCommand.ExecuteScalarAsync());
                            }
                        }
                    }
                    TimeSpan foglalasStart = (TimeSpan)cbDate.SelectedItem;
                    TimeSpan foglalasEnd = foglalasStart.Add(selectedService.SzolgaltatasIdeje);

                    string addReservationQuery = @"INSERT INTO Foglalás (szolgaltatasID, dolgozoID, ugyfelID, foglalasStart, foglalasEnd) 
                                   VALUES (@szolgaltatasID, @dolgozoID, @ugyfelID, @foglalasStart, @foglalasEnd)";

                    using (var addReservationCommand = new MySqlCommand(addReservationQuery, conn))
                    {
                        addReservationCommand.Parameters.AddWithValue("@szolgaltatasID", selectedService.SzolgID);
                        addReservationCommand.Parameters.AddWithValue("@dolgozoID", selectedWorker.DolgozoId);
                        addReservationCommand.Parameters.AddWithValue("@ugyfelID", ugyfelID); 
                        addReservationCommand.Parameters.AddWithValue("@foglalasStart", cbDate.SelectedItem);
                        addReservationCommand.Parameters.AddWithValue("@foglalasEnd", foglalasEnd);

                        await addReservationCommand.ExecuteNonQueryAsync();
                    }
                }
                MessageBox.Show("Sikeres foglalás!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
        private async void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpDate.SelectedDate != null && cbService.SelectedItem != null && cbWorker.SelectedItem != null)
            {
                var selectedWorker = cbWorker.SelectedItem as Dolgozo;
                var selectedService = cbService.SelectedItem as Szolgaltatas;
                DateTime selectedDate = (DateTime)dpDate.SelectedDate;

                await FilterAvailableTimesAsync(selectedWorker, selectedService, selectedDate);
            }
        }
        private async void cbWorker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbWorker.SelectedItem != null && cbService.SelectedItem != null)
            {
                var selectedWorker = cbWorker.SelectedItem as Dolgozo;
                var selectedService = cbService.SelectedItem as Szolgaltatas;
                DateTime selectedDate = (DateTime)dpDate.SelectedDate;

                await FilterAvailableTimesAsync(selectedWorker, selectedService, selectedDate);
            }
        }
        private void txtUsrPhone_MouseEnter(object sender, MouseEventArgs e)
        {
            ToolTip tooltip = new ToolTip();
            tooltip.Content = "Elvárt telefonszám formátum: '06301234567'.";
            txtUsrPhone.ToolTip = tooltip;

            ToolTipService.SetInitialShowDelay(txtUsrPhone, 0);
            ToolTipService.SetShowDuration(txtUsrPhone, 2500); 
            ToolTipService.SetToolTip(txtUsrPhone, tooltip);
        }
        private void txtUsrPhone_MouseLeave(object sender, MouseEventArgs e)
        {
            txtUsrPhone.ToolTip = null;
        }
        private void PreviewTextInputString(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^\p{L}]+$");
            e.Handled = regex.IsMatch(e.Text);

            foreach (char c in e.Text)
            {
                int unicodeValue = char.ConvertToUtf32(e.Text, 0);

                if ((unicodeValue >= 0x1F600 && unicodeValue <= 0x1F64F) ||   
                  (unicodeValue >= 0x1F300 && unicodeValue <= 0x1F5FF) ||   
                  (unicodeValue >= 0x1F680 && unicodeValue <= 0x1F6FF) ||  
                  (unicodeValue >= 0x2600 && unicodeValue <= 0x26FF) ||    
                  (unicodeValue >= 0x2700 && unicodeValue <= 0x27BF))     
                {
                    e.Handled = true;
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
        private async Task FilterAvailableTimesAsync(Dolgozo selectedWorker, Szolgaltatas selectedService, DateTime selectedDate)
        {
            TimeSpan openingTime = new TimeSpan(8, 0, 0);
            TimeSpan closingTime = new TimeSpan(17, 0, 0); 

            using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
            {
                await conn.OpenAsync();

                string query = @"SELECT TIME(foglalasStart) AS foglalasStart, TIME(foglalasEnd) AS foglalasEnd 
                         FROM Foglalás 
                         WHERE dolgozoID = @dolgozoID 
                         AND DATE(foglalasStart) = @selectedDate";

                using (var command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@dolgozoID", selectedWorker.DolgozoId);
                    command.Parameters.AddWithValue("@selectedDate", selectedDate.Date);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        List<Tuple<TimeSpan, TimeSpan>> bookings = new List<Tuple<TimeSpan, TimeSpan>>();

                        while (await reader.ReadAsync())
                        {
                            TimeSpan start = reader.GetTimeSpan("foglalasStart");
                            TimeSpan end = reader.GetTimeSpan("foglalasEnd");
                            bookings.Add(new Tuple<TimeSpan, TimeSpan>(start, end));
                        }

                        TimeSpan interval = new TimeSpan(0, 15, 0);
                        times.Clear();

                        for (TimeSpan time = openingTime; time + selectedService.SzolgaltatasIdeje <= closingTime; time += interval)
                        {
                            bool isAvailable = true;

                            foreach (var booking in bookings)
                            {
                                TimeSpan bookingStart = booking.Item1;
                                TimeSpan bookingEnd = booking.Item2;

                                if (time < bookingEnd && (time + selectedService.SzolgaltatasIdeje) > bookingStart)
                                {
                                    isAvailable = false;
                                    break;
                                }
                            }
                            if (isAvailable)
                            {
                                times.Add(time);
                            }
                        }
                    }
                }
            }
        }
        private int GenerateRandomPoints()
        {
            Random rnd = new Random();

            return (rnd.Next(50, 501) / 10) * 10;
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