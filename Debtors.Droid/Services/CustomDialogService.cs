using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debtors.Core.Enums;
using Debtors.Core.Interfaces;
using Debtors.Core.Models;
using Debtors.Droid.Adapters;
using Plugin.CurrentActivity;

namespace Debtors.Droid.Services
{
	public class CustomDialogService : ICustomDialogService
	{
		public void Show(string title, List<GridItem> actions, enumPlace gravity)
		{
			AlertDialog ad = null;
			View customView = CrossCurrentActivity.Current.Activity.LayoutInflater.Inflate(Resource.Layout.CustomGridViewDialog, null);
			List<GridItem> correctActions = new List<GridItem>();
			if (actions != null)
			{
				GridItem gridItem;
				foreach (var action in actions)
				{
					gridItem = new GridItem();
					gridItem.Id = action.Id;
					gridItem.Title = action.Title;
					gridItem.ClickAction = () =>
					{
						if (ad != null)
							ad.Dismiss();

						action.ClickAction?.Invoke();
					};
					correctActions.Add(gridItem);
				}
			}

			(customView.FindViewById<GridView>(Resource.Id.gridview)).Adapter = new GridAdapter(CrossCurrentActivity.Current.Activity, correctActions, gravity);
			AlertDialog.Builder builder = new AlertDialog.Builder(CrossCurrentActivity.Current.Activity);
			builder.SetTitle(title ?? "");
			builder.SetView(customView);
			ad = builder.Show();
		}

		public void Show(string title, List<GridItem> actions)
		{
			Show(title, actions, enumPlace.Center);
		}

		public void Show(List<GridItem> actions)
		{
			Show(null, actions, enumPlace.Center);
		}

		public Task<GridItem> ShowAsync(string title, List<GridItem> actions, enumPlace gravity)
		{
			TaskCompletionSource<GridItem> tcs = new TaskCompletionSource<GridItem>();
			bool clicked = false;
			AlertDialog ad = null;
			View customView = CrossCurrentActivity.Current.Activity.LayoutInflater.Inflate(Resource.Layout.CustomGridViewDialog, null);
			List<GridItem> correctActions = new List<GridItem>();
			if (actions != null)
			{
				foreach (var action in actions)
				{
					action.ClickAction = () =>
					{
						clicked = true;
						if (ad != null)
							ad.Dismiss();

						tcs.SetResult(action);
					};
					correctActions.Add(action);
				}
			}

			(customView.FindViewById<GridView>(Resource.Id.gridview)).Adapter = new GridAdapter(CrossCurrentActivity.Current.Activity, correctActions, gravity);
			AlertDialog.Builder builder = new AlertDialog.Builder(CrossCurrentActivity.Current.Activity);
			builder.SetTitle(title ?? "");
			builder.SetView(customView);
			builder.SetOnDismissListener(new OnDismissListener(() =>
			{
				if (!clicked)
					tcs.TrySetResult(null);
			}));
			ad = builder.Show();

			return tcs.Task;
		}

		public Task<GridItem> ShowAsync(string title, List<GridItem> actions)
		{
			return ShowAsync(title, actions, enumPlace.Center);
		}

		public Task<GridItem> ShowAsync(List<GridItem> actions)
		{
			return ShowAsync(null, actions, enumPlace.Center);
		}

		public Task<GridItem<T>> ShowAsync<T>(string title, List<GridItem<T>> actions, enumPlace gravity)
		{
			TaskCompletionSource<GridItem<T>> tcs = new TaskCompletionSource<GridItem<T>>();
			bool clicked = false;
			AlertDialog ad = null;
			View customView = CrossCurrentActivity.Current.Activity.LayoutInflater.Inflate(Resource.Layout.CustomGridViewDialog, null);
			List<GridItem<T>> correctActions = new List<GridItem<T>>();
			if (actions != null)
			{
				foreach (var action in actions)
				{
					action.ClickAction = () =>
					{
						clicked = true;
						if (ad != null)
							ad.Dismiss();

						tcs.SetResult(action);
					};
					correctActions.Add(action);
				}
			}

			(customView.FindViewById<GridView>(Resource.Id.gridview)).Adapter = new GridAdapter<T>(CrossCurrentActivity.Current.Activity, correctActions, gravity);
			AlertDialog.Builder builder = new AlertDialog.Builder(CrossCurrentActivity.Current.Activity);
			builder.SetTitle(title ?? "");
			builder.SetView(customView);
			builder.SetOnDismissListener(new OnDismissListener(() =>
			{
				if (!clicked)
					tcs.TrySetResult(null);
			}));
			ad = builder.Show();

			return tcs.Task;
		}

		public Task<GridItem<T>> ShowAsync<T>(string title, List<GridItem<T>> actions)
		{
			return ShowAsync(title, actions, enumPlace.Center);
		}

		public Task<GridItem<T>> ShowAsync<T>(List<GridItem<T>> actions)
		{
			return ShowAsync(null, actions, enumPlace.Center);
		}
	}

	class OnDismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
	{
		private readonly Action action;

		public OnDismissListener(Action action)
		{
			this.action = action;
		}

		public void OnDismiss(IDialogInterface dialog)
		{
			this.action();
		}
	}
}