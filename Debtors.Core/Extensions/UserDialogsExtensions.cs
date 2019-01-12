using Acr.UserDialogs;
using Debtors.Core.Interfaces;
using MvvmCross;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Debtors.Core.Extensions
{
    public static class UserDialogsExtensions
    {
        public static void ToastSucceed<T>(this T toast) where T : IUserDialogs
        {
            ToastConfig toastConfig = new ToastConfig("Saved");
            toastConfig.MessageTextColor = Color.Green;
            UserDialogs.Instance.Toast(toastConfig);
        }

        public static void ToastSucceed<T>(this T toast, string message) where T : IUserDialogs
        {
            ToastConfig toastConfig = new ToastConfig(message ?? "");
            toastConfig.MessageTextColor = Color.Green;
            UserDialogs.Instance.Toast(toastConfig);
        }

        public static void ToastFailure<T>(this T toast) where T : IUserDialogs
        {
            ToastConfig toastConfig = new ToastConfig("Error occured");
            toastConfig.MessageTextColor = Color.Red;
            UserDialogs.Instance.Toast(toastConfig);
        }

        public static void ToastFailure<T>(this T toast, string message) where T : IUserDialogs
        {
            ToastConfig toastConfig = new ToastConfig(message ?? "");
            toastConfig.MessageTextColor = Color.Red;
            UserDialogs.Instance.Toast(toastConfig);
        }

        public static void ConfirmDelete<T>(this T confirm, Action<bool> action) where T : IUserDialogs
        {
            IResourceService ResourceService = Mvx.IoCProvider.Resolve<IResourceService>();
            ConfirmConfig confirmConfig = new ConfirmConfig();
            confirmConfig.Message = ResourceService.GetText("reallyDelete");
            confirmConfig.OnAction = action;
            UserDialogs.Instance.Confirm(confirmConfig);
        }

        public static void Confirm<T>(this T confirm, string message, string confirmText, string cancelText, Action<bool> action) where T : IUserDialogs
        {
            ConfirmConfig confirmConfig = new ConfirmConfig();
            confirmConfig.Message = message;
            confirmConfig.OnAction = action;
            confirmConfig.OkText = confirmText;
            confirmConfig.CancelText = cancelText;
            UserDialogs.Instance.Confirm(confirmConfig);
        }
    }
}
