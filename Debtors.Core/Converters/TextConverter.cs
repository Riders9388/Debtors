using Debtors.Core.Interfaces;
using Debtors.Core.Resources.Strings;
using MvvmCross;
using MvvmCross.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Debtors.Core.Converters
{
	public class TextConverter : MvxValueConverter<string, string>
	{
		private ILogService _logService;
		private ILogService logService => _logService = _logService ?? Mvx.IoCProvider.Resolve<ILogService>();

		protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
		{
			string text = string.Empty;
			try
			{
				text = AppStrings.ResourceManager.GetString(value, culture);
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return text;
		}
	}
}
