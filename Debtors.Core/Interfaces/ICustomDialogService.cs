using Debtors.Core.Enums;
using Debtors.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Debtors.Core.Interfaces
{
	public interface ICustomDialogService
	{
		void Show(string title, List<GridItem> actions, enumPlace gravity);
		void Show(string title, List<GridItem> actions);
		void Show(List<GridItem> actions);
		Task<GridItem> ShowAsync(string title, List<GridItem> actions, enumPlace gravity);
		Task<GridItem> ShowAsync(string title, List<GridItem> actions);
		Task<GridItem> ShowAsync(List<GridItem> actions);
		Task<GridItem<T>> ShowAsync<T>(string title, List<GridItem<T>> actions, enumPlace gravity);
		Task<GridItem<T>> ShowAsync<T>(string title, List<GridItem<T>> actions);
		Task<GridItem<T>> ShowAsync<T>(List<GridItem<T>> actions);
	}
}
