using System.Collections.ObjectModel;
using System.Windows;
using MySqlConnector;
using ProjKozmetika.Classes;

namespace ProjKozmetika
{
    /// <summary>
    /// Interaction logic for ReservationDisplay.xaml
    /// </summary>
    public partial class WorkerDisplay : Window
    {
        ObservableCollection<DisplayWorker> workers = new ObservableCollection<DisplayWorker>();
        MySqlConnection conn;

        public WorkerDisplay()
        {
            InitializeComponent();
            Loaded += WorkerDisplay_LoadedAsync;
            dgWorkers.ItemsSource = workers;
            this.DataContext = this;
        }
        private async void WorkerDisplay_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await LoadWorkersAsync();
        }
        private async Task<Task> LoadWorkersAsync()
        {
            using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
            {
                await conn.OpenAsync();

                string query = @"SELECT dolgozoID, CONCAT(dolgozoFirstName, ' ', dolgozoLastName) AS DolgozoNev, 
                                 dolgozoTel, dolgozoEmail, statusz AS DolgozoStatusz, szolgaltatasKategoria AS DolgozoSzolgaltatasa, szolgáltatasa 
                                 FROM Dolgozók JOIN Szolgáltatás ON szolgáltatasa = szolgaltatasID;";


                using (var command = new MySqlCommand(query, conn))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            workers.Add(new DisplayWorker
                            {
                                DolgozoID = reader.GetByte(0),
                                DolgozoNev = reader.GetString(1),
                                DolgozoTel = reader.GetString(2),
                                DolgozoEmail = reader.GetString(3),
                                DolgozoStatusz = reader.GetBoolean(4),
                                DolgozoSzolgaltatasa = reader.GetString(5),
                                DolgozoSzolgaltatasID = reader.GetByte(6)
                            });
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        private async void btnEditWorker_Click(object sender, RoutedEventArgs e)
        {
            if (dgWorkers.SelectedItem is DisplayWorker selectedWorker)
            {
                //Dolgozo dolgozoData = new Dolgozo();
                // Új dolgozó ablak inicializálása
                Worker workerAblak = new Worker(true, selectedWorker.DolgozoID); 
                
                workerAblak.txtDolgFirstName.Text = selectedWorker.DolgozoNev.Split(' ')[0];
                workerAblak.txtDolgLastName.Text = selectedWorker.DolgozoNev.Split(' ')[1];
                workerAblak.txtDolgPhone.Text = selectedWorker.DolgozoTel;
                workerAblak.txtDolgEmail.Text = selectedWorker.DolgozoEmail;
                workerAblak.chkWorkerStatus.IsChecked = selectedWorker.DolgozoStatusz;
                workerAblak.cbWorkCategory.SelectedIndex = selectedWorker.DolgozoSzolgaltatasID - 1;


                workerAblak.ShowDialog();
                workers.Clear();
                await LoadWorkersAsync();
            }
            else
            {
                MessageBox.Show("Kérlek, válassz ki egy dolgozót a módosításhoz.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
