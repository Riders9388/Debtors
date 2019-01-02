using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Debtors.Core.ViewModels;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Droid.Support.V7.AppCompat;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Views.InputMethods;
using Android.Support.V4.View;
using Android.Content.Res;
using Android.Support.Design.Widget;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Content.PM;

namespace Debtors.Droid.Views
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DebtorsView : BaseView<DebtorsViewModel>
    {
        protected Toolbar toolbar { get; private set; }
        protected MvxActionBarDrawerToggle drawerToggle { get; private set; }
        protected DrawerLayout drawerLayout { get; private set; }
        protected NavigationView navigationView { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.layout_debtors);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            drawerToggle = new MvxActionBarDrawerToggle(
                this,                                   // host Activity
                drawerLayout,                           // DrawerLayout object
                toolbar,                                // nav drawer icon to replace 'Up' caret
                Resource.String.drawer_open,            // "open drawer" description
                Resource.String.drawer_close            // "close drawer" description
            );

            drawerToggle.DrawerOpened += (sender, e) => HideSoftKeyboard();
            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
        }

        private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            e.MenuItem.SetCheckable(false);
            e.MenuItem.SetChecked(false);
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.settings:
                    Toast.MakeText(this, "Settings clicked!", ToastLength.Short).Show();
                    break;
                case Resource.Id.about:
                    ViewModel.AboutClickCommand.Execute();
                    break;
                default:
                    break;
            }

            drawerLayout.CloseDrawers();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            navigationView.InflateMenu(Resource.Menu.nav_menu);
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.home:
                    drawerLayout.OpenDrawer(GravityCompat.Start);
                    break;
                case Resource.Id.menu_add:
                    ViewModel.AddClickCommand.ExecuteAsync();
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            if (drawerLayout != null && drawerLayout.IsDrawerOpen(GravityCompat.Start))
                drawerLayout.CloseDrawers();
            else
                base.OnBackPressed();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (toolbar != null)
                drawerToggle?.OnConfigurationChanged(newConfig);
        }
    }
}