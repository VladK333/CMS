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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Content_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LeaveBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Da li ste sigurni da želite da izađete?",
                "Potvrda izlaska",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool greska = false;
            string poruka = "";

            if (string.IsNullOrWhiteSpace(name.Text))
            {
                poruka += "Niste uneli korisničko ime\n";
                greska = true;
            }

            if (string.IsNullOrWhiteSpace(password.Password))
            {
                poruka += "Niste uneli lozinku\n";
                greska = true;
            }

            if (greska)
            {
                errorLabel.Content = poruka;
            }
            else
            {
                errorLabel.Content = "";
                DoubleAnimation fillAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 363, // Visina punjenja u pikselima
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
                };

                SpiceFill.BeginAnimation(HeightProperty, fillAnimation);
            }



        }
    }
}
