using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Interfaces
{
    public interface IResourceService
    {
        string GetText(string name);
        int GetStyle(string name);
        byte[] GetBytesFromDrawable(string name);
    }
}
