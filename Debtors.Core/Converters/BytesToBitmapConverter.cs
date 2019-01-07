using Android.Graphics;
using MvvmCross.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Debtors.Core.Converters
{
    public class BytesToBitmapConverter : MvxValueConverter<byte[], Bitmap>
    {
        protected override Bitmap Convert(byte[] value, Type targetType, object parameter, CultureInfo culture)
        {
            return BitmapFactory.DecodeByteArray(value, 0, value.Length);
        }
    }
}
