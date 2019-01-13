using Debtors.Core.Interfaces;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private ISettings AppSettings => CrossSettings.Current;

        public string GeneralSettings
        {
            get => AppSettings.GetValueOrDefault(nameof(GeneralSettings), "");
            set => AppSettings.AddOrUpdateValue(nameof(GeneralSettings), value);
        }

        public int GeneralIntSettings
        {
            get => AppSettings.GetValueOrDefault(nameof(GeneralIntSettings), 0);
            set => AppSettings.AddOrUpdateValue(nameof(GeneralIntSettings), value);
        }
    }
}
