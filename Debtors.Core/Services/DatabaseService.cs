using Debtors.Core.Extensions;
using Debtors.Core.Helpers;
using Debtors.Core.Interfaces;
using Debtors.Core.Models;
using MvvmCross.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Debtors.Core.Services
{
	public class DatabaseService : IDatabaseService
	{
		private readonly SQLiteConnection Connection;
		private readonly ILogService logService;

		public DatabaseService(ILogService logService)
		{
			this.logService = logService;

			Connection = new SQLiteConnection(StaticHelper.DatabasePath);
			CreateTables();
		}

		private void CreateTables()
		{
			Connection.CreateTable<Phone>();
			Connection.CreateTable<Mail>();
			Connection.CreateTable<DebtBack>();
			Connection.CreateTable<Debt>();
			Connection.CreateTable<Debtor>();
			Connection.CreateTable<Currency>();
		}

		#region Debtor
		public List<Debtor> GetDebtors()
		{
			List<Debtor> toReturn = new List<Debtor>();
			try
			{
				List<int> tempList = Connection.Table<Debtor>().Select(x => x.Id).ToList();
				foreach (var item in tempList)
					toReturn.Add(GetDebtor(item));
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public Debtor GetDebtor(int id)
		{
			Debtor debtor = new Debtor();
			try
			{
				debtor = Connection.Get<Debtor>(id);
				debtor.Phones = new MvxObservableCollection<Phone>(GetPhones(debtor.Id));
				debtor.Mails = new MvxObservableCollection<Mail>(GetMails(debtor.Id));

				List<Debt> debts = Connection.Table<Debt>().Where(x => x.DebtorId == debtor.Id && x.MissingBackValue > decimal.Zero).ToList();
				if (!debts.IsNullOrEmpty())
				{
					debtor.DebtsValuses = new Dictionary<string, decimal>();
					var currencies = debts.Select(x => x.Currency).Distinct();
					foreach (var currency in currencies)
						debtor.DebtsValuses.Add(currency.Symbol, debts.Where(x => x.Currency == currency).Sum(x => x.MissingBackValue));
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return debtor;
		}

		public bool InsertOrUpdateDebtor(Debtor debtor, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = 0;
				if (debtor.Id > 0)
				{
					debtor.ModifiedAt = DateTime.Now;
					succeed = Connection.Update(debtor);
				}
				else
				{
					debtor.CreatedAt = DateTime.Now;
					debtor.ModifiedAt = DateTime.Now;
					succeed = Connection.Insert(debtor);
				}

				InsertOrUpdateAllDebtorPhones(debtor);
				InsertOrUpdateAllDebtorMails(debtor);

				if (succeed > 0)
					toReturn = true;

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		public bool RemoveDebtor(int id, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = Connection.Delete<Debtor>(id);
				
				if (succeed > 0)
					toReturn = true;

				toReturn = toReturn && RemovePhonesByDebtorId(id, false);
				toReturn = toReturn && RemoveMailsByDebtorId(id, false);
				toReturn = toReturn && RemoveDebtsByDebtorId(id, false);

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}
		#endregion

		#region Phone
		public List<Phone> GetPhones(int debtorId)
		{
			List<Phone> toReturn = new List<Phone>();
			try
			{
				toReturn = Connection.Table<Phone>().Where(x => x.DebtorId == debtorId).ToList();
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public Phone GetPhone(int id)
		{
			Phone toReturn = new Phone();
			try
			{
				toReturn = Connection.Get<Phone>(id);
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public bool InsertOrUpdatePhone(Phone phone, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = 0;
				if (phone.Id > 0)
					succeed = Connection.Update(phone);
				else
					succeed = Connection.Insert(phone);

				if (succeed > 0)
					toReturn = true;

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		private void InsertOrUpdateAllDebtorPhones(Debtor debtor)
		{
			if (debtor == null || debtor.Id <= 0)
				return;

			List<Phone> list = GetPhones(debtor.Id);
			if (!debtor.Phones.IsNullOrEmpty())
			{
				foreach (var item in debtor.Phones)
				{
					item.DebtorId = debtor.Id;
					InsertOrUpdatePhone(item, false);
				}
			}

			if (!list.IsNullOrEmpty())
			{
				List<Phone> tempList = list;
				if (!debtor.Phones.IsNullOrEmpty())
					tempList = list.Where(x => !debtor.Phones.Any(y => y.Id == x.Id)).ToList();

				foreach (var item in tempList)
					RemovePhone(item.Id, false);
			}
		}

		public bool RemovePhone(int id, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = Connection.Delete<Phone>(id);

				if (succeed > 0)
					toReturn = true;

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		private bool RemovePhonesByDebtorId(int id, bool newTransaction = true)
		{
			bool toReturn = true;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				var phones = GetPhones(id);
				if (!phones.IsNullOrEmpty())
					foreach (var phone in phones)
						toReturn = toReturn && RemovePhone(phone.Id, false);

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}
		#endregion

		#region Mail
		public List<Mail> GetMails(int debtorId)
		{
			List<Mail> toReturn = new List<Mail>();
			try
			{
				toReturn = Connection.Table<Mail>().Where(x => x.DebtorId == debtorId).ToList();
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public Mail GetMail(int id)
		{
			Mail toReturn = new Mail();
			try
			{
				toReturn = Connection.Get<Mail>(id);
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public bool InsertOrUpdateMail(Mail mail, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = 0;
				if (mail.Id > 0)
					succeed = Connection.Update(mail);
				else
					succeed = Connection.Insert(mail);

				if (succeed > 0)
					toReturn = true;

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if(newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		private void InsertOrUpdateAllDebtorMails(Debtor debtor)
		{
			if (debtor == null || debtor.Id <= 0)
				return;

			List<Mail> list = GetMails(debtor.Id);
			if (!debtor.Mails.IsNullOrEmpty())
			{
				foreach (var item in debtor.Mails)
				{
					item.DebtorId = debtor.Id;
					InsertOrUpdateMail(item, false);
				}
			}

			if (!list.IsNullOrEmpty())
			{
				List<Mail> tempList = list;
				if (!debtor.Mails.IsNullOrEmpty())
					tempList = list.Where(x => !debtor.Mails.Any(y => y.Id == x.Id)).ToList();

				foreach (var item in tempList)
					RemoveMail(item.Id, false);
			}
		}

		public bool RemoveMail(int id, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = Connection.Delete<Mail>(id);

				if (succeed > 0)
					toReturn = true;

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		private bool RemoveMailsByDebtorId(int id, bool newTransaction = true)
		{
			bool toReturn = true;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				var mails = GetMails(id);
				if (!mails.IsNullOrEmpty())
					foreach (var mail in mails)
						toReturn = toReturn && RemoveMail(mail.Id, false);

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}
		#endregion

		#region Debts
		public List<Debt> GetDebts(int debtorId)
		{
			List<Debt> toReturn = new List<Debt>();
			try
			{
				List<int> debts = Connection.Table<Debt>().Where(x => x.DebtorId == debtorId).Select(x => x.Id).ToList();
				foreach (var id in debts)
					toReturn.Add(GetDebt(id));
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public Debt GetDebt(int id)
		{
			Debt toReturn = new Debt();
			try
			{
				toReturn = Connection.Get<Debt>(id);
				toReturn.Currency = GetCurrency(toReturn.CurrencyId);
				toReturn.ValuesBack = GetDebtsBack(toReturn.Id);
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public bool InsertOrUpdateDebt(Debt debt, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				debt.CurrencyId = debt.Currency?.Id ?? debt.CurrencyId;

				decimal sum = decimal.Zero;
				if (!debt.ValuesBack.IsNullOrEmpty())
					sum = debt.ValuesBack.Sum(x => x.Value);

				decimal missing = (debt.Value ?? 0) - sum;
				debt.PaidBackValue = sum;
				debt.MissingBackValue = missing < decimal.Zero ? decimal.Zero : missing;

				int succeed = 0;
				if (debt.Id > 0)
				{
					debt.ModifiedAt = DateTime.Now;
					succeed = Connection.Update(debt);
				}
				else
				{
					debt.CreatedAt = DateTime.Now;
					debt.ModifiedAt = DateTime.Now;
					succeed = Connection.Insert(debt);
				}

				if (succeed > 0)
					toReturn = true;

				toReturn = toReturn && InsertOrUpdateAllDebtBackValues(debt);

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		public bool RemoveDebt(int id, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = Connection.Delete<Debt>(id);

				if (succeed > 0)
					toReturn = true;

				toReturn = toReturn && RemoveDebtsBackByDebtId(id, false);

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		private bool RemoveDebtsByDebtorId(int id, bool newTransaction = true)
		{
			bool toReturn = true;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				var debts = GetDebts(id);
				if (!debts.IsNullOrEmpty())
					foreach (var debt in debts)
						toReturn = toReturn && RemoveDebt(debt.Id, false);

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}
		#endregion

		#region DebtsBack
		public List<DebtBack> GetDebtsBack(int debtId)
		{
			List<DebtBack> toReturn = new List<DebtBack>();
			try
			{
				toReturn = Connection.Table<DebtBack>().Where(x => x.DebtId == debtId).ToList();
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public DebtBack GetDebtBack(int id)
		{
			DebtBack toReturn = new DebtBack();
			try
			{
				toReturn = Connection.Get<DebtBack>(id);
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public bool InsertOrUpdateDebtBack(DebtBack debtBack, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = 0;
				if (debtBack.Id > 0)
				{
					debtBack.ModifiedAt = DateTime.Now;
					succeed = Connection.Update(debtBack);
				}
				else
				{
					debtBack.CreatedAt = DateTime.Now;
					debtBack.ModifiedAt = DateTime.Now;
					succeed = Connection.Insert(debtBack);
				}

				if (succeed > 0)
					toReturn = true;

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		private bool InsertOrUpdateAllDebtBackValues(Debt debt)
		{
			bool toReturn = true;
			if (debt == null || debt.Id <= 0)
				return toReturn;

			List<DebtBack> list = GetDebtsBack(debt.Id);
			if (!debt.ValuesBack.IsNullOrEmpty())
			{
				foreach (var item in debt.ValuesBack)
				{
					item.DebtId = debt.Id;
					toReturn = toReturn && InsertOrUpdateDebtBack(item, false);
				}
			}

			if (!list.IsNullOrEmpty())
			{
				List<DebtBack> tempList = list;
				if (!debt.ValuesBack.IsNullOrEmpty())
					tempList = list.Where(x => !debt.ValuesBack.Any(y => y.Id == x.Id)).ToList();

				foreach (var item in tempList)
					toReturn = toReturn && RemoveDebtBack(item.Id, false);
			}

			return toReturn;
		}

		public bool RemoveDebtBack(int id, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = Connection.Delete<DebtBack>(id);

				if (succeed > 0)
					toReturn = true;

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		private bool RemoveDebtsBackByDebtId(int id, bool newTransaction = true)
		{
			bool toReturn = true;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				var debtsBack = GetDebtsBack(id);
				if (!debtsBack.IsNullOrEmpty())
					foreach (var debtBack in debtsBack)
						toReturn = toReturn && RemoveDebtBack(debtBack.Id, false);

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}
		#endregion

		#region Currency
		public List<Currency> GetCurrencies()
		{
			List<Currency> toReturn = new List<Currency>();
			try
			{
				toReturn = Connection.Table<Currency>().ToList();
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public Currency GetCurrency(int id)
		{
			Currency toReturn = new Currency();
			try
			{
				toReturn = Connection.Get<Currency>(id);
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}

		public bool InsertOrUpdateCurrency(Currency currency, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = 0;
				if (currency.Id > 0)
					succeed = Connection.Update(currency);
				else
					succeed = Connection.Insert(currency);

				if (succeed > 0)
					toReturn = true;

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		public bool RemoveCurrency(int id, bool newTransaction = true)
		{
			bool toReturn = false;
			try
			{
				if (newTransaction && !Connection.IsInTransaction)
					Connection.BeginTransaction();

				int succeed = Connection.Delete<Currency>(id);

				if (succeed > 0)
					toReturn = true;

				if (newTransaction && Connection.IsInTransaction)
				{
					if (toReturn)
						Connection.Commit();
					else
						Connection.Rollback();
				}
			}
			catch (Exception ex)
			{
				logService.Error(ex);
				toReturn = false;
				if (newTransaction && Connection.IsInTransaction)
					Connection.Rollback();
			}
			return toReturn;
		}

		public bool IsCurrencyInUse(int currencyId)
		{
			bool toReturn = true;
			try
			{
				toReturn = Connection.Table<Debt>().Any(x => x.CurrencyId == currencyId);
			}
			catch (Exception ex)
			{
				logService.Error(ex);
			}
			return toReturn;
		}
		#endregion
	}
}
