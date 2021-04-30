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

namespace Reservio.Screens
{
    /// <summary>
    /// Interaction logic for registrationPage.xaml
    /// </summary>
    public partial class registrationPage : Window
    {

        static string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gokul\source\repos\Reservio\Reservio\reservio.mdf;Integrated Security = True";
        reservioDataContext dc;

        public registrationPage()
        {
            InitializeComponent();
            dc = new reservioDataContext(connectionString);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ///instance creation of tables
                Registration registration = new Registration();
                registration.Name = Name.Text;
                registration.Contact = Contact.Text;
                registration.Address = Address.Text;
                registration.Email = Email.Text;
                registration.Password = Password.Text;
                ///Inserting data to table
                dc.Registrations.InsertOnSubmit(registration);
                ///Changes are saved
                dc.SubmitChanges();
                MessageBox.Show("Welcome to Reservio! Click OK to login.");

                loginPage login = new loginPage();
                this.Close();
                login.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fill in the details correctly");
            }
        }

    }

}
