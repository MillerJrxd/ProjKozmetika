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
using ProjKozmetika.Classes;

namespace ProjKozmetika
{
    /// <summary>
    /// Interaction logic for ReservationDisplay.xaml
    /// </summary>
    public partial class WorkerDisplay : Window
    {
        ObservableCollection<Dolgozo> reservation = new();

        public WorkerDisplay()
        {
            InitializeComponent();
        }

        /*private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 2)
            {
                for (int i = 2; i < dataGridView1.SelectedRows.Count; i++)
                {
                    dataGridView1.SelectedRows[i].Selected = false;

                }
            }
        }*/
    }
}
