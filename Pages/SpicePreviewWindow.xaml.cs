using System;
using System.Collections.Generic;
using System.IO;
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

namespace Content_Management_System.Pages
{
    public partial class SpicePreviewWindow : Window
    {
        public SpicePreviewWindow(Spice spice)
        {
            InitializeComponent();

            IdBox.Text = spice.Id.ToString();
            NameBox.Text = spice.Name;
            ImagePreview.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath(spice.ImagePath)));

            // data from rtf file
            string rtfFullPath = System.IO.Path.GetFullPath(spice.RtfPath);
            if (File.Exists(rtfFullPath))
            {
                using (FileStream fs = new FileStream(rtfFullPath, FileMode.Open, FileAccess.Read))
                {
                    var range = new TextRange(DescriptionBox.Document.ContentStart, DescriptionBox.Document.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);
                }
            }

            var text = new TextRange(DescriptionBox.Document.ContentStart, DescriptionBox.Document.ContentEnd).Text;
            var wordCount = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            txtWordCount.Text = $"Words: {wordCount}";
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
