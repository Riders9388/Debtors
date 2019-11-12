using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Interfaces
{
	public interface ILogService
	{
		void Error(string message);
		void Error(Exception e, string message);
		void Error(string format, params object[] args);
		void Error(Exception e, string format, params object[] args);
	}
}
