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
using System.Windows.Shapes;

namespace Reservio.Views
{
    /// <summary>
    /// Interaction logic for ViewReservations.xaml
    /// Author Swagat Samal
    /// </summary>
    public partial class ViewReservations : Window
    {
        //Connection Path
        //static string connectionPath = @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\MadMax\Downloads\Reservio\Reservio\Reservio\reservio.mdf;Integrated Security = True";
        static string connectionPath = Reservio.Properties.Settings.Default.reservioConnectionString;



        //Create an instance of Data Context
        reservioDataContext dataContext;

        //Create a list to store all the reservations
        List<Booking> reservations;
        public ViewReservations()
        {
            InitializeComponent();

            //Create an instance of Data Context using the specified path
            dataContext = new reservioDataContext(connectionPath);

            //Assign the data from DB to the list
            reservations = dataContext.Bookings.ToList();

            //Display the Reservations in the datagrid
            DisplayReservations(reservations);
        }

        /*
         * This method is used for displaying the reservations in the data grid
         */
        private void DisplayReservations(List<Booking> bookings)
        {
            try
            {
                //Assign the data to the Data grid
                ReservationsGrid.ItemsSource = bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong! " + ex);
            }
        }

        /*
         * This method is triggered when the there is a change in the From date
         * This is done so that the ToDate will either be the same date as From or a future date
         */
        private void AdjustToDate(object sender, SelectionChangedEventArgs e)
        {
            DateTime fromDate = DateTime.Parse(FromDate.Text);
            ToDate.SelectedDate = fromDate;
            ToDate.DisplayDateStart = fromDate;
        }

        /*
         * The user is redirected to the reservation page
         */
        private void GoBack(object sender, RoutedEventArgs e)
        {
            Reservation reservation = new Reservation();
            this.Close();
            reservation.Show();
        }

        //The search filters are applied and the data grid displays the updated data
        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            if (FromDate.Text.Length == 0 || ToDate.Text.Length == 0)
            {
                MessageBox.Show("Please enter all the fields!");
            }
            else if (FromDate.Text.Length > 10)
            {
                MessageBox.Show("Incorrect From Date!");
            }
            else if (ToDate.Text.Length > 10)
            {
                MessageBox.Show("Incorrect To Date!");
            }
            else 
            {
                bool filterFlag = false;
                DateTime fromDate = DateTime.Parse(FromDate.Text);
                DateTime toDate = DateTime.Parse(ToDate.Text);

                //List to store the filtered results
                List<Booking> filteredBookings = new List<Booking>();

                //Loop through all the reservations
                foreach (var reservation in reservations)
                {
                    //Find dates between From and To Date
                    if (fromDate <= DateTime.Parse(reservation.BookingDate) && DateTime.Parse(reservation.BookingDate) <= toDate)
                    {
                        //Add the item to the list
                        filteredBookings.Add(reservation);
                        filterFlag = true;
                    }
                }
                if (filterFlag)
                {
                    //Display the updated results in the data grid
                    DisplayReservations(filteredBookings);
                }
                else
                {
                    MessageBox.Show("No Reservations are made between the selected dates!");
                }
            }
        }

        //This method logs out the user and redirects to the splash page
        private void Logout(object sender, RoutedEventArgs e)
        {
            MainWindow landingPage = new MainWindow();
            this.Close();
            landingPage.Show();
        }

    }
}
