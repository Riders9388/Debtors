using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Models
{
	public class GridItem
	{
		public GridItem() { }
		public GridItem(string title, Action action)
		{
			Title = title;
			ClickAction = action;
		}
		public GridItem(int id, string title, Action action)
		{
			Id = id;
			Title = title;
			ClickAction = action;
		}
		public int Id { get; set; }
		public string Title { get; set; }
		public Action ClickAction { get; set; }
	}

	public class GridItem<T> : GridItem
	{
		public GridItem() { }
		public GridItem(int id, string title, T value)
		{
			Id = id;
			Title = title;
			Value = value;
		}
		public T Value { get; set; }
	}
}
