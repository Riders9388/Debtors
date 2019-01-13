using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debtors.Core.Interfaces;
using Java.IO;
using MvvmCross;

namespace Debtors.Droid.Services
{
    public class ResourceService : IResourceService
    {
        public Context context;
        public ResourceService(Context context)
        {
            this.context = context;
        }

        public string GetText(string name)
        {
            string toReturn = name;
            try
            {
                int resourceId = (int)typeof(Resource.String).GetField(name).GetValue(null);
                toReturn = context.GetString(resourceId);
            }
            catch (Exception ex)
            {
                
            }
            return toReturn;
        }

        public int GetStyle(string name)
        {
            int resourceId = 0;
            try
            {
                resourceId = (int)typeof(Resource.Style).GetField(name).GetValue(null);
            }
            catch (Exception ex)
            {

            }
            return resourceId;
        }

        public byte[] GetBytesFromDrawable(string name)
        {
            try
            {
                int resourceId = (int)typeof(Resource.Drawable).GetField(name).GetValue(null);
                Drawable drawable = context.GetDrawable(resourceId);
                Bitmap bitmap = ((BitmapDrawable)drawable).Bitmap;
                MemoryStream ms = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, ms);
                return ms.ToArray();
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}