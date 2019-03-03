using Acr.UserDialogs;
using Debtors.Core.Models;
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
        #endregion

        #region Methods
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
            config.Add(ResourceService.GetText("cancelAction"));
            UserDialogs.Instance.ActionSheet(config);
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
            config.Add(ResourceService.GetText("cancelAction"));
            UserDialogs.Instance.ActionSheet(config);
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
        #endregion
    }
}
