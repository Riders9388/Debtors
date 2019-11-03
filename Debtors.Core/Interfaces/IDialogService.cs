using Acr.UserDialogs;
using Debtors.Core.Enums;
using Debtors.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Debtors.Core.Interfaces
{
	public interface IDialogService
	{
		void ConfirmDialog(string message, string title, string okText, string cancelText, int styleId, bool useYesNo, Action<bool> onAction);
		void ConfirmDialog(string message, string okText, string cancelText, bool useYesNo, Action<bool> onAction);
		void ConfirmDialog(string message, string title, Action<bool> onAction);
		void ConfirmDialog(string message, Action<bool> onAction);
		void ConfirmDialog(string message, string title, Action onConfirm);
		void ConfirmDialog(string message, bool useYesNo, Action<bool> onAction);
		void ConfirmDialog(string message, Action onConfirm);
		void YesNoDialog(string message, Action yesAction, Action noAction = null);
		void MessageDialog(string message, string title, int styleId, Action okAction = null);
		void MessageDialog(string message, string title, Action okAction = null);
		void MessageDialog(string message, Action okAction = null);
		void InputDialog(string message, string title, string okText, string cancelText, int styleId, Action<PromptResult> onAction);
		void InputDialog(string message, string title, string okText, string cancelText, Action<PromptResult> onAction);
		void InputDialog(string message, string title, Action<PromptResult> onAction);
		void InputDialog(string message, Action<PromptResult> onAction);
		void ActionsDialog(string title, List<GridItem> actions);
		void ActionsDialog(List<GridItem> actions);

		Task<bool> ConfirmDialogAsync(string message, string title, string okText, string cancelText, int styleId, bool useYesNo);
		Task<bool> ConfirmDialogAsync(string message, string okText, string cancelText, bool useYesNo);
		Task<bool> ConfirmDialogAsync(string message, string title);
		Task<bool> ConfirmDialogAsync(string message, bool useYesNo);
		Task<bool> ConfirmDialogAsync(string message);
		Task MessageDialogAsync(string message, string title, int styleId);
		Task MessageDialogAsync(string message, string title);
		Task MessageDialogAsync(string message);
		Task<PromptResult> InputDialogAsync(string message, string title, string okText, string cancelText, InputType inputType, int styleId, string text);
		Task<PromptResult> InputDialogAsync(string message, string title, string okText, string cancelText, InputType inputType, int styleId);
		Task<PromptResult> InputDialogAsync(string message, string title, string okText, string cancelText);
		Task<PromptResult> InputDialogAsync(string message, string title);
		Task<PromptResult> InputDialogAsync(string message, InputType inputType, string text);
		Task<PromptResult> InputDialogAsync(string message, InputType inputType);
		Task<PromptResult> InputDialogAsync(string message);
		Task<GridItem> ActionsDialogAsync(string title, List<GridItem> actions);
		Task<GridItem> ActionsDialogAsync(List<GridItem> actions);
		Task<GridItem<T>> ActionsDialogAsync<T>(string title, List<GridItem<T>> actions, enumPlace gravity);
		Task<GridItem<T>> ActionsDialogAsync<T>(string title, List<GridItem<T>> actions);
		Task<GridItem<T>> ActionsDialogAsync<T>(List<GridItem<T>> actions);
	}
}
