using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Debtors.Core.ViewModels
{
    public abstract class BaseViewModelResult<TResult> : BaseViewModel, IMvxViewModelResult<TResult>, IMvxViewModel
    {
        protected BaseViewModelResult(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService) { }

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }
    }
}
