using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ListManager.ClassLibrary;
using ListManager.Views;

namespace ListManager
{
    sealed partial class App : Application
    {
        #region Properties & Variables

        // AJP ToDo: Put this in App.xaml
        public const string APPLICATION_NAME = "RTAccounts";

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Determine Device and Margins
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsMobile
        {
            get
            {
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                return (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile");
            }
        }

        public bool IsDesktop
        {
            get
            {
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                return (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Desktop");
            }
        }

        // Widths
        public double WindowWidth { get; set; }
        public double ItemWidth { get; set; }

        // Margins
        public Thickness PageMargins { get; set; }
        public Thickness ItemMargins { get; set; }

        // Alignments
        public HorizontalAlignment PageAlignment { get; set; }

        // For ScrollingIntoView when returning to Lists Page
        public int CurrentListId { get; set; }

        // For ScrollingIntoView when returning to ListItemss Page
        public int CurrentListItemId { get; set; }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Determine Device and set Margins
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #endregion Properties & Variables

        #region Constructor

        public App()
        {
            this.InitializeComponent();
            Suspending += OnSuspending;
        }

        #endregion Constructor

        #region OnLaunched

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////    
            // Set ConnectionString
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            string DatabaseName = "ListManager.slmx";
            DatabaseHelper.ConnectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, DatabaseName);

            // Use LocalFolder because Database is larger than 100k and WON'T roam!
            var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(DatabaseName);
            if (file == null)
            {
                await DatabaseHelper.CreateDatabase();
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // If ListManager.slmx is in PicturesLibrary overlay the Database with the one in PicturesLibrary
            // This is to make it easy to update the DB - Quick way to get a full Database
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Get the Pictures Folder
            StorageFolder PicturesFolder = KnownFolders.PicturesLibrary;

            // Get the Local Folder
            StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

            // Get the DB File
            StorageFile DatabaseFile = null;
            try
            {
                // Will abend if File is not there
                DatabaseFile = await PicturesFolder.GetFileAsync(DatabaseName);

                // Copy DB File to Local Folder
                await DatabaseFile.CopyAsync(LocalFolder, DatabaseName, NameCollisionOption.ReplaceExisting);

                // Delete SQLiteDBFile from Pictures Library
                await DatabaseFile.DeleteAsync();
            }
            catch (Exception ex)
            {
                // Ok - Don't do anything if RTAccounts.db doesn't exist in PicturesLibrary
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Calculate Page Widths and Margins
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (IsMobile)
            {
                WindowWidth = (double)Window.Current.Bounds.Width - 32;
                ItemWidth = WindowWidth - 32;

                PageMargins = new Thickness(0, 0, 0, 0);
                ItemMargins = new Thickness(4, 0, 4, 0);

                PageAlignment = HorizontalAlignment.Center;
            }
            else if (IsDesktop)
            {
                WindowWidth = (double)Window.Current.Bounds.Width - 64;
                ItemWidth = 320;

                PageMargins = new Thickness(8, 0, 0, 0);
                ItemMargins = new Thickness(8, 2, 8, 0);

                PageAlignment = HorizontalAlignment.Center;
            }

            Frame RootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();

                RootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = RootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (RootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    RootFrame.Navigate(typeof(Lists), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        #endregion OnLaunched

        #region Events

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        #endregion Events
    }
}
