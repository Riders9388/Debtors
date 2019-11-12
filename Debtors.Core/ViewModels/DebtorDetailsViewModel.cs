﻿using Acr.UserDialogs;
using Debtors.Core.Models;
using Debtors.Core.Resources.Strings;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.ViewModels
{
	public class DebtorDetailsViewModel : BaseViewModel<Debtor, bool>
	{
		public DebtorDetailsViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
			: base(logProvider, navigationService) { }

		#region Overwritten
		public override void Prepare(Debtor parameter)
		{
			if (parameter == null)
			{
				Debtor = new Debtor();
				return;
			}

			Debtor = parameter;
		}
		public override void ViewDestroy(bool viewFinishing = true)
		{
			base.ViewDestroy(viewFinishing);
			NavigationService.Close(this, true);
		}
		#endregion

		#region Properties
		private Debtor debtor;
		public Debtor Debtor
		{
			get { return debtor; }
			set
			{
				debtor = value;
				RaisePropertyChanged(() => Debtor);
			}
		}
		#endregion

		#region Commands
		private IMvxCommand<Phone> phoneClickCommand;
		private IMvxCommand<Mail> mailClickCommand;
		private IMvxCommand showDebtsClickCommand;

		public IMvxCommand<Phone> PhoneClickCommand => phoneClickCommand = phoneClickCommand ?? new MvxCommand<Phone>(ClickPhone);
		public IMvxCommand<Mail> MailClickCommand => mailClickCommand = mailClickCommand ?? new MvxCommand<Mail>(ClickMail);
		public IMvxCommand ShowDebtsClickCommand => showDebtsClickCommand = showDebtsClickCommand ?? new MvxCommand(ShowDebts);
		#endregion

		#region Methods
		private void ClickPhone(Phone phone)
		{
			if (phone == null)
				return;

			ActionSheetConfig config = new ActionSheetConfig();
			config.Add(AppStrings.callAction, () =>
			{
				IPhoneCallTask phoneDialer = CrossMessaging.Current.PhoneDialer;
				if (!phoneDialer.CanMakePhoneCall)
				{
					UserDialogs.Instance.Alert(AppStrings.cannotCall);
					return;
				}
				phoneDialer.MakePhoneCall(phone.Number);
			});
			config.Add(AppStrings.messageAction, () =>
			{
				ISmsTask smsMessenger = CrossMessaging.Current.SmsMessenger;
				if (!smsMessenger.CanSendSms)
				{
					UserDialogs.Instance.Alert(AppStrings.cannotSms);
					return;
				}
				smsMessenger.SendSms(phone.Number);
			});
			config.Add(AppStrings.cancelAction);
			UserDialogs.Instance.ActionSheet(config);
		}
		private void ClickMail(Mail mail)
		{
			if (mail == null)
				return;

			ActionSheetConfig config = new ActionSheetConfig();
			config.Add(AppStrings.messageAction, () =>
			{
				IEmailTask emailMessenger = CrossMessaging.Current.EmailMessenger;
				if (!emailMessenger.CanSendEmail)
				{
					UserDialogs.Instance.Alert(AppStrings.cannotMail);
					return;
				}
				emailMessenger.SendEmail(mail.Address);
			});
			config.Add(AppStrings.cancelAction);
			UserDialogs.Instance.ActionSheet(config);
		}
		private void ShowDebts()
		{
			if (Debtor.Id <= 0)
			{
				UserDialogs.Instance.Alert(AppStrings.debtorIsNotSave);
				return;
			}
			NavigationService.Navigate<DebtsViewModel, Debtor, bool>(Debtor);
		}
		#endregion
	}
}
