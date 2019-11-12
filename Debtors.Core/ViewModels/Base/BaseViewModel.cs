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
		private IDatabaseService databaseService;
		private IResourceService resourceService;
		private ILogService logService;

		protected IDatabaseService DatabaseService => databaseService = databaseService ?? Mvx.IoCProvider.Resolve<IDatabaseService>();
		protected IResourceService ResourceService => resourceService = resourceService ?? Mvx.IoCProvider.Resolve<IResourceService>();
		protected ILogService LogService => logService = logService ?? Mvx.IoCProvider.Resolve<ILogService>();

		public BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
			: base(logProvider, navigationService) { }
	}
}
