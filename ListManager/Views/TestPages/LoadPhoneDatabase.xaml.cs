using System;
using System.IO;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.Storage;
using ListManager.ClassLibrary;
using ListManager.Common;

namespace ListManager.Views.TestPages
{
    public sealed partial class LoadPhoneDatabase : NavigationPage
    {
        public LoadPhoneDatabase()
        {
            this.InitializeComponent();

            SystemFamily.Text = "System Family: " + Info.SystemFamily;
            SystemVersion.Text = "System Version: " + Info.SystemVersion;
            SystemArchitecture.Text = "System Architecture: " + Info.SystemArchitecture;
            ApplicationName.Text = "Application Name: " + Info.ApplicationName;
            ApplicationVersion.Text = "Application Version: " + Info.ApplicationVersion;
            DeviceManufacturer.Text = "Device Manufacturer: " + Info.DeviceManufacturer;
            DeviceModel.Text = "Device Model: " + Info.DeviceModel;
        }

        private void LoadDatabase_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelper.LoadPhoneDatabase();
        }

        private void ClearDatabase_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelper.ClearAllLists();
        }

        private async void CopyDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////    
                // Set ConnectionString
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                string DatabaseName = "ListManager.slmx";
                DatabaseHelper.ConnectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, DatabaseName);

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Backup the Database to Pictures Library for emergencies
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Get the Pictures Folder
                StorageFolder PicturesFolder = KnownFolders.PicturesLibrary;

                // Get the Local Folder
                StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

                // Get the DB File
                StorageFile DatabaseFile = null;

                // Get the Database File
                DatabaseFile = await LocalFolder.GetFileAsync(DatabaseName);

                // Copy DB File to Pictures Folder
                await DatabaseFile.CopyAsync(PicturesFolder, DatabaseName, NameCollisionOption.ReplaceExisting);
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message, "Critical Error!");
                await md.ShowAsync();
            }
        }

        private async void RestoreDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////    
                // Set ConnectionString
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                string DatabaseName = "ListManager.slmx";
                DatabaseHelper.ConnectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, DatabaseName);

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Get Database File from Pictures Library - For Testing Only!!!
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Get the Pictures Folder
                StorageFolder PicturesFolder = KnownFolders.PicturesLibrary;

                // Get the Local Folder
                StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

                // Get the DB File
                StorageFile DatabaseFile = null;

                // Get the Database File from the Pictures Library
                DatabaseFile = await PicturesFolder.GetFileAsync(DatabaseName);

                // Copy DB File to Pictures Folder
                await DatabaseFile.CopyAsync(LocalFolder, DatabaseName, NameCollisionOption.ReplaceExisting);
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message, "Critical Error!");
                await md.ShowAsync();
            }
        }

        private void InitializeEmulatorDatabase_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelper.LoadPhoneDatabase();
        }
    }
}
