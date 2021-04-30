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
    /// Interaction logic for Registration.xaml
    /// Author Himanshu
    /// </summary>
    public partial class RegistrationPage : Window
    {
        //Connection Path
        //static string connectionPath = @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\MadMax\Downloads\Reservio\Reservio\Reservio\reservio.mdf;Integrated Security = True";
        static string connectionPath = Reservio.Properties.Settings.Default.reservioConnectionString;


        //Data Context Initialization
        reservioDataContext dc;

        List<Registration> registrations;

        public RegistrationPage()
        {
            InitializeComponent();

            //Create an instance of Data Context using the specified path
            dc = new reservioDataContext(connectionPath);

            registrations = dc.Registrations.ToList();
        }

        /*
         * This method is triggered when the user selects the register button
         * Proper error validations are handled
         * If the user is not already present then the user is added else his contents are updated
         */
        private void MakeRegistration(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Name.Text.Length == 0 || Contact.Text.Length == 0 || Address.Text.Length == 0 || Email.Text.Length == 0 || Password.Password.Length == 0)
                {
                    System.Windows.MessageBox.Show("Please enter all the fields!");
                }
                else if (Name.Text.Length > 30)
                {
                    System.Windows.MessageBox.Show("Name is too big!");
                }
                else if (Contact.Text.Length != 10)
                {
                    System.Windows.MessageBox.Show("Number should be 10 digits!");
                }
                else if (Address.Text.Length > 50)
                {
                    System.Windows.MessageBox.Show("Address is too big!");
                }
                else if (!ValidateEmail(Email.Text))
                {
                    System.Windows.MessageBox.Show("Incorrect Email Address!");
                }
                else if (Password.Password.Length > 20)
                {
                    System.Windows.MessageBox.Show("Password is too big. Only 20 characters is allowed!");
                }
                else
                {
                    //Search if the account is already registered
                    List<Registration> filteredList = new List<Registration>();
                    foreach (var item in registrations)
                    {
                        if(String.Compare(item.Email, Email.Text) == 0)
                        {
                            //Add account to the fileteredList
                            filteredList.Add(item);
                        }
                    }
                    
                    if (filteredList.Count != 0)
                    {
                        //Prompty the confirmation dialog
                        DialogResult code = System.Windows.Forms.MessageBox.Show("A account with this email already exists.", "Would you like to update the account with the new information?", MessageBoxButtons.YesNo);
                        if (code == System.Windows.Forms.DialogResult.Yes)
                        {
                            //Update the account
                            filteredList[0].Name = Name.Text;
                            filteredList[0].Contact = Contact.Text;
                            filteredList[0].Address = Address.Text;
                            filteredList[0].Password = Password.Password;

                            ///Changes are saved
                            dc.SubmitChanges();
                            System.Windows.MessageBox.Show("Account Updated!");
                        }
                    }
                    else
                    {
                        ///instance creation of Registration table
                        Registration registration = new Registration();
                        registration.Name = Name.Text;
                        registration.Contact = Contact.Text;
                        registration.Address = Address.Text;
                        registration.Email = Email.Text;
                        registration.Password = Password.Password;

                        ///Inserting data to DB
                        dc.Registrations.InsertOnSubmit(registration);

                        ///Changes are saved
                        dc.SubmitChanges();

                        System.Windows.MessageBox.Show("Welcome to Reservio! Click OK to login.");

                        //Redirecting to Login on successful registartion
                        Login login = new Login();
                        this.Close();
                        login.Show();
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Fill in the details correctly!");
            }
        }

        /*
         * This method is triggered when the user selects the Cancel button
         * It Redirects the user back to the splash page
         */
        private void CancelRegistration(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
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
