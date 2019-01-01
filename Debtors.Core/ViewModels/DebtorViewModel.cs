using Acr.UserDialogs;
using Debtors.Core.Enums;
using Debtors.Core.Extensions;
using Debtors.Core.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.ViewModels
{
    public class DebtorViewModel : BaseViewModel<Debtor, bool>
    {
        public DebtorViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        #region Overwritten
        public override void Start()
        {
            base.Start();
        }

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

        #endregion

        #region Methods
        private void DeleteDebtor()
        {
            ConfirmConfig config = new ConfirmConfig();
            config.Message = "Do you really want to delete?";
            config.OnAction = (accepted) =>
            {
                if (!accepted || Debtor == null)
                    return;

                DatabaseService.RemoveDebtor(Debtor);
                NavigationService.Close(this, true);
            };
            UserDialogs.Instance.Confirm(config);
        }

        private void SaveDebtor()
        {
            ConfirmConfig config = new ConfirmConfig();
            config.Message = "Do you really want to save?";
            config.OnAction = (accepted) =>
            {
                if (!accepted || Debtor == null)
                    return;

                DatabaseService.InsertOrUpdateDebtor(Debtor);

                ToastConfig toastConfig = new ToastConfig("Saved");
                //toastConfig.SetIcon("abc_btn_check_to_on_mtrl_000");
                UserDialogs.Instance.Toast(toastConfig);
            };
            UserDialogs.Instance.Confirm(config);
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
            config.SetMessage("Set phone number");
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
            config.SetMessage("Set phone number");
            config.SetText(phone.Number);
            UserDialogs.Instance.Prompt(config);
        }

        private void ClickPhone(Phone phone)
        {
            if (phone == null)
                return;

            ActionSheetConfig config = new ActionSheetConfig();
            config.Add("Call", () =>
            {
                UserDialogs.Instance.Alert("Not implemented");
            });
            config.Add("Send", () =>
            {
                UserDialogs.Instance.Alert("Not implemented");
            });
            config.Add("Edit", () => 
            {
                EditPhone(phone);
            });
            config.Add("Delete", () =>
            {
                if (Debtor == null || Debtor.Phones.IsNullOrEmpty())
                    return;

                Debtor.Phones.Remove(phone);
            });
            config.Add("Cancel");
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
            config.SetMessage("Set mail address");
            UserDialogs.Instance.Prompt(config);
        }

        private void ClickMail(Mail mail)
        {
            if (mail == null)
                return;

            ActionSheetConfig config = new ActionSheetConfig();
            config.Add("Send", () =>
            {
                UserDialogs.Instance.Alert("Not implemented");
            });
            config.Add("Edit", () =>
            {
                EditMail(mail);
            });
            config.Add("Delete", () =>
            {
                if (Debtor == null || Debtor.Mails.IsNullOrEmpty())
                    return;

                Debtor.Mails.Remove(mail);
            });
            config.Add("Cancel");
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
            config.SetMessage("Set mail address");
            config.SetText(mail.Address);
            UserDialogs.Instance.Prompt(config);
        }
        #endregion
    }
}
