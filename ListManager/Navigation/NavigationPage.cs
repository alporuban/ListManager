using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ListManager.Views.TestPages;

// AJP ToDo: Clean this Page Up!!!


namespace ListManager.Views
{
    public class NavigationPage : Page
    {
        public static NavigationPage Current;

        #region Properties

        #endregion Properties

        #region Page Methods

        public NavigationPage()
        {
            Loaded += NavigationPage_Loaded;

            // Use a CommandBar rather than an AppBar so that we get default layout
            CommandBar commandBar = new CommandBar();
            commandBar.HorizontalContentAlignment = HorizontalAlignment.Left;

            AppBarButton HomeButton;
            AppBarButton HelpButton;
            AppBarButton AboutButton;
            AppBarButton AddButton;

            AppBarSeparator Separator = new AppBarSeparator();

            if (GetType() == typeof(ListManager.Views.Lists))
            {
                AddButton = new AppBarButton();
                AddButton.Label = "Edit Lists";
                AddButton.Icon = new SymbolIcon(Symbol.Edit);
                AddButton.Click += AddButton_Click;
                commandBar.PrimaryCommands.Add(AddButton);

                HelpButton = new AppBarButton();
                HelpButton.Label = "Help";
                HelpButton.Icon = new SymbolIcon(Symbol.Help);
                HelpButton.Click += HelpButton_Click;
                commandBar.PrimaryCommands.Add(HelpButton);

                AboutButton = new AppBarButton();
                AboutButton.Label = "About";
                AboutButton.Icon = new SymbolIcon(Symbol.Important);
                AboutButton.Click += AboutButton_Click;
                commandBar.PrimaryCommands.Add(AboutButton);
            }

            if (GetType() == typeof(ListManager.Views.ListItems))
            {
                HomeButton = new AppBarButton();
                HomeButton.Label = "Home";
                HomeButton.Icon = new SymbolIcon(Symbol.Home);
                HomeButton.Click += ListsButton_Click;
                commandBar.PrimaryCommands.Add(HomeButton);

                AddButton = new AppBarButton();
                AddButton.Label = "Edit Lists";
                AddButton.Icon = new SymbolIcon(Symbol.Edit);
                AddButton.Click += AddButton_Click;
                commandBar.PrimaryCommands.Add(AddButton);

                HelpButton = new AppBarButton();
                HelpButton.Label = "Help";
                HelpButton.Icon = new SymbolIcon(Symbol.Help);
                HelpButton.Click += HelpButton_Click;
                commandBar.PrimaryCommands.Add(HelpButton);
            }

            if (GetType() == typeof(ListManager.Views.ListEdit))
            {
                HomeButton = new AppBarButton();
                HomeButton.Label = "Home";
                HomeButton.Icon = new SymbolIcon(Symbol.Home);
                HomeButton.Click += ListsButton_Click;
                commandBar.PrimaryCommands.Add(HomeButton);

                HelpButton = new AppBarButton();
                HelpButton.Label = "Help";
                HelpButton.Icon = new SymbolIcon(Symbol.Help);
                HelpButton.Click += HelpButton_Click;
                commandBar.PrimaryCommands.Add(HelpButton);
            }

            if (GetType() == typeof(ListManager.Views.Help))
            {
                AppBarButton ListsButton = new AppBarButton();
                ListsButton.Label = "Home";
                ListsButton.Icon = new SymbolIcon(Symbol.Home);
                ListsButton.Click += ListsButton_Click;
                commandBar.PrimaryCommands.Add(ListsButton);

                AddButton = new AppBarButton();
                AddButton.Label = "Edit Lists";
                AddButton.Icon = new SymbolIcon(Symbol.Edit);
                AddButton.Click += AddButton_Click;
                commandBar.PrimaryCommands.Add(AddButton);
            }

            if (GetType() == typeof(ListManager.Views.About))
            {
                AppBarButton ListsButton = new AppBarButton();
                ListsButton.Label = "Home";
                ListsButton.Icon = new SymbolIcon(Symbol.Home);
                ListsButton.Click += ListsButton_Click;
                commandBar.PrimaryCommands.Add(ListsButton);

                AddButton = new AppBarButton();
                AddButton.Label = "Edit Lists";
                AddButton.Icon = new SymbolIcon(Symbol.Edit);
                AddButton.Click += AddButton_Click;
                commandBar.PrimaryCommands.Add(AddButton);
            }

            if (GetType() == typeof(ListManager.Views.TestPages.LoadPhoneDatabase))
            {
                HomeButton = new AppBarButton();
                HomeButton.Label = "Home";
                HomeButton.Icon = new SymbolIcon(Symbol.Home);
                HomeButton.Click += ListsButton_Click;
                commandBar.PrimaryCommands.Add(HomeButton);

                AddButton = new AppBarButton();
                AddButton.Label = "Edit Lists";
                AddButton.Icon = new SymbolIcon(Symbol.Edit);
                AddButton.Click += AddButton_Click;
                commandBar.PrimaryCommands.Add(AddButton);
            }

            //HelpButton = new AppBarButton();
            //HelpButton.Label = "Help";
            //HelpButton.Icon = new SymbolIcon(Symbol.Help);
            //HelpButton.Click += HelpButton_Click;
            //commandBar.PrimaryCommands.Add(HelpButton);

            AppBarButton DatabaseButton = new AppBarButton();
            DatabaseButton.Label = "Testing";
            DatabaseButton.Icon = new SymbolIcon(Symbol.Setting);
            DatabaseButton.Click += DatabaseButton_Click;
            commandBar.PrimaryCommands.Add(DatabaseButton);

            BottomAppBar = commandBar;
            BottomAppBar.HorizontalAlignment = HorizontalAlignment.Center;
            BottomAppBar.FlowDirection = FlowDirection.LeftToRight;
        }

        void NavigationPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        }

        #endregion Page Methods

        #region BottomAppBar Commands

        #region Primary Commands

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ListEdit));
        }

        private void ListsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Lists));
        }

        private void DatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoadPhoneDatabase));
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Help));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }
        #endregion Primary Commands

        #region Secondary Commands

        #endregion Secondary Commands

        #endregion BottomAppBar Commands
    }
}
