using System;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using ListManager.Common;
using ListManager.ClassLibrary;

namespace ListManager.Views
{
    public sealed partial class Lists : NavigationPage
    {
        #region Properties & Variables

        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        // Stored in App.xaml.cs so it's accessible across Pages
        public int CurrentListId
        {
            get { return ((App)Application.Current).CurrentListId; }
            set { ((App)Application.Current).CurrentListId = value; }
        }

        private List _CurrentList;
        public List CurrentList
        {
            set { _CurrentList = value; }
            get { return _CurrentList; }
        }

        List<List> _Lists = null;

        #endregion Properties & Variables

        #region Constructor

        public Lists()
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
                BindListsGridView();

                if (_Lists.Count > 0)
                {
                    if (CurrentListId > 0)
                    {
                        // Find Current List in Lists List
                        CurrentList = GetListFromLists(CurrentListId);

                        ListsListView.SelectedItem = CurrentList;

                        if (ListsListView.SelectedItem != null)
                        {
                            ScrollToList((List)ListsListView.SelectedItem, ScrollIntoViewAlignment.Leading);
                        }
                    }
                    else
                    {
                        ListsListView.SelectedItem = ListsListView.Items[ListsListView.Items.Count - 1];
                        if (ListsListView.SelectedItem != null)
                        {
                            ScrollToList((List)ListsListView.SelectedItem, ScrollIntoViewAlignment.Default);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message, "Critical Error!");
                await md.ShowAsync();
            }
        }

        #endregion OnNavigatedTo

        #region Events

        private void ListsListView_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Ha‌​nd, 1);
        }

        private void ListsListView_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        private async void ListsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                CurrentList = ((List)e.ClickedItem);

                // Save the Id for the Currently Selected List
                CurrentListId = CurrentList.Id;

                // Pass the List.Id to ListItems
                Frame.Navigate(typeof(ListItems), CurrentList.Id);
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message, "Critical Error!");
                await md.ShowAsync();
            }
        }

        #endregion Events

        #region Methods

        private async void BindListsGridView()
        {
            try
            {
                _Lists = DatabaseHelper.GetLists();
                DefaultViewModel["Lists"] = _Lists;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message, "Critical Error!");
                await md.ShowAsync();
            }
        }

        private void ScrollToList(List List, ScrollIntoViewAlignment Alignment)
        {
            ListsListView.UpdateLayout();
            ListsListView.ScrollIntoView(List, Alignment);
        }

        private List GetListFromLists(int ListId)
        {
            // Find List in Lists List using LINQ
            List CurrentList = (from list in _Lists where list.Id.Equals(ListId) select list).FirstOrDefault();

            return CurrentList;
        }

        #endregion Methods
    }
}
