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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Content_Management_System.Models;
using Content_Management_System.HelpMethods;
using System.ComponentModel;

namespace Content_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Table.xaml
    /// </summary>
    public partial class TableWindow : Window

    {
        public ObservableCollection<Spice> Spices { get; set; }
        Data dataHelper = new Data();

        public TableWindow()
        {
            InitializeComponent();
            LoadSpicesData();
        }

        private void LoadSpicesData()
        {

            Spices = dataHelper.DeSerializeObject<ObservableCollection<Spice>>("Spices.xml");

            if (Spices == null)
            {
                MessageBox.Show("Error with opening the file");
            }

            DataContext = this;
            SubscribeToSelectedChanged();

        }

        public void HideAdminButtons()
        {
            AddBtn.Visibility = Visibility.Collapsed;
            DltBtn.Visibility = Visibility.Collapsed;
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Logout confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Spices != null)
            {
                foreach (var spice in Spices)
                {
                    spice.Selected = true;
                }
                TableGrid.Items.Refresh();
            }
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Spices != null)
            {
                foreach (var spice in Spices)
                {
                    spice.Selected = false;
                }
                TableGrid.Items.Refresh();
            }
        }

        private void SubscribeToSelectedChanged()
        {
            if (Spices == null) return;

            foreach (var spice in Spices)
            {
                spice.PropertyChanged += Spice_PropertyChanged;
            }
        }

        private void Spice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Spice.Selected))
            {
                var spice = sender as Spice;
                if (spice != null)
                {
                    SelectAllCheckBox.Checked -= SelectAllCheckBox_Checked;
                    SelectAllCheckBox.Unchecked -= SelectAllCheckBox_Unchecked;

                    if (Spices.All(s => s.Selected))
                    {
                        //if all are checked, check SelectAll
                        SelectAllCheckBox.IsChecked = true;
                    }
                    else if (Spices.Any(s => !s.Selected))
                    {
                        // if at least one is unchecked, uncheck selectAll
                        SelectAllCheckBox.IsChecked = false;
                    }

                    SelectAllCheckBox.Checked += SelectAllCheckBox_Checked;
                    SelectAllCheckBox.Unchecked += SelectAllCheckBox_Unchecked;
                }
            }
        }

        private void DltBtn_Click(object sender, RoutedEventArgs e)
        {
            var spicesRemove = Spices.Where(s => s.Selected).ToList();

            if (spicesRemove.Count == 0)
            {
                MessageBox.Show("No items selected!");
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete item(s)?",
                                "Delete Confirmation",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var spice in spicesRemove)
                {
                    Spices.Remove(spice);
                }
                TableGrid.Items.Refresh();
            }
            SelectAllCheckBox.IsChecked = false;
        }
    }
}
