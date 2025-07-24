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
//using System.Windows.Shapes;
using Microsoft.Win32;
using Content_Management_System.Models;
using Content_Management_System.HelpMethods;
using System.Collections.ObjectModel;
using System.IO;

namespace Content_Management_System.Pages
{

    public partial class NewSpiceWindow : Window
    {
        public ObservableCollection<Spice> spices { get; set; }
        private Data dataHelper = new Data();
        private Spice spice = new Spice();

        public NewSpiceWindow()
        {
            InitializeComponent();
            spices = dataHelper.DeSerializeObject<ObservableCollection<Spice>>("Spices.xml");

            // Popuni fontove
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);

            // Popuni boje
            cboColors.ItemsSource = typeof(Colors).GetProperties()
                .Select(c => new { Name = c.Name, Color = (Color)c.GetValue(null) });

            // Popuni veličine fonta
            cmbFontSize.ItemsSource = new double[] { 8, 10, 12, 14, 16, 18, 20, 24, 28, 32 };

            // Event za brojanje reči
            DescriptionBox.TextChanged += DescriptionBox_TextChanged;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            if (ImagePreview.Source == null || ImagePreview.Source.ToString().Contains("imagePlaceholder"))
            {
                errorLabelImage.Content = "Image is required";
                error = true;
            }

            var parsedID = 0;
            if (string.IsNullOrWhiteSpace(IdBox.Text))
            {
                errorLabelID.Content = "ID is required";
                error = true;
            }
            else if (!int.TryParse(IdBox.Text, out parsedID))
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
            if (string.IsNullOrWhiteSpace(rtfText) || rtfText.Trim() == string.Empty)
            {
                errorLabelDescription.Content = "Description is required";
                error = true;
            }

            if (error) return;

            if (spices.Any(s => s.Id == parsedID))
            {
                MessageBox.Show("Spice with this ID already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Priprema putanje za sliku (uzima ime fajla izabrane slike)
            string imageFileName = System.IO.Path.GetFileName((ImagePreview.Source as BitmapImage)?.UriSource.LocalPath);
            string relativeImagePath = "../../Resources/Images/Spices/" + imageFileName;

            // Priprema RTF putanja
            string rtfFileName = NameBox.Text.Replace(" ", "") + ".rtf";
            string relativeRtfPath = "../../Resources/rtfs/" + rtfFileName;

            // Puna putanja na disku
            string rtfFullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\rtfs", rtfFileName));

            // Snimi sadržaj opisa u RTF fajl
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(rtfFullPath));
            using (FileStream fs = new FileStream(rtfFullPath, FileMode.Create))
            {
                new TextRange(DescriptionBox.Document.ContentStart, DescriptionBox.Document.ContentEnd)
                    .Save(fs, DataFormats.Rtf);
            }

            Spice newSpice = new Spice(parsedID, NameBox.Text, relativeImagePath, relativeRtfPath, DateTime.Now);
            newSpice.Selected = false;

            // Dodavanje u kolekciju i snimanje u XML
            spices.Add(newSpice);
            dataHelper.SerializeObject(spices, "Spices.xml");

            MessageBox.Show("New spice saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true; // Zatvara prozor
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

        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            errorLabelDescription.Content = "";
            // Brojanje reči
            var text = new TextRange(DescriptionBox.Document.ContentStart, DescriptionBox.Document.ContentEnd).Text;
            var wordCount = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            txtWordCount.Text = $"Words: {wordCount}";
        }

        private void CmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
            {
                var font = (FontFamily)cmbFontFamily.SelectedItem;
                DescriptionBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, font);
            }
        }

        private void CmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontSize.SelectedItem != null)
            {
                DescriptionBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, cmbFontSize.SelectedItem);
            }
        }

        private void CboColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboColors.SelectedItem != null)
            {
                var colorProp = cboColors.SelectedItem.GetType().GetProperty("Color");
                var color = (Color)colorProp.GetValue(cboColors.SelectedItem);
                DescriptionBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            }
        }
    }
}
