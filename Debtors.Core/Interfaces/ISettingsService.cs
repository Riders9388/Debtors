using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Interfaces
{
    public interface ISettingsService
    {
        string GeneralSettings { get; set; }
        int GeneralIntSettings { get; set; }
    }
}
