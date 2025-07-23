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
using Microsoft.Win32;

namespace Content_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for NewSpiceWindow.xaml
    /// </summary>
    public partial class NewSpiceWindow : Window
    {
        public NewSpiceWindow()
        {
            InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            if (ImagePreview.Source == null || ImagePreview.Source.ToString().Contains("imagePlaceholder"))
            {
                errorLabelImage.Content="Image is required";
                error=true;
            }

            if (string.IsNullOrWhiteSpace(IdBox.Text))
            {
                errorLabelID.Content = "ID is required";
                error = true;
            }
            else if (!int.TryParse(IdBox.Text, out _))
            {
                errorLabelID.Content = "ID must be a number";
                error = true;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                errorLabelName.Content = "Name is required";
                error = true;
            }

            string rtfText = new TextRange(DescriptionBox.Document.ContentStart, DescriptionBox.Document.ContentEnd).Text;
            if (string.IsNullOrWhiteSpace(rtfText) || rtfText.Trim() == "\r\n")
            {
                MessageBox.Show("Description cannot be empty!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                error = true;
            }

            if (!error)
            {
                DialogResult = true;  // Ovo zatvara formu i vraća rezultat
            }
        }

        private void ImageBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select a spice image",
                InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\Images\Spices")),
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                ImagePreview.Source = new BitmapImage(new Uri(dialog.FileName));
                errorLabelImage.Content = "";
            }

        }

        private void IdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            errorLabelID.Content = "";
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            errorLabelName.Content = "";
        }
    }
}
