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
using System.Text.RegularExpressions;

namespace Reservio.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// Author Nallagatla Swathi
    /// </summary>
    public partial class Login : Window
    {
        //Connection Path
        //static string connectionPath = @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\MadMax\Downloads\Reservio\Reservio\Reservio\reservio.mdf;Integrated Security = True";
        static string connectionPath = Reservio.Properties.Settings.Default.reservioConnectionString


        //Initialize the Data Context
        reservioDataContext myContext;

        //List of all the Registered Users
        List<Registration> allUsers;

        public Login()
        {
            InitializeComponent();

            //Create an instance of Data Context using the specified path
            myContext = new reservioDataContext(connectionPath);

            //Fetch the registered members from the DB and assign it to the list
            allUsers = myContext.Registrations.ToList();
        }

        /*
         * This method is triggered when the login button is selected
         * The fields validations are done and proper error messages are displayed
         * If the credentials are correct the user is logged in
         * Else an error message is displayed
         */
        private void loginButton(object sender, RoutedEventArgs e)
        {
            if (emailTextbox.Text.Length == 0 || passwordTextbox.Password.Length == 0)
            {
                MessageBox.Show("Please enter all the fields!");
            }
            else if (!ValidateEmail(emailTextbox.Text))
            {
                MessageBox.Show("Invalid Email!");
            }
            else if (passwordTextbox.Password.Length > 20)
            {
                MessageBox.Show("Password length is too big!");
            }
            else
            {
                string email = emailTextbox.Text;
                string password = passwordTextbox.Password.ToString();
                int loginFlag = 0;

                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (allUsers[i].Email == email && allUsers[i].Password == password)
                    {
                        //Redirect the user to Reservation page on successful login
                        Reservation reservation = new Reservation();
                        this.Hide();
                        reservation.Show();
                        loginFlag = 1;
                    }
                }

                if (loginFlag == 0)
                {
                    MessageBox.Show("You entered wrong details or you are not yet registered.");
                }
            }

        }

        /*
         * This method is triggered when the user selects the Forgot Password link
         */
        private void PasswordReset(object sender, MouseButtonEventArgs e)
        {
            //The user is redirected to the forgot password page to reset his/her password
            ForgotPassword forgotPassword = new ForgotPassword();
            this.Close();
            forgotPassword.Show();
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
