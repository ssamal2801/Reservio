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
using System.Data.Linq;

namespace Reservio
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void registerRedirect(object sender, RoutedEventArgs e)
        {
            Reservio.Views.RegistrationPage reg = new Reservio.Views.RegistrationPage();
            this.Close();
            reg.Show();
        }

        private void loginRedirect(object sender, RoutedEventArgs e)
        {
            Reservio.Views.Login loginRedirect = new Reservio.Views.Login();
            this.Close();
            loginRedirect.Show();

        }

    }

}