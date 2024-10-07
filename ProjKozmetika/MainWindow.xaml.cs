using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjKozmetika
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //MySqlConnection conn;
        private readonly string connectionString = "server=localhost;port=3306;uid=root;database=kozmetika";
        ObservableCollection<Szolgaltatas> szolgatatasok = new ObservableCollection<Szolgaltatas>();
        ObservableCollection<Dolgozo> dolgozok = new ObservableCollection<Dolgozo>();
        ObservableCollection<TimeSpan> times = new ObservableCollection<TimeSpan>();

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

        }
        /*
private async void MainWindow_LoadedAsync(object sender, RoutedEventArgs e)
{
   //await GetServicesAsync();
   //await GetWorkersAsync();
}
private async Task GetServicesAsync()
{
   try
   {
       conn = new MySqlConnection(connectionString);
       await conn.OpenAsync();
   }
   catch (MySqlException ex)
   {
       MessageBox.Show(ex.Message, "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
   }

   string query = "SELECT szolgaltatasID, szolgaltatasKategoria, szolgaltatasIdotartam, szolgaltatasAr FROM Szolgáltatás";

   var command = new MySqlCommand(query, conn);
   var reader = await command.ExecuteReaderAsync();

   while (await reader.ReadAsync())
   {
       szolgatatasok.Add(new Szolgaltatas(reader.GetByte(0), reader.GetString(1), reader.GetTimeSpan(2), reader.GetInt32(3)));
   }
   await conn.CloseAsync();
}
private async Task GetWorkersAsync(byte ID)
{
   try
   {
       conn = new MySqlConnection(connectionString);
       await conn.OpenAsync();
   }
   catch (MySqlException ex)
   {
       MessageBox.Show(ex.Message, "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
   }

   string query = $"SELECT dolgozoID, dolgozoFirstName, dolgozoLastName, dolgozoTel, dolgozoEmail, statusz, szolgáltatasa FROM dolgozók WHERE szolgáltatasa = {ID} ";

   var command = new MySqlCommand(query, conn);
   var reader = await command.ExecuteReaderAsync();

   while (await reader.ReadAsync())
   {
       if (reader.GetBoolean(5) == false)
       {
           continue;
       }
       else dolgozok.Add(new Dolgozo(reader.GetByte(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetBoolean(5), reader.GetByte(6)));
   }
   await conn.CloseAsync();
}

/// <summary>
/// !!NEM HASZNÁLT KÓD!! 
/// Ellenőrzi, hogy a kiválaszott szolgáltatáshoz a megfelelő dolgozó van-e kiválasztva,
/// </summary>
private void IsCorrectWorker()
{
   Dolgozo chosenWorker = (Dolgozo)cbWorker.SelectedItem;
   Szolgaltatas chosenService = (Szolgaltatas)cbService.SelectedItem;

   if (chosenWorker == null || chosenService == null)
   {
       return;
   }

   var worker = dolgozok.ToList().Find(x => x.DolgozoFullName == $"{chosenWorker.DolgLastName} {chosenWorker.DolgFirstName}");
   var service = szolgatatasok.ToList().Find(x => x.SzolgKategoria == chosenService.SzolgKategoria);

   if (worker == null || service == null)
   {
       MessageBox.Show("A kiválasztott dolgozó vagy szolgáltatás nem található.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
       return;
   }

   if (worker.Szolgaltatas != service.SzolgID)
   {
       MessageBox.Show("Figyelem! Az általad válaszott dolgozó nem biztosítja a válaszott szolgáltatás!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
       cbWorker.SelectedItem = null;
       cbService.SelectedItem = null;
   }
}
private async void cbService_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
   cbWorker.IsEnabled = true;
   Szolgaltatas selectedItem = (Szolgaltatas)cbService.SelectedItem;
   dolgozok.Clear();
   await GetWorkersAsync(selectedItem.SzolgID);
   times.Clear();
   await TimeFillAsync(selectedItem.SzolgaltatasIdeje);
   cbWorker.SelectedIndex = 0;
}
private void PreviewTextRegex(object sender, TextCompositionEventArgs e)
{
   Regex regex = new Regex(@"[^a-zA-Z\S\P{IsBasicLatin}[(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff]]]+");
   e.Handled = regex.IsMatch(e.Text);
}
private void PreviewAppCommandsExecuted(object sender, ExecutedRoutedEventArgs e)
{
   if (e.Command == ApplicationCommands.Copy ||
   e.Command == ApplicationCommands.Cut ||
   e.Command == ApplicationCommands.Paste)
   {
       e.Handled = true;
   }
}
private void btnSubmit_Click(object sender, RoutedEventArgs e)
{
   if (string.IsNullOrEmpty(txtUsrFirstName.Text) == true)
   {
       MessageBox.Show("Hibás vagy hiányzó vezetéknév!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
       txtUsrFirstName.Clear();
       txtUsrFirstName.Focus();
   }
   else if (string.IsNullOrEmpty(txtUsrLastName.Text) == true)
   {
       MessageBox.Show("Hibás vagy hiányzó keresztnév!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
       txtUsrLastName.Clear();
       txtUsrLastName.Focus();
   }
   else if (string.IsNullOrEmpty(txtUsrEmail.Text) == true && !txtUsrEmail.Text.Contains('@'))
   {
       MessageBox.Show("Hibás vagy hiányzó email cím!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
       txtUsrEmail.Clear();
       txtUsrEmail.Focus();
   }
   else if (string.IsNullOrEmpty(txtUsrPhone.Text) == true && txtUsrPhone.Text.Count() != 11)
   {
       MessageBox.Show("Hibás vagy hiányzó telefonszám!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
       txtUsrPhone.Clear();
       txtUsrPhone.Focus();
   }
   else
   {

       //?ujablak = new Ujablak(ObservableCollection<Ugyfel> Ugyfel, ObservableCollection<Foglalas> foglalas) 
   }
}
private async Task TimeFillAsync(TimeSpan serviceTime)
{
   TimeSpan openingTime = new TimeSpan(8, 0, 0);  // 08:00
   TimeSpan closingTime = new TimeSpan(17, 0, 0); // 17:00


   // Iterálás a nyitási időponttól a zárási időpontig a szolgáltatás időtartamával
   for (TimeSpan currentTime = openingTime; currentTime.Add(serviceTime) <= closingTime; currentTime = currentTime.Add(serviceTime))
   {
       times.Add(currentTime);
   }
}

private void txtUsrFirstName_PreviewTextInput(object sender, TextCompositionEventArgs e)
{
   //Regex regex = new Regex(@"[^\p{L}[\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff]]]+$");
   //e.Handled = regex.IsMatch(e.Text);

   foreach (char c in e.Text)
   {
       // Ellenőrizzük a karakter Unicode értékét
       int unicodeValue = char.ConvertToUtf32(e.Text, 0);

       // Tiltjuk az emojik és más nem kívánt karakterek tartományát
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
}*/
    }
}