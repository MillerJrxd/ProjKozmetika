using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjKozmetika.Classes;

namespace ProjKozmetika
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public static string ConnectionString()
        {
            return "server=localhost;port=3306;uid=root;database=kozmetika";
        }
        private void btnNewReservation_Click(object sender, RoutedEventArgs e)
        {
            Reservation reservation = new Reservation();
            reservation.ShowDialog();
        }
        private void btnReservationShow_Click(object sender, RoutedEventArgs e)
        {
            ReservationDisplay reservationDisplay = new ReservationDisplay();
            reservationDisplay.ShowDialog();
        }
        private void btnNewWorker_Click(object sender, RoutedEventArgs e)
        {
            Worker worker = new Worker();
            worker.ShowDialog();
        }
    }
}