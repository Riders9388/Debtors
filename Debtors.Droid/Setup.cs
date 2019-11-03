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
using Debtors.Core.Interfaces;
using Debtors.Core.Services;
using Debtors.Droid.Services;
using MvvmCross;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.IoC;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.ViewModels;

namespace Debtors.Droid
{
	public class Setup : MvxAppCompatSetup<App>
	{
		protected override void InitializeLastChance()
		{
			base.InitializeLastChance();
			
			Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDatabaseService, DatabaseService>();
			Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IResourceService, ResourceService>();
			Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ISettingsService, SettingsService>();
			Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ICustomDialogService, CustomDialogService>();
			Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDialogService, DialogService>();
		}
	}
}