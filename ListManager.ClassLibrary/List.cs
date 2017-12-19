using System;
using System.ComponentModel;
using SQLite.Net.Attributes;

namespace ListManager.ClassLibrary
{
    public class List : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public Int32 Id { get; set; }
        public String Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
