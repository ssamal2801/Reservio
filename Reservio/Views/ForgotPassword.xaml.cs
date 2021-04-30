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
using System.Data.Linq;
using System.Net;
using System.Net.Mail;

namespace Reservio.Views
{
    /// <summary>
    /// Interaction logic for ForgotPassword.xaml
    /// Author Vikaramjeet Singh
    /// </summary>
    public partial class ForgotPassword : Window
    {
        //Connection Path
        //static string connectionPath = @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\MadMax\Downloads\Reservio\Reservio\Reservio\reservio.mdf;Integrated Security = True";
        static string connectionPath = Reservio.Properties.Settings.Default.reservioConnectionString;


        //Initialize the Data Context
        reservioDataContext dataContext;

        public ForgotPassword()
        {
            InitializeComponent();

            //Create an instance of Data Context using the specified path
            dataContext = new reservioDataContext(connectionPath);
        }

        /*
         * This method is Triggered when the Reset button is clicked
         * If the EmailID matches then the password is reset and the user is redirected to the login page
         * Else a message is displayed
         */
        private void ResetPassword(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Email.Text.Length == 0 || Password.Password.Length == 0)
                {
                    MessageBox.Show("Please enter all the fields!");
                }
                else if (Email.Text.Length > 50)
                {
                    MessageBox.Show("Email length is too big!");
                }
                else if (Password.Password.Length > 20)
                {
                    MessageBox.Show("Password length is too big!");
                }
                else
                {
                    //Filter the registration table data
                    List<Registration> registrations = dataContext.Registrations.ToList().Where(registration => String.Compare(registration.Email, Email.Text) == 0).ToList(); ;

                    //If record exists
                    if (registrations.Count == 1)
                    {
                        //Update password
                        registrations[0].Password = Password.Password;
                        dataContext.SubmitChanges();
                        MessageBox.Show("Your Password has been successfully reset!");

                        //Redirect to Login
                        Login login = new Login();
                        this.Close();
                        login.Show();
                    }
                    else
                    {
                        MessageBox.Show("Email ID not found! If you are a new user please Register!");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fill in the details correctly!");
            }
        }

        /*
         * This method is triggered when the cancel button is clicked
         * The user is navigated to the login page
         */
        private void CancelReset(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            this.Close();
            login.Show();
        }

    }
}
