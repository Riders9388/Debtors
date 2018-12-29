using Debtors.Core.Interfaces;
using MvvmCross;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.ViewModels
{
    public abstract class BaseViewModel : MvxNavigationViewModel
    {
        protected readonly IDatabaseService DatabaseService;
        public BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        {
            DatabaseService = Mvx.IoCProvider.Resolve<IDatabaseService>();
        }
    }
}
