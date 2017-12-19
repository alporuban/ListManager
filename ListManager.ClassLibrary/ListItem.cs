using System;
using System.ComponentModel;
using SQLite.Net.Attributes;

namespace ListManager.ClassLibrary
{
    public class ListItem : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public Int32 Id { get; set; }
        public Int32 ListId { get; set; }
        public String Item { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string item)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(item));
            }
        }
    }
}
