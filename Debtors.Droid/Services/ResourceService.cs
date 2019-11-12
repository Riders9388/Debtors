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
using Plugin.CurrentActivity;

namespace Debtors.Droid.Services
{
	public class ResourceService : IResourceService
	{
		private readonly ILogService logService;

		public ResourceService(ILogService logService)
		{
			this.logService = logService;
		}

		public string GetString(string key)
		{
				int resourceId = (int)(typeof(Resource.String).GetField(key)?.GetValue(null) ?? 0);
				return CrossCurrentActivity.Current.Activity.GetString(resourceId);
		}

		public int GetStyle(string key)
		{
			   var resourceId = (int)(typeof(Resource.Style).GetField(key).GetValue(null) ?? 0);
			return resourceId;
		}

		public int GetLayout(string key)
		{
			var resourceId = (int)(typeof(Resource.Layout).GetField(key).GetValue(null) ?? 0);
			return resourceId;
		}

		public byte[] GetBytesFromDrawable(string key)
		{
			try
			{
				int resourceId = (int)(typeof(Resource.Drawable).GetField(key).GetValue(null) ?? 0);
				Drawable drawable = CrossCurrentActivity.Current.Activity.GetDrawable(resourceId);
				Bitmap bitmap = ((BitmapDrawable)drawable).Bitmap;
				MemoryStream ms = new MemoryStream();
				bitmap.Compress(Bitmap.CompressFormat.Png, 0, ms);
				return ms.ToArray();
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return null;
		}
	}
}