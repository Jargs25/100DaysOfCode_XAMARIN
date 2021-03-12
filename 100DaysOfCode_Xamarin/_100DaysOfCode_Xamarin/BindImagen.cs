using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace _100DaysOfCode_Xamarin
{
    public class BindImagen : IValueConverter
    {
        public object Convert(object value, Type oType, object param, System.Globalization.CultureInfo culture)
        {
            ImageSource imgSource = null;

            if (value != null)
            {
                byte[] img = (byte[])value;
                imgSource = ImageSource.FromStream(() => new MemoryStream(img));
            }

            return imgSource;
        }

        public object ConvertBack(object value, Type oType, object param, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
