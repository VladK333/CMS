using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace Content_Management_System.Models
{
    [Serializable]
    public class Spice : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string RtfPath { get; set; }
        public DateTime DateTime { get; set; }

        private bool selected;
        public bool Selected
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    selected = value;
                    OnPropertyChanged(nameof(Selected));
                }
            }
        }

        public Spice() { }

        public Spice(int id, string name, string imagePath, string rtfPath, DateTime dateAdded)
        {
            Id = id;
            Name = name;
            ImagePath = imagePath;
            RtfPath = rtfPath;
            DateTime = dateAdded;
            Selected = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

