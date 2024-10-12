using MySqlConnector;
using ProjKozmetika.Classes;
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

namespace ProjKozmetika
{
    /// <summary>
    /// Interaction logic for ServiceDisplay.xaml
    /// </summary>
    public partial class ServiceDisplay : Window
    {
        MySqlConnection conn;
        ObservableCollection<DisplayService> services = new ObservableCollection<DisplayService>();

        public ServiceDisplay()
        {
            InitializeComponent();
            dgServices.ItemsSource = services;
            this.DataContext = this;
            Loaded += ServiceDisplay_LoadedAsync;
        }

        private async void ServiceDisplay_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await LoadServicesAsync();
        }

        private async Task<Task> LoadServicesAsync()
        {
            using (var conn = new MySqlConnection(MainWindow.ConnectionString()))
            {
                await conn.OpenAsync();

                string query = @"SELECT szolgaltatasID, szolgaltatasKategoria, szolgaltatasIdotartam,
                                    szolgaltatasAr FROM szolgáltatás";

                using (var command = new MySqlCommand(query, conn))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            services.Add(new DisplayService
                            {
                                SzolgáltatásID = reader.GetByte(0),
                                SzolgáltatásKategória = reader.GetString(1),
                                SzolgáltatásIdeje = reader.GetTimeSpan(2),
                                SzolgáltatásÁra = reader.GetInt32(3),
                            });
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        private async void btnModifyServices_Click(object sender, RoutedEventArgs e)
        {
            if (dgServices.SelectedItem is DisplayService selectedService)
            {
                Service serviceAblak = new Service(true, selectedService.SzolgáltatásID);

                serviceAblak.txtServiceName.Text = selectedService.SzolgáltatásKategória;
                serviceAblak.txtServicePrice.Text = selectedService.SzolgáltatásÁra.ToString();
                serviceAblak.cbServiceTime.SelectedItem = selectedService.SzolgáltatásIdeje;

                serviceAblak.ShowDialog();
                services.Clear();
                await LoadServicesAsync();
            }
            else
            {
                MessageBox.Show("Kérlek, válassz ki egy szolgáltatást a módosításhoz.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
