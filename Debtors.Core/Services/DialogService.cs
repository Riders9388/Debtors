using Acr.UserDialogs;
using Debtors.Core.Enums;
using Debtors.Core.Interfaces;
using Debtors.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Debtors.Core.Services
{
	public class DialogService : IDialogService
	{
		private readonly IResourceService resourceService;
		private readonly ICustomDialogService customDialogService;
		public DialogService(IResourceService resourceService, ICustomDialogService customDialogService)
		{
			this.resourceService = resourceService;
			this.customDialogService = customDialogService;
		}

		public void ConfirmDialog(string message, string title, string okText, string cancelText, int styleId, bool useYesNo, Action<bool> onAction)
		{
			ConfirmConfig confirmConfig = new ConfirmConfig();
			confirmConfig.Message = message;

			if (okText != null)
				confirmConfig.OkText = okText;

			if (cancelText != null)
				confirmConfig.CancelText = cancelText;

			if (useYesNo)
				confirmConfig.UseYesNo();

			if (!string.IsNullOrEmpty(title))
				confirmConfig.Title = title;

			if (styleId > 0)
				confirmConfig.AndroidStyleId = styleId;

			if (onAction != null)
				confirmConfig.OnAction = onAction;
			else
				confirmConfig.OnAction = DumbAction;

			UserDialogs.Instance.Confirm(confirmConfig);
		}

		public void ConfirmDialog(string message, string okText, string cancelText, bool useYesNo, Action<bool> onAction)
		{
			ConfirmDialog(message, null, okText, cancelText, 0, useYesNo, onAction);
		}

		public void ConfirmDialog(string message, string title, Action<bool> onAction)
		{
			ConfirmDialog(message, title, null, null, 0, false, onAction);
		}

		public void ConfirmDialog(string message, bool useYesNo, Action<bool> onAction)
		{
			ConfirmDialog(message, null, null, null, 0, useYesNo, onAction);
		}

		public void ConfirmDialog(string message, Action<bool> onAction)
		{
			ConfirmDialog(message, null, null, null, 0, false, onAction);
		}

		public void ConfirmDialog(string message, string title, Action onConfirm)
		{
			ConfirmDialog(message, title, null, null, 0, false, confirm =>
			{
				if (confirm)
					onConfirm?.Invoke();
			});
		}

		public void ConfirmDialog(string message, Action onConfirm)
		{
			ConfirmDialog(message, null, null, null, 0, false, confirm =>
			{
				if (confirm)
					onConfirm?.Invoke();
			});
		}

		public void YesNoDialog(string message, Action okAction, Action noAction = null)
		{
			ConfirmDialog(message, true, yes =>
			{
				if (yes)
					okAction?.Invoke();
				else
					noAction?.Invoke();
			});
		}

		public void MessageDialog(string message, string title, int styleId, Action okAction = null)
		{
			AlertConfig config = new AlertConfig();
			config.Title = title;
			config.Message = message;
			config.OnAction = okAction;

			if (styleId > 0)
				config.AndroidStyleId = styleId;

			UserDialogs.Instance.Alert(config);
		}

		public void MessageDialog(string message, string title, Action okAction = null)
		{
			MessageDialog(message, title, 0, okAction);
		}

		public void MessageDialog(string message, Action okAction = null)
		{
			MessageDialog(message, null, 0, okAction);
		}

		public void InputDialog(string message, string title, string okText, string cancelText, int styleId, Action<PromptResult> onAction)
		{
			PromptConfig p = new PromptConfig();
			p.InputType = InputType.Number;

			if (okText != null)
				p.OkText = okText;

			if (cancelText != null)
				p.CancelText = cancelText;

			p.Message = message;
			p.Title = title ?? "";
			p.OnAction = onAction;

			if (styleId > 0)
				p.AndroidStyleId = styleId;

			UserDialogs.Instance.Prompt(p);
		}

		public void InputDialog(string message, string title, string okText, string cancelText, Action<PromptResult> onAction)
		{
			InputDialog(message, title, okText, cancelText, 0, onAction);
		}

		public void InputDialog(string message, string title, Action<PromptResult> onAction)
		{
			InputDialog(message, title, null, null, 0, onAction);
		}

		public void InputDialog(string message, Action<PromptResult> onAction)
		{
			InputDialog(message, null, null, null, 0, onAction);
		}

		public void ActionsDialog(string title, List<GridItem> actions)
		{
			customDialogService.Show(title, actions);
		}

		public void ActionsDialog(List<GridItem> actions)
		{
			customDialogService.Show(null, actions);
		}

		private void DumbAction(bool obj)
		{
			//Do or do not, there is no try
		}

		public async Task<bool> ConfirmDialogAsync(string message, string title, string okText, string cancelText, int styleId, bool useYesNo)
		{
			ConfirmConfig confirmConfig = new ConfirmConfig();
			confirmConfig.Message = message;

			if (okText != null)
				confirmConfig.OkText = okText;

			if (cancelText != null)
				confirmConfig.CancelText = cancelText;

			if (useYesNo)
				confirmConfig.UseYesNo();

			if (!string.IsNullOrEmpty(title))
				confirmConfig.Title = title;

			if (styleId > 0)
				confirmConfig.AndroidStyleId = styleId;

			return await UserDialogs.Instance.ConfirmAsync(confirmConfig);
		}

		public async Task<bool> ConfirmDialogAsync(string message, string okText, string cancelText, bool useYesNo)
		{
			return await ConfirmDialogAsync(message, null, okText, cancelText, 0, useYesNo);
		}

		public async Task<bool> ConfirmDialogAsync(string message, string title)
		{
			return await ConfirmDialogAsync(message, title, null, null, 0, false);
		}

		public async Task<bool> ConfirmDialogAsync(string message, bool useYesNo)
		{
			return await ConfirmDialogAsync(message, null, null, null, 0, useYesNo);
		}

		public async Task<bool> ConfirmDialogAsync(string message)
		{
			return await ConfirmDialogAsync(message, null, null, null, 0, false);
		}
		public async Task MessageDialogAsync(string message, string title, int styleId)
		{
			AlertConfig config = new AlertConfig();
			config.Title = title;
			config.Message = message;

			if (styleId > 0)
				config.AndroidStyleId = styleId;

			await UserDialogs.Instance.AlertAsync(config);
		}

		public async Task MessageDialogAsync(string message, string title)
		{
			await MessageDialogAsync(message, title, 0);
		}

		public async Task MessageDialogAsync(string message)
		{
			await MessageDialogAsync(message, null, 0);
		}

		public async Task<PromptResult> InputDialogAsync(string message, string title, string okText, string cancelText, InputType inputType, int styleId, string text)
		{
			PromptConfig p = new PromptConfig();
			p.InputType = inputType;

			if (okText != null)
				p.OkText = okText;

			if (cancelText != null)
				p.CancelText = cancelText;

			p.Message = message;
			p.Title = title ?? "";

			if (!string.IsNullOrEmpty(text))
				p.Text = text;

			if (styleId > 0)
				p.AndroidStyleId = styleId;

			return await UserDialogs.Instance.PromptAsync(p);
		}

		public async Task<PromptResult> InputDialogAsync(string message, string title, string okText, string cancelText, InputType inputType, int styleId)
		{
			return await InputDialogAsync(message, title, okText, cancelText, InputType.Number, 0, null);
		}

		public async Task<PromptResult> InputDialogAsync(string message, string title, string okText, string cancelText)
		{
			return await InputDialogAsync(message, title, okText, cancelText, InputType.Number, 0, null);
		}

		public async Task<PromptResult> InputDialogAsync(string message, string title)
		{
			return await InputDialogAsync(message, title, null, null, InputType.Number, 0, null);
		}

		public async Task<PromptResult> InputDialogAsync(string message, InputType inputType, string text)
		{
			return await InputDialogAsync(message, null, null, null, inputType, 0, text);
		}

		public async Task<PromptResult> InputDialogAsync(string message, InputType inputType)
		{
			return await InputDialogAsync(message, null, null, null, inputType, 0, null);
		}

		public async Task<PromptResult> InputDialogAsync(string message)
		{
			return await InputDialogAsync(message, null, null, null, InputType.Number, 0, null);
		}

		public async Task<GridItem> ActionsDialogAsync(string title, List<GridItem> actions)
		{
			return await customDialogService.ShowAsync(title, actions);
		}

		public async Task<GridItem> ActionsDialogAsync(List<GridItem> actions)
		{
			return await customDialogService.ShowAsync(actions);
		}

		public async Task<GridItem<T>> ActionsDialogAsync<T>(string title, List<GridItem<T>> actions, enumPlace gravity)
		{
			return await customDialogService.ShowAsync(title, actions, gravity);
		}

		public async Task<GridItem<T>> ActionsDialogAsync<T>(string title, List<GridItem<T>> actions)
		{
			return await customDialogService.ShowAsync(title, actions);
		}

		public async Task<GridItem<T>> ActionsDialogAsync<T>(List<GridItem<T>> actions)
		{
			return await customDialogService.ShowAsync(actions);
		}
	}
}
