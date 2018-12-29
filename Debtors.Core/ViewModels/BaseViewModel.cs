using Debtors.Core.Interfaces;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        protected readonly IMvxNavigationService NavigationService;
        protected readonly IDatabaseService DatabaseService;

        public BaseViewModel()
        {
            NavigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            DatabaseService = Mvx.IoCProvider.Resolve<IDatabaseService>();
        }
    }
}
