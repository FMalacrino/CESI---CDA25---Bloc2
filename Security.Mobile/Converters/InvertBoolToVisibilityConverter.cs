using System.Globalization;

namespace Security.Mobile.Converters
{
    public class InvertBoolToVisibilityConverter : IValueConverter
    {
        // VM -> V
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || value is not bool boolValue)
                return false;

            return !boolValue;
                //boolValue
                //? Visibility.Hidden
                //: Visibility.Visible;
        }

        // V -> VM
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}