using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reservio.Screens
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Window
    {
        reservioDataContext dbContext;

        List<Booking> bookings;
        List<Booking> filteredBookings = new List<Booking>();
        int rowIndex = 0;

        static string connectionPath = @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gokul\source\repos\Reservio\Reservio\reservio.mdf;Integrated Security = True";

        public HomePage()
        {
            InitializeComponent();



            dbContext = new reservioDataContext(connectionPath);
            bookings = dbContext.Bookings.ToList();



            //Prevent past date selection
            SearchDate.DisplayDateStart = DateTime.Now;
            SearchDate.DisplayDateEnd = DateTime.Now.AddDays(7);
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SearchTable(object sender, RoutedEventArgs e)
        {
            filteredBookings.Clear();
            TableResults.Visibility = Visibility.Visible;
            NoResults.Visibility = Visibility.Hidden;
            string date = SearchDate.Text;
            string time = Time.Text;
            fetchTable(date, time);
        }

        private void SaveBooking(object sender, RoutedEventArgs e)
        {
            try
            {
                dbContext.ReserveTables(TableNumber.Text, SearchDate.Text, Time.Text, CustomerName.Text, Email.Text, Phone.Text, Count.Text);
                MessageBox.Show("Reservation Successful!");
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong!"); ;
            }
        }

        private void FetchPreviousTable(object sender, RoutedEventArgs e)
        {
            if (rowIndex > 0)
            {
                rowIndex -= 1;
                DisplayBooking();
            }
            else
            {
                MessageBox.Show("This is the 1st Table!");
            }
        }

        private void FetchNextTable(object sender, RoutedEventArgs e)
        {
            if (rowIndex < filteredBookings.Count - 1)
            {
                rowIndex += 1;
                DisplayBooking();
            }
            else
            {
                MessageBox.Show("This is the last Table!");
            }
        }

        private void CancelBooking(object sender, RoutedEventArgs e)
        {
            TableResults.Visibility = Visibility.Hidden;
            NoResults.Visibility = Visibility.Visible;
        }

        private void SpecialActions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void fetchTable(string date, string time)
        {

            bool flag = false;
            foreach (var booking in bookings)
            {
                if (String.Compare(booking.BookingDate.ToString(), date) == 0 && String.Compare(booking.BookingTime, time) == 0)
                {
                    filteredBookings.Add(booking);
                    flag = true;
                }
            }
            if (flag == false)
            {
                MessageBox.Show("No Tables available for the selected date!");
            }
            else
            {
                DisplayBooking();
                flag = false;
            }
        }

        private void DisplayBooking()
        {
            TableNumber.Text = filteredBookings[rowIndex].TableNumber;
            CustomerName.Text = filteredBookings[rowIndex].CustomerName;
            Email.Text = filteredBookings[rowIndex].PhoneNumber;
            Phone.Text = filteredBookings[rowIndex].EmailID;
            Count.Text = filteredBookings[rowIndex].Count;
        }

    }

}
