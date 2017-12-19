 using System;
using Windows.Storage;
using Windows.UI.Xaml.Data;

namespace ListManager.Converters
{
    public class MenuFlyoutIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string SourcePage = (string)parameter;

                switch (SourcePage)
                {
                    case "ListItemsCutPaste":
                        switch ((string)ApplicationData.Current.LocalSettings.Values["ListItemsCutPasteComposite"])
                        {
                            case null:
                                return false;
                            default:
                                return true;
                        }
                    case "ListItemsCopyPaste":
                        switch ((string)ApplicationData.Current.LocalSettings.Values["ListItemsCopyPasteComposite"])
                        {
                            case null:
                                return false;
                            default:
                                return true;
                        }
                    case "ListEditCopyName":
                        switch ((string)ApplicationData.Current.LocalSettings.Values["ListEditCopyNameComposite"])
                        {
                            case null:
                                return false;
                            default:
                                return true;
                        }
                    case "ListEditCopyItems":
                        switch ((string)ApplicationData.Current.LocalSettings.Values["ListEditCopyItemsComposite"])
                        {
                            case null:
                                return false;
                            default:
                                return true;
                        }
                }

                return null;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return new Exception(ex.Message);
            }
        }
    }
}
