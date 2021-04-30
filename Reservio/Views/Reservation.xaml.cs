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
using System.Windows.Forms;
using System.Data.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Reservio.Views
{
    /// <summary>
    /// Interaction logic for Reservation.xaml
    /// Author Gokul Ramesh Babu
    /// </summary>
    public partial class Reservation : Window
    {
        //Create an instance of Data Context
        reservioDataContext dbContext;

        //Initialize a list to store all the reservations
        List<Booking> bookings;

        //Create a list to store the filtered reservations
        List<Booking> filteredBookings = new List<Booking>();

        //Set rowIndex
        int rowIndex = 0;

        //Connection Path
        //static string connectionPath = @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\MadMax\Downloads\Reservio\Reservio\Reservio\reservio.mdf;Integrated Security = True";
        static string connectionPath = Reservio.Properties.Settings.Default.reservioConnectionString;


        public Reservation()
        {
            InitializeComponent();

            //Create an instance of Data Context using the specified path
            dbContext = new reservioDataContext(connectionPath);

            //Assign the data from DB to the list
            bookings = dbContext.Bookings.ToList();

            //Prevent past date selection
            SearchDate.DisplayDateStart = DateTime.Now;
            SearchDate.DisplayDateEnd = DateTime.Now.AddDays(7);
        }

        /* 
         * This method is triggered when the user clicks the bell icon
         * A reminder email is sent to all the customer who have made a reservation on the next day
         */
        private void SendReminder(object sender, RoutedEventArgs e)
        {
            //Confirmation dialog
            DialogResult code = System.Windows.Forms.MessageBox.Show("Would You like to Send a reminder to all the reservation?", "Some Title", MessageBoxButtons.YesNo);
            if (code == System.Windows.Forms.DialogResult.Yes)
            {
                //Get all the reservations
                FetchReservations();
                System.Windows.MessageBox.Show("Sent all reminder emails");
            }
        }

        //This method logs out the user and redirects to the splash page
        private void Logout(object sender, RoutedEventArgs e)
        {
            MainWindow landingPage = new MainWindow();
            this.Close();
            landingPage.Show();
        }

        //This method is used to redirect the user to View All Reservations Page
        private void ViewAllReservations(object sender, RoutedEventArgs e)
        {
            ViewReservations viewReservations = new ViewReservations();
            this.Close();
            viewReservations.Show();
        }

        /* 
         * This method is triggered when the user clicks the Check button
         * This method calls the fetchTable method
         */
        private void SearchTable(object sender, RoutedEventArgs e)
        {
            if (SearchDate.Text.Length == 0 || Time.Text.Length == 0)
            {
                System.Windows.MessageBox.Show("Please enter all the fields!");
            }
            else if (SearchDate.Text.Length > 10)
            {
                System.Windows.MessageBox.Show("Incorrect Date!");
            }
            else
            {
                rowIndex = 0;
                filteredBookings.Clear();
                string date = SearchDate.Text;
                string time = Time.Text;
                fetchTable(date, time);
            }
        }

        /* 
         * This method is triggered when the user saves the reservation
         * Error validations are handled properly
         * The reservation data is saved successfully if there are no errors
         */
        private void SaveBooking(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomerName.Text.Length == 0 || Email.Text.Length == 0 || Count.Text.Length == 0 || Phone.Text.Length == 0)
                {
                    System.Windows.MessageBox.Show("Please enter all the fields!");
                }
                else if (!ValidateEmail(Email.Text))
                {
                    System.Windows.MessageBox.Show("Invalid Email!");
                }
                else if(Phone.Text.Length != 10)
                {
                    System.Windows.MessageBox.Show("Phone Number should have 10 digits!");
                }
                else if (int.Parse(Count.Text) > 9)
                {
                    System.Windows.MessageBox.Show("Max count is 9!");
                }
                else
                {
                    //save data to DB
                    bookings[rowIndex].CustomerName = CustomerName.Text;
                    bookings[rowIndex].EmailID = Email.Text;
                    bookings[rowIndex].Count = Count.Text;
                    bookings[rowIndex].PhoneNumber = Phone.Text;
                    dbContext.SubmitChanges();
                    System.Windows.MessageBox.Show("Reservation Successful!");

                    //Send Confirmation Email after successful reservation
                    SendConfirmationEmail();
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Something went wrong!"); ;
            }
        }

        //This method is triggered when the previous button is clicked. It displays the previous record.
        private void FetchPreviousTable(object sender, RoutedEventArgs e)
        {
            //If rowIndex is not 0 then previous record is displayed
            if (rowIndex > 0)
            {
                rowIndex -= 1;
                DisplayBooking();
            }
            else
            {
                System.Windows.MessageBox.Show("This is the 1st Table!");
            }
        }

        //This method is triggered when the next button is clicked. It displays the next record.
        private void FetchNextTable(object sender, RoutedEventArgs e)
        {
            //If rowIndex is not the last item then next record is displayed
            if (rowIndex < filteredBookings.Count - 1)
            {
                rowIndex += 1;
                DisplayBooking();
            }
            else
            {
                System.Windows.MessageBox.Show("This is the last Table!");
            }
        }

        //This method is used to hide the search results area
        private void CancelBooking(object sender, RoutedEventArgs e)
        {
            TableResults.Visibility = Visibility.Hidden;
            NoResults.Visibility = Visibility.Visible;
        }

        //This method is triggered when there is a mouse down action
        private void SpecialActions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /*
         * This method fetches the records as per the date and time and displays them
         */
        private void fetchTable(string date, string time)
        {

            bool flag = false;

            //iterate all the bookings
            foreach (var booking in bookings)
            {
                //compare the records with the input
                if (String.Compare(booking.BookingDate.ToString(), date) == 0 && String.Compare(booking.BookingTime, time) == 0)
                {
                    //Add the record to the filteredList
                    filteredBookings.Add(booking);
                    flag = true;
                }
            }
            if (flag == false)
            {
                //Display appropriate message if there are no search results
                System.Windows.MessageBox.Show("No Tables available for the selected date!");
                TableResults.Visibility = Visibility.Hidden;
                NoResults.Visibility = Visibility.Visible;
            }
            else
            {
                //display the records if there are search results
                TableResults.Visibility = Visibility.Visible;
                NoResults.Visibility = Visibility.Hidden;
                DisplayBooking();
                flag = false;
            }
        }

        //Display the data in the UI
        private void DisplayBooking()
        {
            //Set the data to the input fields
            TableNumber.Text = filteredBookings[rowIndex].TableNumber;
            CustomerName.Text = filteredBookings[rowIndex].CustomerName;
            Email.Text = filteredBookings[rowIndex].EmailID;
            Phone.Text = filteredBookings[rowIndex].PhoneNumber;
            Count.Text = filteredBookings[rowIndex].Count;
        }

        //Fetch all the rservations on the next day and send a reminder email
        private void FetchReservations()
        {
            string date = DateTime.Now.AddDays(1).ToString("M/d/yyyy");
            string message;
            List<Booking> reservations = dbContext.Bookings.ToList().Where(booking => String.Compare(booking.BookingDate, date) == 0).ToList();
            foreach(var reservation in reservations)
            {
                message = "Hi Mr/Ms. " + reservation.CustomerName + ", this is a reminder for your upcoming reservation on " + reservation.BookingDate + " (" + reservation.BookingTime + ").";
                SendEmail(reservation.EmailID, "Reminder: Upcoming Reservation", message, false, "");
            }
        }

        //Send a confirmation email after successfull reservation
        private void SendConfirmationEmail()
        {
            string to = Email.Text;  
            string message = "Your Reservation on " + filteredBookings[rowIndex].BookingDate + " (" + filteredBookings[rowIndex].BookingTime + ") is confirmed.";

            SendEmail(to, "Booking Confirmation", message, true, "Reservation Confirmation has been sent to the registered Email Id.");
        }

        //This method is used to send an email based on the input paramaters provided
        private void SendEmail(string to, string subject, string message, bool sendConfirmation, string confirmationMessage)
        {
            MailMessage myMessage = new MailMessage();

            //account details
            string from = "email.test.user.smtp@gmail.com";
            string pwd = "JohnDoe@123";

            //add the input to the body
            myMessage.To.Add(to);
            myMessage.From = new MailAddress(from);
            myMessage.Body = message;
            myMessage.Subject = subject;
            myMessage.IsBodyHtml = true;

            //Email configurations
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(from, pwd);

            try
            {
                //Send Email
                smtp.Send(myMessage);
                if (sendConfirmation == true)
                {
                    System.Windows.MessageBox.Show(confirmationMessage);
                }

            }
            catch (Exception exp)
            {
                System.Windows.MessageBox.Show(exp.Message);
            }
        }

        /* 
         * This method is user for validating the email.
         * It send true if the email is valid.
         * Else it sends false
         * */
        private bool ValidateEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
                return true;
            else
                return false;
        }

    }
}
