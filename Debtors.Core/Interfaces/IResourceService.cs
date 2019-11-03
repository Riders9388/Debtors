using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Interfaces
{
	public interface IResourceService
	{
		string GetString(string key);
		int GetStyle(string key);
		int GetLayout(string key);
		byte[] GetBytesFromDrawable(string key);
	}
}
