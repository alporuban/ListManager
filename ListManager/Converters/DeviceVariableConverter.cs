using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ListManager.Converters
{
    public class DeviceVariableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((string)parameter)
            {
                case "WindowWidth":
                    return ((App)Application.Current).WindowWidth;
                case "ItemWidth":
                    return ((App)Application.Current).ItemWidth;
                case "PageMargins":
                    return ((App)Application.Current).PageMargins;
                case "ItemMargins":
                    return ((App)Application.Current).ItemMargins;
                case "PageAlignment":
                    return ((App)Application.Current).PageAlignment;
                default:
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
