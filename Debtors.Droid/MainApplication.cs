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
using Debtors.Core;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Views;
using Plugin.CurrentActivity;

namespace Debtors.Droid
{
	[Application]
	public class MainApplication : MvxAppCompatApplication<Setup, App>
	{
		public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();
			CrossCurrentActivity.Current.Init(this);
		}

		public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
			CrossCurrentActivity.Current.Activity = activity;
		}

		public void OnActivityResumed(Activity activity)
		{
			CrossCurrentActivity.Current.Activity = activity;
		}

		public void OnActivityStarted(Activity activity)
		{
			CrossCurrentActivity.Current.Activity = activity;
		}
	}
}