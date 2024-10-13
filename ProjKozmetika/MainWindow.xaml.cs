using System.Windows;

namespace ProjKozmetika
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            Worker worker = new Worker(false);
            worker.ShowDialog();
        }
        private void btnWorkerEdit_Click(object sender, RoutedEventArgs e)
        {
            WorkerDisplay workerDisplay = new WorkerDisplay();
            workerDisplay.ShowDialog();
        }
        private void btnServiceEdit_Click(object sender, RoutedEventArgs e)
        {
            ServiceDisplay serviceDisplay = new ServiceDisplay();
            serviceDisplay.ShowDialog();
        }
        private void btnNewService_Click(object sender, RoutedEventArgs e)
        {
            Service service = new Service(false);
            service.ShowDialog();
        }
        public static string ConnectionString()
        {
            return "server=localhost;port=3306;uid=root;database=kozmetika";
        }
    }
}