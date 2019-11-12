﻿using Acr.UserDialogs;
using Debtors.Core.Enums;
using Debtors.Core.Extensions;
using Debtors.Core.Interfaces;
using Debtors.Core.Models;
using Debtors.Core.Resources.Strings;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Debtors.Core.ViewModels
{
	public class DebtorViewModel : BaseViewModel<Debtor, bool>
	{
		public DebtorViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
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
		private IMvxCommand deleteClickCommand;
		private IMvxCommand saveClickCommand;
		private IMvxCommand addPhoneClickCommand;
		private IMvxCommand addMailClickCommand;
		private IMvxCommand<Phone> phoneClickCommand;
		private IMvxCommand<Mail> mailClickCommand;
		private IMvxCommand setPictureClickCommand;

		public IMvxCommand DeleteClickCommand => deleteClickCommand = deleteClickCommand ?? new MvxCommand(DeleteDebtor);
		public IMvxCommand SaveClickCommand => saveClickCommand = saveClickCommand ?? new MvxCommand(SaveDebtor);
		public IMvxCommand AddPhoneClickCommand => addPhoneClickCommand = addPhoneClickCommand ?? new MvxCommand(AddPhone);
		public IMvxCommand AddMailClickCommand => addMailClickCommand = addMailClickCommand ?? new MvxCommand(AddMail);
		public IMvxCommand<Phone> PhoneClickCommand => phoneClickCommand = phoneClickCommand ?? new MvxCommand<Phone>(ClickPhone);
		public IMvxCommand<Mail> MailClickCommand => mailClickCommand = mailClickCommand ?? new MvxCommand<Mail>(ClickMail);
		public IMvxCommand SetPictureClickCommand => setPictureClickCommand = setPictureClickCommand ?? new MvxCommand(GetImage);
		#endregion

		#region Methods
		private void DeleteDebtor()
		{
			if (Debtor == null || Debtor.Id <= 0)
			{
				UserDialogs.Instance.Alert(AppStrings.debtorIsNotSave);
				return;
			}

			UserDialogs.Instance.Confirm(AppStrings.reallyDelete, AppStrings.yes, AppStrings.no,
				(accepted) =>
				{
					if (!accepted || Debtor == null)
						return;

					if (DatabaseService.RemoveDebtor(Debtor.Id))
						NavigationService.Close(this, true);
					else
						UserDialogs.Instance.ToastFailure(AppStrings.error);
				});
		}
		private void SaveDebtor()
		{
			if (string.IsNullOrWhiteSpace(Debtor.FirstName))
			{
				UserDialogs.Instance.Alert(AppStrings.noDebtorName);
				return;
			}

			if (DatabaseService.InsertOrUpdateDebtor(Debtor))
				UserDialogs.Instance.ToastSucceed(AppStrings.saved);
			else
				UserDialogs.Instance.ToastFailure(AppStrings.error);
		}
		private void AddPhone()
		{
			PromptConfig config = new PromptConfig();
			config.SetAction((result) =>
			{
				if (!result.Ok || Debtor == null || string.IsNullOrWhiteSpace(result.Value))
					return;

				if (Debtor.Phones == null)
					Debtor.Phones = new MvxObservableCollection<Phone>();

				Debtor.Phones.Add(new Phone()
				{
					Number = result.Value,
					Type = PhoneNumberType.Mobile
				});
				RaisePropertyChanged(() => Debtor);
			});
			config.SetInputMode(InputType.Phone);
			config.SetMessage(AppStrings.setPhoneNumber);
			config.OkText = AppStrings.ok;
			config.CancelText = AppStrings.cancel;
			UserDialogs.Instance.Prompt(config);
		}
		private void EditPhone(Phone phone)
		{
			if (phone == null)
				return;

			PromptConfig config = new PromptConfig();
			config.SetAction((result) =>
			{
				if (!result.Ok || Debtor == null || string.IsNullOrWhiteSpace(result.Value))
					return;

				phone.Number = result.Value;
				RaisePropertyChanged(() => Debtor);
			});
			config.SetInputMode(InputType.Phone);
			config.SetMessage(AppStrings.setPhoneNumber);
			config.SetText(phone.Number);
			config.OkText = AppStrings.ok;
			config.CancelText = AppStrings.cancel;
			UserDialogs.Instance.Prompt(config);
		}
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
			config.Add(AppStrings.editAction, () =>
			{
				EditPhone(phone);
			});
			config.Add(AppStrings.deleteAction, () =>
			{
				if (Debtor == null || Debtor.Phones.IsNullOrEmpty())
					return;

				UserDialogs.Instance.Confirm(AppStrings.reallyDelete, AppStrings.yes, AppStrings.no,
					(accepted) =>
					{
						if (accepted)
							Debtor.Phones.Remove(phone);
					});
			});
			config.Add(AppStrings.cancelAction);
			UserDialogs.Instance.ActionSheet(config);
		}
		private void AddMail()
		{
			PromptConfig config = new PromptConfig();
			config.SetAction((result) =>
			{
				if (!result.Ok || Debtor == null || string.IsNullOrWhiteSpace(result.Value))
					return;

				if (Debtor.Mails == null)
					Debtor.Mails = new MvxObservableCollection<Mail>();

				Debtor.Mails.Add(new Mail()
				{
					Address = result.Value
				});
				RaisePropertyChanged(() => Debtor);
			});
			config.SetInputMode(InputType.Email);
			config.SetMessage(AppStrings.setMailAddress);
			UserDialogs.Instance.Prompt(config);
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
			config.Add(AppStrings.editAction, () =>
			{
				EditMail(mail);
			});
			config.Add(AppStrings.deleteAction, () =>
			{
				if (Debtor == null || Debtor.Mails.IsNullOrEmpty())
					return;

				UserDialogs.Instance.Confirm(AppStrings.reallyDelete, AppStrings.yes, AppStrings.no,
					(accepted) =>
					{
						if (accepted)
							Debtor.Mails.Remove(mail);
					});
			});
			config.Add(AppStrings.cancelAction);
			UserDialogs.Instance.ActionSheet(config);
		}
		private void EditMail(Mail mail)
		{
			if (mail == null)
				return;

			PromptConfig config = new PromptConfig();
			config.SetAction((result) =>
			{
				if (!result.Ok || Debtor == null || string.IsNullOrWhiteSpace(result.Value))
					return;

				mail.Address = result.Value;
				RaisePropertyChanged(() => Debtor);
			});
			config.SetInputMode(InputType.Email);
			config.SetMessage(AppStrings.setMailAddress);
			config.SetText(mail.Address);
			config.OkText = AppStrings.ok;
			config.CancelText = AppStrings.cancel;
			UserDialogs.Instance.Prompt(config);
		}
		private async void GetImage()
		{
			try
			{
				if (!CrossMedia.IsSupported || !CrossMedia.Current.IsPickPhotoSupported)
					return;

				MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
				if (photo == null)
					return;

				Stream stream = photo.GetStreamWithImageRotatedForExternalStorage();
				using (MemoryStream ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					Debtor.Image = ms.ToArray();
				}
			}
			catch(Exception ex)
			{
				LogService.Error(ex);
			}
		}
		#endregion
	}
}
