using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMStudio.Helpers;
using TMStudio.Utils;

namespace TMStudio.Converts
{
    public class ItemIdToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            int _id = int.Parse(value.ToString());

            if (ItemsManager.Instance != null)
            {
                var _item = ItemsManager.Instance.Items.Where(x => x.Id == _id).FirstOrDefault();

                if (_item != null)
                {
                    return _item.Textures[0].Texture1.ToImage();
                }
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
