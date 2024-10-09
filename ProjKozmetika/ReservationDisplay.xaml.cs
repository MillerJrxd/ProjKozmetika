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
        ObservableCollection<Foglalas> reservation = new();
        

        public ReservationDisplay()
        {
            InitializeComponent();
        }
        private async Task<Task> DisplayDataAsync()
        {

            throw   new NotImplementedException();
            try
            {
                conn = new MySqlConnection(MainWindow.ConnectionString());
                await conn.OpenAsync();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
