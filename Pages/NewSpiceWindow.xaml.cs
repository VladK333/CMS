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
        private bool isEditMode = false;       
        private Spice editingSpice = null; 
        public Spice ResultSpice { get; private set; }

        // Konstruktor za dodavanje (prazna forma)
        public NewSpiceWindow()
        {
            InitializeComponent();
            InitializeWindow();

            SetDefaultTextStyle();
            Loaded += (s, e) =>
            {
                FocusManager.SetFocusedElement(this, this);
                Keyboard.ClearFocus();
            };

            DescriptionBox.TextChanged += DescriptionBox_TextChanged;
        }

        // Konstruktor za edit postojeceg zacina
        public NewSpiceWindow(Spice spiceToEdit) : this()
        {
            isEditMode = true;
            editingSpice = spiceToEdit;

            // Popuni polja podacima
            IdBox.Text = spiceToEdit.Id.ToString();
            IdBox.IsReadOnly = true;
            NameBox.Text = spiceToEdit.Name;
            ImagePreview.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(spiceToEdit.ImagePath)));

            // Učitaj opis iz RTF fajla
            string rtfFullPath = System.IO.Path.GetFullPath(spiceToEdit.RtfPath);
            if (File.Exists(rtfFullPath))
            {
                using (FileStream fs = new FileStream(rtfFullPath, FileMode.Open, FileAccess.Read))
                {
                    var range = new TextRange(DescriptionBox.Document.ContentStart, DescriptionBox.Document.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);
                }
            }
        }

        private void InitializeWindow()
        {
            spices = dataHelper.DeSerializeObject<ObservableCollection<Spice>>("Spices.xml");

            // Fontovi, boje, veličine
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cboColors.ItemsSource = typeof(Colors).GetProperties()
                .Select(c => new { Name = c.Name, Color = (Color)c.GetValue(null) });
            cmbFontSize.ItemsSource = new double[] { 8, 10, 12, 14, 16, 18, 20, 24, 28, 32 };
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            errorLabelImage.Content = errorLabelID.Content = errorLabelName.Content = errorLabelDescription.Content = "";

            if (ImagePreview.Source == null || ImagePreview.Source.ToString().Contains("imagePlaceholder"))
            {
                errorLabelImage.Content = "Image is required";
                error = true;
            }

            int parsedID = 0;
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

            // Ako je novi, provera da li postoji ID
            if (!isEditMode && spices.Any(s => s.Id == parsedID))
            {
                MessageBox.Show("Spice with this ID already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Putanje
            string imageFileName = System.IO.Path.GetFileName((ImagePreview.Source as BitmapImage)?.UriSource.LocalPath);
            string relativeImagePath = "../../Resources/Images/Spices/" + imageFileName;

            string rtfFileName = NameBox.Text.Replace(" ", "") + ".rtf";
            string relativeRtfPath = "../../Resources/rtfs/" + rtfFileName;
            string rtfFullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\rtfs", rtfFileName));

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(rtfFullPath));
            using (FileStream fs = new FileStream(rtfFullPath, FileMode.Create))
            {
                new TextRange(DescriptionBox.Document.ContentStart, DescriptionBox.Document.ContentEnd)
                    .Save(fs, DataFormats.Rtf);
            }

            if (isEditMode)
            {
                // Nađi objekat u kolekciji
                var existing = spices.FirstOrDefault(s => s.Id == editingSpice.Id);
                if (existing != null)
                {
                    existing.Id = parsedID;
                    existing.Name = NameBox.Text;
                    existing.ImagePath = relativeImagePath;
                    existing.RtfPath = relativeRtfPath;
                    existing.DateTime = DateTime.Now;
                    ResultSpice = existing;
                }
            }
            else
            {
                var newSpice = new Spice(parsedID, NameBox.Text, relativeImagePath, relativeRtfPath, DateTime.Now);
                newSpice.Selected = false;
                spices.Add(newSpice);
                ResultSpice = newSpice;
            }

            // Snimi sve u XML
            dataHelper.SerializeObject(spices, "Spices.xml");

            MessageBox.Show(isEditMode ? "Spice updated!" : "New spice saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
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

        private void IdBox_TextChanged(object sender, TextChangedEventArgs e) => errorLabelID.Content = "";
        private void NameBox_TextChanged(object sender, TextChangedEventArgs e) => errorLabelName.Content = "";
        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            errorLabelDescription.Content = "";
            var text = new TextRange(DescriptionBox.Document.ContentStart, DescriptionBox.Document.ContentEnd).Text;
            var wordCount = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            txtWordCount.Text = $"Words: {wordCount}";
        }

        private void CmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem is FontFamily font)
            {
                DescriptionBox.Focus();
                ApplyTextFormatting(TextElement.FontFamilyProperty, font);
            }
        }

        private void CmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontSize.SelectedItem is double size)
            {
                DescriptionBox.Focus();
                ApplyTextFormatting(TextElement.FontSizeProperty, size);
            }
        }

        private void CboColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboColors.SelectedItem != null)
            {
                DescriptionBox.Focus();
                var colorProp = cboColors.SelectedItem.GetType().GetProperty("Color");
                var color = (Color)colorProp.GetValue(cboColors.SelectedItem);
                ApplyTextFormatting(TextElement.ForegroundProperty, new SolidColorBrush(color));
            }
        }

        private void ApplyTextFormatting(DependencyProperty property, object value)
        {
            if (!DescriptionBox.Selection.IsEmpty)
            {
                DescriptionBox.Selection.ApplyPropertyValue(property, value);
            }
            else
            {
                DescriptionBox.Focus();
                var caret = DescriptionBox.CaretPosition;
                var emptyRange = new TextRange(caret, caret);
                emptyRange.ApplyPropertyValue(property, value);
                DescriptionBox.Selection.ApplyPropertyValue(property, value);
            }
        }

        private void SetDefaultTextStyle()
        {
            var defaultFont = new FontFamily("Courier New");
            var defaultSize = 12.0;
            var defaultBrush = Brushes.Black;

            DescriptionBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, defaultFont);
            DescriptionBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, defaultSize);
            DescriptionBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, defaultBrush);

            cmbFontFamily.SelectedItem = Fonts.SystemFontFamilies.FirstOrDefault(f => f.Source == "Courier New");
            cmbFontSize.SelectedItem = defaultSize;

            var defaultColorItem = cboColors.Items.Cast<object>()
                .FirstOrDefault(c =>
                {
                    var nameProp = c.GetType().GetProperty("Name");
                    return (string)nameProp.GetValue(c) == "Black";
                });

            cboColors.SelectedItem = defaultColorItem;
        }
    }
}
