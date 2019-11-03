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
using Debtors.Core.Enums;
using Debtors.Core.Models;

namespace Debtors.Droid.Adapters
{
	public class GridAdapter : BaseAdapter<GridItem>
	{
		private readonly IList<GridItem> items;
		private readonly Context context;
		private readonly enumPlace alignment;

		public GridAdapter(Context context, IList<GridItem> items)
		{
			this.context = context;
			this.items = items;
			alignment = enumPlace.Center;
		}

		public GridAdapter(Context context, IList<GridItem> items, enumPlace alignment)
		{
			this.context = context;
			this.items = items;
			this.alignment = alignment;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			GridItem item = items[position];
			View view = convertView;
			LayoutInflater inflater = LayoutInflater.FromContext(context);
			view = inflater.Inflate(Resource.Layout.template_dialog_item, parent, false);
			view.Clickable = true;
			view.Click += (s, a) => item.ClickAction.Invoke();

			TextView title = view.FindViewById<TextView>(Resource.Id.dialog_item_title);
			title.Text = item.Title;

			if (alignment == enumPlace.Left)
				title.Gravity = GravityFlags.Left;
			else if (alignment == enumPlace.Right)
				title.Gravity = GravityFlags.Right;
			else
				title.Gravity = GravityFlags.CenterHorizontal;

			return view;
		}

		public override int Count
		{
			get { return items.Count; }
		}

		public override GridItem this[int position]
		{
			get { return items[position]; }
		}
	}

	public class GridAdapter<T> : BaseAdapter<GridItem<T>>
	{
		private readonly IList<GridItem<T>> items;
		private readonly Context context;
		private readonly enumPlace alignment;

		public GridAdapter(Context context, IList<GridItem<T>> items)
		{
			this.context = context;
			this.items = items;
			alignment = enumPlace.Center;
		}

		public GridAdapter(Context context, IList<GridItem<T>> items, enumPlace alignment)
		{
			this.context = context;
			this.items = items;
			this.alignment = alignment;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			GridItem<T> item = items[position];
			View view = convertView;
			LayoutInflater inflater = LayoutInflater.FromContext(context);
			view = inflater.Inflate(Resource.Layout.template_dialog_item, parent, false);
			view.Clickable = true;
			view.Click += (s, a) => item.ClickAction.Invoke();

			TextView title = view.FindViewById<TextView>(Resource.Id.dialog_item_title);
			title.Text = item.Title;

			if (alignment == enumPlace.Left)
				title.Gravity = GravityFlags.Left;
			else if (alignment == enumPlace.Right)
				title.Gravity = GravityFlags.Right;
			else
				title.Gravity = GravityFlags.CenterHorizontal;

			return view;
		}

		public override int Count
		{
			get { return items.Count; }
		}

		public override GridItem<T> this[int position]
		{
			get { return items[position]; }
		}
	}
}