using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Content_Management_System.Models;

namespace Content_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<User> Users;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void LeaveBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure?",
                "Exit confirmation",
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
                poruka += "Username is required\n";
                greska = true;
            }

            if (string.IsNullOrWhiteSpace(password.Password))
            {
                poruka += "Password is required\n";
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
                    To = 363, // fill height
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
                };

                SpiceFill.BeginAnimation(HeightProperty, fillAnimation);
            }



        }
    }
}
