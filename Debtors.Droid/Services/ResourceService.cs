using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debtors.Core.Interfaces;
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
    }
}