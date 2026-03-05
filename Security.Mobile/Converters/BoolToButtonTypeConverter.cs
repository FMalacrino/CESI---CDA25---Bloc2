using System.Globalization;
using HorusStudio.Maui.MaterialDesignControls;

namespace Security.Mobile.Converters
{
    public class BoolToButtonTypeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return "MaterialOutlinedFilled";

            return
                boolValue
                ? "MaterialOutlinedFilled"
                : "MaterialOutlined";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}