﻿using Acr.UserDialogs;
using Debtors.Core.Enums;
using Debtors.Core.Extensions;
using Debtors.Core.Interfaces;
using Debtors.Core.Models;
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
        public IMvxCommand DeleteClickCommand
        {
            get
            {
                deleteClickCommand = deleteClickCommand ?? new MvxCommand(DeleteDebtor);
                return deleteClickCommand;
            }
        }

        private IMvxCommand saveClickCommand;
        public IMvxCommand SaveClickCommand
        {
            get
            {
                saveClickCommand = saveClickCommand ?? new MvxCommand(SaveDebtor);
                return saveClickCommand;
            }
        }

        private IMvxCommand addPhoneClickCommand;
        public IMvxCommand AddPhoneClickCommand
        {
            get
            {
                addPhoneClickCommand = addPhoneClickCommand ?? new MvxCommand(AddPhone);
                return addPhoneClickCommand;
            }
        }

        private IMvxCommand addMailClickCommand;
        public IMvxCommand AddMailClickCommand
        {
            get
            {
                addMailClickCommand = addMailClickCommand ?? new MvxCommand(AddMail);
                return addMailClickCommand;
            }
        }

        private IMvxCommand<Phone> phoneClickCommand;
        public IMvxCommand<Phone> PhoneClickCommand
        {
            get
            {
                phoneClickCommand = phoneClickCommand ?? new MvxCommand<Phone>(ClickPhone);
                return phoneClickCommand;
            }
        }

        private IMvxCommand<Mail> mailClickCommand;
        public IMvxCommand<Mail> MailClickCommand
        {
            get
            {
                mailClickCommand = mailClickCommand ?? new MvxCommand<Mail>(ClickMail);
                return mailClickCommand;
            }
        }

        private IMvxCommand showDebtsClickCommand;
        public IMvxCommand ShowDebtsClickCommand
        {
            get
            {
                showDebtsClickCommand = showDebtsClickCommand ?? new MvxCommand(ShowDebts);
                return showDebtsClickCommand;
            }
        }

        private IMvxCommand setPictureClickCommand;
        public IMvxCommand SetPictureClickCommand
        {
            get
            {
                setPictureClickCommand = setPictureClickCommand ?? new MvxCommand(GetImage);
                return setPictureClickCommand;
            }
        }
        #endregion

        #region Methods
        private void DeleteDebtor()
        {
            if (Debtor == null || Debtor.Id <= 0)
            {
                UserDialogs.Instance.Alert(ResourceService.GetText("debtorIsNotSave"));
                return;
            }

            UserDialogs.Instance.Confirm(ResourceService.GetText("reallyDelete"),
                ResourceService.GetText("yes"),
                ResourceService.GetText("no"),
                (accepted) =>
                {
                    if (!accepted || Debtor == null)
                        return;

                    if (DatabaseService.RemoveDebtor(Debtor.Id))
                        NavigationService.Close(this, true);
                    else
                        UserDialogs.Instance.ToastFailure(ResourceService.GetText("error"));
                });
        }

        private void SaveDebtor()
        {
            if (string.IsNullOrWhiteSpace(Debtor.FirstName))
            {
                UserDialogs.Instance.Alert(ResourceService.GetText("noDebtorName"));
                return;
            }

            UserDialogs.Instance.Confirm(ResourceService.GetText("reallySave"),
                ResourceService.GetText("yes"),
                ResourceService.GetText("no"),
                (accepted) =>
                {
                    if (!accepted || Debtor == null)
                        return;

                    if (DatabaseService.InsertOrUpdateDebtor(Debtor))
                        UserDialogs.Instance.ToastSucceed(ResourceService.GetText("saved"));
                    else
                        UserDialogs.Instance.ToastFailure(ResourceService.GetText("error"));
                });
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
            config.SetMessage(ResourceService.GetText("setPhoneNumber"));
            config.OkText = ResourceService.GetText("ok");
            config.CancelText = ResourceService.GetText("cancel");
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
            config.SetMessage(ResourceService.GetText("setPhoneNumber"));
            config.SetText(phone.Number);
            config.OkText = ResourceService.GetText("ok");
            config.CancelText = ResourceService.GetText("cancel");
            UserDialogs.Instance.Prompt(config);
        }

        private void ClickPhone(Phone phone)
        {
            if (phone == null)
                return;

            ActionSheetConfig config = new ActionSheetConfig();
            config.Add(ResourceService.GetText("callAction"), () =>
            {
                IPhoneCallTask phoneDialer = CrossMessaging.Current.PhoneDialer;
                if (!phoneDialer.CanMakePhoneCall)
                {
                    UserDialogs.Instance.Alert(ResourceService.GetText("cannotCall"));
                    return;
                }
                phoneDialer.MakePhoneCall(phone.Number);
            });
            config.Add(ResourceService.GetText("messageAction"), () =>
            {
                ISmsTask smsMessenger = CrossMessaging.Current.SmsMessenger;
                if (!smsMessenger.CanSendSms)
                {
                    UserDialogs.Instance.Alert(ResourceService.GetText("cannotSms"));
                    return;
                }
                smsMessenger.SendSms(phone.Number);
            });
            config.Add(ResourceService.GetText("editAction"), () =>
            {
                EditPhone(phone);
            });
            config.Add(ResourceService.GetText("deleteAction"), () =>
            {
                if (Debtor == null || Debtor.Phones.IsNullOrEmpty())
                    return;

                UserDialogs.Instance.Confirm(ResourceService.GetText("reallyDelete"),
                    ResourceService.GetText("yes"),
                    ResourceService.GetText("no"),
                    (accepted) =>
                    {
                        if (accepted)
                            Debtor.Phones.Remove(phone);
                    });
            });
            config.Add(ResourceService.GetText("cancelAction"));
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
            config.SetMessage(ResourceService.GetText("setMailAddress"));
            UserDialogs.Instance.Prompt(config);
        }

        private void ClickMail(Mail mail)
        {
            if (mail == null)
                return;

            ActionSheetConfig config = new ActionSheetConfig();
            config.Add(ResourceService.GetText("messageAction"), () =>
            {
                IEmailTask emailMessenger = CrossMessaging.Current.EmailMessenger;
                if (!emailMessenger.CanSendEmail)
                {
                    UserDialogs.Instance.Alert(ResourceService.GetText("cannotMail"));
                    return;
                }
                emailMessenger.SendEmail(mail.Address);
            });
            config.Add(ResourceService.GetText("editAction"), () =>
            {
                EditMail(mail);
            });
            config.Add(ResourceService.GetText("deleteAction"), () =>
            {
                if (Debtor == null || Debtor.Mails.IsNullOrEmpty())
                    return;

                UserDialogs.Instance.Confirm(ResourceService.GetText("reallyDelete"),
                    ResourceService.GetText("yes"),
                    ResourceService.GetText("no"),
                    (accepted) =>
                    {
                        if (accepted)
                            Debtor.Mails.Remove(mail);
                    });
            });
            config.Add(ResourceService.GetText("cancelAction"));
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
            config.SetMessage(ResourceService.GetText("setMailAddress"));
            config.SetText(mail.Address);
            config.OkText = ResourceService.GetText("ok");
            config.CancelText = ResourceService.GetText("cancel");
            UserDialogs.Instance.Prompt(config);
        }

        private void ShowDebts()
        {
            if (Debtor.Id <= 0)
            {
                UserDialogs.Instance.Alert(ResourceService.GetText("debtorIsNotSave"));
                return;
            }
            NavigationService.Navigate<DebtsViewModel, Debtor, bool>(Debtor);
        }

        private async void GetImage()
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
        #endregion
    }
}
