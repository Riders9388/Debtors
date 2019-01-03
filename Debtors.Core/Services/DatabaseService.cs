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
        public DatabaseService()
        {
            Connection = new SQLiteConnection(StaticHelper.DatabasePath);
            CreateTables();
        }

        private void CreateTables()
        {
            Connection.CreateTable<Phone>();
            Connection.CreateTable<Mail>();
            Connection.CreateTable<Debt>();
            Connection.CreateTable<Debtor>();
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

            }
            return toReturn;
        }

        public Debtor GetDebtor(int id)
        {
            Debtor debtor = new Debtor();
            try
            {
                debtor = Connection.Get<Debtor>(id);
                GetAdditionalDebtorElements(ref debtor);
            }
            catch (Exception ex)
            {

            }
            return debtor;
        }

        private void GetAdditionalDebtorElements(ref Debtor debtor)
        {
            if (debtor == null)
                return;

            debtor.Phones = new MvxObservableCollection<Phone>(GetPhones(debtor.Id));
            debtor.Mails = new MvxObservableCollection<Mail>(GetMails(debtor.Id));
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

                InserOrUpdateAllDebtorPhones(debtor);
                InserOrUpdateAllDebtorMails(debtor);

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
                RemovePhonesByDebtorId(id, false);
                RemoveMailsByDebtorId(id, false);

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
                toReturn = false;
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }

        private void InserOrUpdateAllDebtorPhones(Debtor debtor)
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
                toReturn = false;
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }

        private bool RemovePhonesByDebtorId(int id, bool newTransaction = true)
        {
            bool toReturn = false;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                int succeed = Connection.Table<Phone>().Delete(x => x.DebtorId == id);

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
                toReturn = false;
                if(newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }

        private void InserOrUpdateAllDebtorMails(Debtor debtor)
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
                toReturn = false;
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }

        private bool RemoveMailsByDebtorId(int id, bool newTransaction = true)
        {
            bool toReturn = false;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                int succeed = Connection.Table<Mail>().Delete(x => x.DebtorId == id);

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
                toReturn = Connection.Table<Debt>().Where(x => x.DebtorId == debtorId).ToList();
            }
            catch (Exception ex)
            {

            }
            return toReturn;
        }

        public Debt GetDebt(int id)
        {
            Debt toReturn = new Debt();
            try
            {
                toReturn = Connection.Get<Debt>(id);
            }
            catch (Exception ex)
            {

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
                toReturn = false;
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }

        private bool RemoveDebtssByDebtorId(int id, bool newTransaction = true)
        {
            bool toReturn = false;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                int succeed = Connection.Table<Debt>().Delete(x => x.DebtorId == id);

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
                toReturn = false;
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }
        #endregion
    }
}
