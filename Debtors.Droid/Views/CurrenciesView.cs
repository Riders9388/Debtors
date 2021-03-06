﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debtors.Core.ViewModels;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Debtors.Droid.Views
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait)]
    public class CurrenciesView : BaseView<CurrenciesViewModel>
    {
        protected Toolbar toolbar { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.layout_currencies);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
                case Resource.Id.menu_add:
                    ViewModel.AddClickCommand.Execute();
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}