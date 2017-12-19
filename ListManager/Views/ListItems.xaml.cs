using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using ListManager.Common;
using ListManager.ClassLibrary;
using Windows.Storage;

namespace ListManager.Views
{
    public sealed partial class ListItems : NavigationPage
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

        private int _CurrentListItemId;
        public int CurrentListItemId
        {
            get { return _CurrentListItemId; }
            set { _CurrentListItemId = value; }
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

        public ListItems()
        {
            this.InitializeComponent();
        }

        #endregion Constructor

        #region OnNavigatedTo

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                CurrentListId = (int)e.Parameter;
                CurrentListItemId = 0;
                Heading.Text = "(SLM) " + DatabaseHelper.GetList(CurrentListId).Name;

                BindListItemsListView();
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
            if (Item.Text != String.Empty)
            {
                switch (CurrentListItemId)
                {
                    case 0:
                        DatabaseHelper.CreateListItem(new ListItem() { ListId = CurrentListId, Item = Item.Text });
                        break;
                    default:
                        DatabaseHelper.UpdateListItem(new ListItem() { Id = CurrentListItemId, ListId = CurrentListId, Item = Item.Text });
                        AddButton.Content = "Add New Item";
                        break;
                }

                CurrentListItemId = 0;
                Item.Text = string.Empty;
                BindListItemsListView();
            }
        }

        // Used during Testing
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelper.ClearAllLists();
            BindListItemsListView();
        }

        private void ListItemsListView_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Ha‌​nd, 1);
        }

        private void ListItemsListView_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        #endregion Events

        #region Methods

        private async void BindListItemsListView()
        {
            try
            {
                List<ListItem> ListItems = DatabaseHelper.GetListItems(CurrentListId);
                DefaultViewModel["ListItems"] = ListItems;

                AddButton.Focus(FocusState.Programmatic);
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

                int ListItemId = 0;
                int ListItemListId = 0;
                string ListItemItem = string.Empty;

                MenuFlyoutItem mfi = sender as MenuFlyoutItem;

                if (mfi == null)
                {
                    return;
                }

                ListItem SelectedListItem = mfi.DataContext as ListItem;
                List<ListItem> PasteListItems = null;

                ApplicationDataCompositeValue ListItemsCutPasteComposite = new ApplicationDataCompositeValue();
                ApplicationDataCompositeValue ListItemsCopyPasteComposite = new ApplicationDataCompositeValue();

                switch (mfi.Name)
                {
                    case null:
                        break;
                    case "DeleteFlyout":
                        Navigate = true;
                        DatabaseHelper.DeleteListItem(SelectedListItem.Id);

                        // Wipe out all Paste Composites
                        LocalSettings.Values["ListItemsCutPasteComposite"] = null;
                        LocalSettings.Values["ListItemsCopyPasteComposite"] = null;
                        break;
                    case "EditFlyout":
                        CurrentListItemId = SelectedListItem.Id;
                        Item.Text = SelectedListItem.Item;
                        AddButton.Content = "Update Item";

                        // Wipe out all Paste Composites
                        LocalSettings.Values["ListItemsCutPasteComposite"] = null;
                        LocalSettings.Values["ListItemsCopyPasteComposite"] = null;
                        break;
                    case "CutFlyout":
                        Navigate = true;
                        ListItemsCutPasteComposite["ListItemId"] = SelectedListItem.Id;
                        ListItemsCutPasteComposite["ListItemListId"] = SelectedListItem.ListId;
                        ListItemsCutPasteComposite["ListItemItem"] = SelectedListItem.Item;
                        LocalSettings.Values["ListItemsCutPasteComposite"] = ListItemsCutPasteComposite;

                        // Wipe out CopyPaste Composite so as to have only one Paste Operation Active at a time
                        LocalSettings.Values["ListItemsCopyPasteComposite"] = null;
                        break;
                    case "PasteCutFlyout":
                        // Catch a Double Click to prevent a null Exception because the Composite was deleted on the first Click
                        if (LocalSettings.Values["ListItemsCutPasteComposite"] == null) { break; }

                        Navigate = true;
                        ListItemsCutPasteComposite = (ApplicationDataCompositeValue)LocalSettings.Values["ListItemsCutPasteComposite"];
                        ListItemId = (int)ListItemsCutPasteComposite["ListItemId"];
                        ListItemListId = (int)ListItemsCutPasteComposite["ListItemListId"];
                        ListItemItem = (string)ListItemsCutPasteComposite["ListItemItem"];

                        PasteListItems = DatabaseHelper.GetListItems(SelectedListItem.ListId);
                        DatabaseHelper.DeleteListItems(SelectedListItem.ListId);

                        foreach (ListItem LI in PasteListItems)
                        {
                            if (LI.Id == SelectedListItem.Id)
                            {
                                DatabaseHelper.CreateListItem(new ListItem() { ListId = SelectedListItem.ListId, Item = ListItemItem });

                                // If its the same Item don't Paste
                                if (LI.Id != ListItemId)
                                {
                                    DatabaseHelper.CreateListItem(new ListItem() { ListId = SelectedListItem.ListId, Item = LI.Item });
                                }
                            }
                            else if (LI.ListId == ListItemListId && LI.Id == ListItemId)
                            {
                                int one = 1;
                            }
                            else
                            {
                                DatabaseHelper.CreateListItem(new ListItem() { ListId = LI.ListId, Item = LI.Item });
                            }
                        }

                        // Delete the Cut Item if it was cut from a different List
                        // If it was cut from this List it was omitted in the above logic
                        if (ListItemListId != SelectedListItem.ListId)
                        {
                            DatabaseHelper.DeleteListItem(ListItemId);
                        }

                        // Wipe out all Paste Composites
                        LocalSettings.Values["ListItemsCutPasteComposite"] = null;
                        LocalSettings.Values["ListItemsCopyPasteComposite"] = null;
                        break;
                    case "CopyFlyout":
                        Navigate = true;
                        ListItemsCopyPasteComposite["ListItemId"] = SelectedListItem.Id;
                        ListItemsCopyPasteComposite["ListItemListId"] = SelectedListItem.ListId;
                        ListItemsCopyPasteComposite["ListItemItem"] = SelectedListItem.Item;
                        LocalSettings.Values["ListItemsCopyPasteComposite"] = ListItemsCopyPasteComposite;

                        // Wipe out CutPaste Composite so as to have only one Paste Operation Active at a time
                        LocalSettings.Values["ListItemsCutPasteComposite"] = null;
                        break;
                    case "PasteCopyFlyout":
                        // Catch a Double Click to prevent a null Exception because the Composite was deleted on the first Click
                        if (LocalSettings.Values["ListItemsCopyPasteComposite"] == null) { break; }

                        Navigate = true;
                        ListItemsCopyPasteComposite = (ApplicationDataCompositeValue)LocalSettings.Values["ListItemsCopyPasteComposite"];
                        ListItemId = (int)ListItemsCopyPasteComposite["ListItemId"];
                        ListItemListId = (int)ListItemsCopyPasteComposite["ListItemListId"];
                        ListItemItem = (string)ListItemsCopyPasteComposite["ListItemItem"];

                        PasteListItems = DatabaseHelper.GetListItems(SelectedListItem.ListId);
                        DatabaseHelper.DeleteListItems(SelectedListItem.ListId);

                        foreach (ListItem LI in PasteListItems)
                        {
                            if (LI.Id == SelectedListItem.Id)
                            {
                                DatabaseHelper.CreateListItem(new ListItem() { ListId = SelectedListItem.ListId, Item = ListItemItem });
                            }

                            DatabaseHelper.CreateListItem(new ListItem() { ListId = LI.ListId, Item = LI.Item });
                        }

                        // Wipe out all Paste Composites
                        LocalSettings.Values["ListItemsCutPasteComposite"] = null;
                        LocalSettings.Values["ListItemsCopyPasteComposite"] = null;
                        break;
                }

                switch (Navigate)
                {
                    case true:
                        Frame.Navigate(typeof(ListItems), CurrentListId);
                        break;
                    default:
                        BindListItemsListView();
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
