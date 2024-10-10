using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
                                FoglalasStart = reader.GetTimeSpan(4)
                            });
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
