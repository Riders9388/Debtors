using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Debtors.Core.Helpers
{
    public static class StaticHelper
    {
        public static readonly string DatabaseName = "DebtorsDatabase.db";
        public static readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DatabaseName);
    }
}
