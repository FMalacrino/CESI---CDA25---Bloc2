using System.Globalization;

namespace Security.Mobile.Converters
{
    public class PasswordMatchConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || value is not bool match)
                return "";

            return
                match
                ? ""
                : "Le mot de passe ne correspond pas";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}