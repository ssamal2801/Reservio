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
    /// Interaction logic for loginPage.xaml
    /// </summary>
    public partial class loginPage : Window
    {

        static string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gokul\source\repos\Reservio\Reservio\reservio.mdf;Integrated Security = True";
        reservioDataContext myContext;
        List<Registration> allUsers;

        public loginPage()
        {
            InitializeComponent();
            myContext = new reservioDataContext(connectionString);
            allUsers = myContext.Registrations.ToList();
        }

        private void loginButton(object sender, RoutedEventArgs e)
        {
            string email = emailTextbox.Text;
            string password = passwordTextbox.Password.ToString();
            int loginFlag = 0;

            for (int i=0; i<allUsers.Count; i++)
            {
                if (allUsers[i].Email == email && allUsers[i].Password == password)
                {
                    MessageBox.Show("User Logged in!");
                    loginFlag = 1;
                }
            }

            if (loginFlag == 0)
            {
                MessageBox.Show("You entered wrong details or you are not yet registered.");
            }

        }
    }
}
