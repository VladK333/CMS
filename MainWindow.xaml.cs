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
using Content_Management_System.HelpMethods;
using Content_Management_System.HelpMethods.Users;
using Content_Management_System.Models;
using Content_Management_System.Pages;

namespace Content_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserAuth _userAuth;
        private ObservableCollection<User> Users;


        public MainWindow()
        {
            InitializeComponent();
            _userAuth = new UserAuth();
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
                bool validUser = _userAuth.UserAuthentication(name.Text, password.Password);

                if (validUser)
                {
                    errorLabel.Content = "";
                    DoubleAnimation fillAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 363, // fill height
                        Duration = new Duration(TimeSpan.FromSeconds(1)),
                        EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
                    };

                    fillAnimation.Completed += (s, ev) =>
                    {
                        TableWindow tableWindow = new TableWindow();
                        tableWindow.Show();
                        this.Close();
                    };

                    SpiceFill.BeginAnimation(HeightProperty, fillAnimation);
                    
                }
                else
                {
                    errorLabel.Content = "User doesn't exist";
                }
            }
        }
    }
}
