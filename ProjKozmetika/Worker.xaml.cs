using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Worker.xaml
    /// </summary>
    public partial class Worker : Window
    {
        public Worker()
        {
            InitializeComponent();
        }

        private void PreviewTextInputString(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^\p{L}]+$");
            e.Handled = regex.IsMatch(e.Text);

            foreach (char c in e.Text)
            {
                int unicodeValue = char.ConvertToUtf32(e.Text, 0);

                if ((unicodeValue >= 0x1F600 && unicodeValue <= 0x1F64F) ||   // Arcok
                  (unicodeValue >= 0x1F300 && unicodeValue <= 0x1F5FF) ||   // Piktogramok
                  (unicodeValue >= 0x1F680 && unicodeValue <= 0x1F6FF) ||   // Szállítás és térképek
                  (unicodeValue >= 0x2600 && unicodeValue <= 0x26FF) ||    // Szimbólumok
                  (unicodeValue >= 0x2700 && unicodeValue <= 0x27BF))       // Dingbatok
                {
                    e.Handled = true; // Megakadályozzuk a bevitel
                    return;
                }
            }
        }
        private void PreviewNumInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^\d]");
            e.Handled |= regex.IsMatch(e.Text);
        }
        public bool IsValidEmail(string email)
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
    }
}
