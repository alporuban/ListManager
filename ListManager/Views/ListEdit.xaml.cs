using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using ListManager.Common;
using ListManager.ClassLibrary;

namespace ListManager.Views
{
    public sealed partial class ListEdit : NavigationPage
    {
        #region Properties & Variables

        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        private int _CurrentListId;
        public int CurrentListId
        {
            get { return _CurrentListId; }
            set { _CurrentListId = value; }
        }

        private ApplicationDataContainer _LocalSettings;
        public ApplicationDataContainer LocalSettings
        {
            get
            {
                if (_LocalSettings == null)
                {
                    ApplicationData AppData = Windows.Storage.ApplicationData.Current;
                    _LocalSettings = AppData.LocalSettings;
                }
                return _LocalSettings;
            }
        }

        #endregion Properties & Variables

        #region Constructor

        public ListEdit()
        {
            InitializeComponent();

            ListName.Focus(FocusState.Programmatic);
        }

        #endregion Constructor

        #region OnNavigatedTo

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                CurrentListId = 0;

                BindListsListView();
                ListName.Focus(FocusState.Programmatic);
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message, "Critical Error!");
                await md.ShowAsync();
            }
        }

        #endregion OnNavigatedTo

        #region Events

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListName.Text != String.Empty)
            {
                switch (CurrentListId)
                {
                    case 0:
                        DatabaseHelper.CreateList(new List() { Name = ListName.Text });
                        break;
                    default:
                        DatabaseHelper.UpdateList(new List() { Id = CurrentListId, Name = ListName.Text });
                        AddButton.Content = "Add New List";
                        break;
                }

                CurrentListId = 0;
                ListName.Text = string.Empty;
                ListName.Focus(FocusState.Programmatic);
                BindListsListView();
            }
        }

        // Used during Testing
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelper.ClearAllLists();
            BindListsListView();
            ListName.Focus(FocusState.Programmatic);
        }

        private void ListsListView_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Ha‌​nd, 1);
        }

        private void ListsListView_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        #endregion Events

        #region Methods

        private async void BindListsListView()
        {
            try
            {
                List<List> Lists = DatabaseHelper.GetLists();
                DefaultViewModel["Lists"] = Lists;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message, "Critical Error!");
                await md.ShowAsync();
            }
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Used to determine whether to Bind the ListView or Navigate back to this Screen
                bool Navigate = false;
                int ListId = 0;
                string CopiedListName = string.Empty;

                MenuFlyoutItem mfi = sender as MenuFlyoutItem;
                List SelectedList = mfi.DataContext as List;

                ApplicationDataCompositeValue ListEditCopyNameComposite = new ApplicationDataCompositeValue();
                ApplicationDataCompositeValue ListEditCopyItemsComposite = new ApplicationDataCompositeValue();

                switch (mfi.Name)
                {
                    case "DeleteFlyout":
                        Navigate = true;
                        DatabaseHelper.DeleteList(SelectedList.Id);
                        break;
                    case "EditFlyout":
                        CurrentListId = SelectedList.Id;
                        ListName.Text = SelectedList.Name;
                        AddButton.Content = "Update List";
                        break;
                    case "CopyNameFlyout":
                        Navigate = true;
                        ListEditCopyNameComposite["ListId"] = SelectedList.Id;
                        ListEditCopyNameComposite["ListName"] = SelectedList.Name;
                        LocalSettings.Values["ListEditCopyNameComposite"] = ListEditCopyNameComposite;
                        break;
                    case "PasteNameFlyout":
                        Navigate = true;
                        ListEditCopyNameComposite = (ApplicationDataCompositeValue)LocalSettings.Values["ListEditCopyNameComposite"];
                        ListId = (int)ListEditCopyNameComposite["ListId"];
                        CopiedListName = (string)ListEditCopyNameComposite["ListName"];
                        DatabaseHelper.UpdateList(new List() { Id = SelectedList.Id, Name = CopiedListName });
                        LocalSettings.Values["ListEditCopyNameComposite"] = null;
                        break;
                    case "CopyItemsFlyout":
                        Navigate = true;
                        ListEditCopyItemsComposite["ListId"] = SelectedList.Id;
                        LocalSettings.Values["ListEditCopyItemsComposite"] = ListEditCopyItemsComposite;
                        break;
                    case "SwitchNameFlyout":
                        // If you switch Names you have to switch ListItems also
                        Navigate = true;
                        ListEditCopyNameComposite = (ApplicationDataCompositeValue)LocalSettings.Values["ListEditCopyNameComposite"];
                        ListId = (int)ListEditCopyNameComposite["ListId"];
                        CopiedListName = (string)ListEditCopyNameComposite["ListName"];

                        List<ListItem> SelectedListItems = DatabaseHelper.GetListItems(SelectedList.Id);
                        List<ListItem> ListItems = DatabaseHelper.GetListItems(ListId);

                        DatabaseHelper.DeleteListItems(SelectedList.Id);
                        DatabaseHelper.DeleteListItems(ListId);

                        foreach (ListItem LI in SelectedListItems)
                        {
                            DatabaseHelper.CreateListItem(new ListItem() { ListId = ListId, Item = LI.Item });
                        }

                        foreach (ListItem LI in ListItems)
                        {
                            DatabaseHelper.CreateListItem(new ListItem() { ListId = SelectedList.Id, Item = LI.Item });
                        }

                        Navigate = true;
                        ListEditCopyNameComposite = (ApplicationDataCompositeValue)LocalSettings.Values["ListEditCopyNameComposite"];
                        ListId = (int)ListEditCopyNameComposite["ListId"];
                        CopiedListName = (string)ListEditCopyNameComposite["ListName"];
                        DatabaseHelper.UpdateList(new List() { Id = SelectedList.Id, Name = CopiedListName });
                        DatabaseHelper.UpdateList(new List() { Id = ListId, Name = SelectedList.Name });
                        LocalSettings.Values["ListEditCopyNameComposite"] = null;
                        break;
                    case "PasteItemsFlyout":
                        Navigate = true;
                        ListEditCopyItemsComposite = (ApplicationDataCompositeValue)LocalSettings.Values["ListEditCopyItemsComposite"];
                        ListId = (int)ListEditCopyItemsComposite["ListId"];

                        List<ListItem> ListItemsToCopy = DatabaseHelper.GetListItems(ListId);
                        foreach (ListItem LI in ListItemsToCopy)
                        {
                            DatabaseHelper.CreateListItem(new ListItem() { ListId = SelectedList.Id, Item = LI.Item });
                        }

                        LocalSettings.Values["ListEditCopyItemsComposite"] = null;
                        break;
                    case "DeleteItemsFlyout":
                        DatabaseHelper.DeleteListItems(SelectedList.Id);
                        break;
                }

                switch (Navigate)
                {
                    case true:
                        Frame.Navigate(typeof(ListEdit), CurrentListId);
                        break;
                    default:
                        BindListsListView();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message, "Critical Error!");
                await md.ShowAsync();
            }
        }

        #endregion Methods
    }
}
