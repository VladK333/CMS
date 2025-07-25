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
    public partial class TableWindow : Window
    {
        public ObservableCollection<Spice> Spices { get; set; }
        Data dataHelper = new Data();
        private UserRole currentRole;

        public TableWindow(UserRole role)   //role needed for hyperlink 
        {
            InitializeComponent();
            currentRole = role;
            LoadSpicesData();
        }

        private void LoadSpicesData()
        {
            Spices = dataHelper.DeSerializeObject<ObservableCollection<Spice>>("Spices.xml");

            if (Spices == null)
            {
                MessageBox.Show("Error with opening the file","Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DataContext = this;
            SubscribeToSelectedChanged();
        }

        public void HideAdminButtons() //function used in MainWindow.xaml.cs
        {
            AddBtn.Visibility = Visibility.Collapsed;
            DltBtn.Visibility = Visibility.Collapsed;
            SelectAllCheckBox.Visibility = Visibility.Collapsed;
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
                var spice = sender as Spice;    //spice that had propertyChange
                if (spice != null)
                {
                    //event hanlers off, so the program wont end up in infinite loop
                    SelectAllCheckBox.Checked -= SelectAllCheckBox_Checked;
                    SelectAllCheckBox.Unchecked -= SelectAllCheckBox_Unchecked;

                    SelectAllCheckBox.IsChecked = Spices.All(s => s.Selected); //if all spices are selected manually, check SelectAll

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
                MessageBox.Show("No items selected!",
                                "Warning", 
                                 MessageBoxButton.OK, 
                                 MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete item(s)?",
                                "Delete Confirmation",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Base folder (bin/Debug/.. -> project folder)
                string projectDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."));

                foreach (var spice in spicesRemove)
                {
                    // Delete RTF file
                    if (!string.IsNullOrWhiteSpace(spice.RtfPath))
                    {
                        string rtfFullPath = System.IO.Path.Combine(projectDir,
                            spice.RtfPath.Replace("../../", "").Replace("/", System.IO.Path.DirectorySeparatorChar.ToString())); //full path to rtf file

                        if (System.IO.File.Exists(rtfFullPath))
                        {
                            try
                            {
                                System.IO.File.Delete(rtfFullPath); //delete rtf file
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to delete RTF file for {spice.Name}: {ex.Message}","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    Spices.Remove(spice);
                }

                dataHelper.SerializeObject(Spices, "Spices.xml");

                MessageBox.Show(
                    "Selected spice(s) deleted successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            SelectAllCheckBox.IsChecked = false;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            NewSpiceWindow newSpiceWindow = new NewSpiceWindow();
            bool? result = newSpiceWindow.ShowDialog();

            if (result == true)
            {
                RefreshTable();
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            var spice = (Spice)((FrameworkElement)hyperlink.Parent).DataContext;

            if (currentRole == UserRole.Admin)
            {
                var editWindow = new NewSpiceWindow(spice); // form for edit
                bool? result = editWindow.ShowDialog();
                if (result == true)
                {
                    RefreshTable();
                }
            }
            else
            {
                var previewWindow = new SpicePreviewWindow(spice); // preview form
                bool? result = previewWindow.ShowDialog();
                if (result == true)
                {
                    RefreshTable();
                }
            }
        }
        private void RefreshTable()
        {
            var newTable = new TableWindow(currentRole);
            newTable.Show();
            this.Close();
        }
    }
}
