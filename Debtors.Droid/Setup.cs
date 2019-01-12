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
using Debtors.Droid.Services;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.ViewModels;

namespace Debtors.Droid
{
    public class Setup : MvxAndroidSetup<App>
    {
        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();
            
            Mvx.IoCProvider.RegisterSingleton<IResourceService>(new ResourceService(Application.Context));
        }
    }
}