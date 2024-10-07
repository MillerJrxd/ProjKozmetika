using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjKozmetika.Classes;

namespace ProjKozmetika
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*Loaded += MainWindow_LoadedAsync;
            cbService.ItemsSource = szolgatatasok;
            cbWorker.ItemsSource = dolgozok;
            cbDate.ItemsSource = times;
            this.DataContext = this;
            cbWorker.IsEnabled = false;*/
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