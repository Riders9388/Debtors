using Acr.UserDialogs;
using Debtors.Core.Interfaces;
using Debtors.Core.ViewModels;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Debtors.Core
{
	public class AppStart : MvxAppStart
	{
		public AppStart(IMvxApplication app, IMvxNavigationService mvxNavigationService)
			: base(app, mvxNavigationService) { }

		protected override Task NavigateToFirstViewModel(object hint = null)
		{
			IResourceService resourceService = Mvx.IoCProvider.Resolve<IResourceService>();

			string okText = resourceService.GetString("ok");
			string cancelText = resourceService.GetString("cancel");

			ConfirmConfig.DefaultOkText = okText;
			ConfirmConfig.DefaultCancelText = cancelText;
			ConfirmConfig.DefaultNo = resourceService.GetString("no");
			ConfirmConfig.DefaultYes = resourceService.GetString("yes");

			AlertConfig.DefaultOkText = okText;

			PromptConfig.DefaultCancelText = cancelText;
			PromptConfig.DefaultOkText = okText;

			return NavigationService.Navigate<DebtorsViewModel>();
		}
	}
}
