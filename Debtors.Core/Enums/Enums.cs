using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Debtors.Core.Enums
{
	public enum PhoneNumberType
	{
		Stationary = 0,
		Mobile = 1
	}

	public enum enumPlace
	{
		[Description("Góra")]
		Top = 0,
		[Description("Środek")]
		Center = 1,
		[Description("Dół")]
		Bottom = 2,
		[Description("Lewo")]
		Left = 3,
		[Description("Prawo")]
		Right = 4
	}
}
