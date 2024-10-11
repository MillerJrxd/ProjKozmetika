using System.Collections.ObjectModel;
using System.Windows;
using MySqlConnector;
using ProjKozmetika.Classes;

namespace ProjKozmetika
{
    /// <summary>
    /// Interaction logic for ReservationDisplay.xaml
    /// </summary>
    public partial class ReservationDisplay : Window
    {
        MySqlConnection conn;
        ObservableCollection<DisplayReservation> reservation = new ObservableCollection<DisplayReservation>();


        public ReservationDisplay()
        {
            InitializeComponent();
            dgReservations.ItemsSource = reservation;
            this.DataContext = this;
            Loaded +=ReservationDisplayAsync_Loaded;

        }
        private async void ReservationDisplayAsync_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadReservationsAsync();
        }

        private async Task<Task> LoadReservationsAsync()
        {
            using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
            {
                await conn.OpenAsync();

                string query = @"SELECT 
                            Foglalás.foglalasID,
                            CONCAT(Ügyfél.ugyfelFirstName, ' ', Ügyfél.ugyfelLastName) AS UgyfelNev,
                            Szolgáltatás.szolgaltatasKategoria,
                            CONCAT(Dolgozók.dolgozoFirstName, ' ', Dolgozók.dolgozoLastName) AS DolgozoNev,
                            Foglalás.foglalasStart
                         FROM Foglalás
                         JOIN Ügyfél ON Foglalás.ugyfelID = Ügyfél.ugyfelID
                         JOIN Szolgáltatás ON Foglalás.szolgaltatasID = Szolgáltatás.szolgaltatasID
                         JOIN Dolgozók ON Foglalás.dolgozoID = Dolgozók.dolgozoID";

                using (var command = new MySqlCommand(query, conn))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reservation.Add(new DisplayReservation
                            {
                                FoglalasID = reader.GetInt32(0),
                                UgyfelNev = reader.GetString(1),
                                SzolgaltatasNev = reader.GetString(2),
                                DolgozoNev = reader.GetString(3),
                                FoglalasStart = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        private async void btnDeleteReservation_ClickAsync(object sender, RoutedEventArgs e)
        {
            var selectedReservation = (DisplayReservation)dgReservations.SelectedItem;

            if (dgReservations.SelectedItem != null)
            {
                if (dgReservations.SelectedItems.Count == 1)
                {
                    using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
                    {
                        await conn.OpenAsync();
                        for (int i = dgReservations.SelectedItems.Count - 1; i >= 0; i--)
                        {
                            DisplayReservation reservation = (DisplayReservation)dgReservations.SelectedItem;
                            string query = @"DELETE FROM foglalás WHERE foglalasID = @foglalasID";
                            using (var command = new MySqlCommand(query, conn))
                            {
                                command.Parameters.AddWithValue("@foglalasID", selectedReservation.FoglalasID);

                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        reservation.Remove(selectedReservation);
                    }
                    await LoadReservationsAsync();
                }
                else MessageBox.Show("Nem jelölhet ki egynél több foglalást!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else MessageBox.Show("A törléshez jelöljön ki egy foglalást!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
