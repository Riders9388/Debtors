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

        public int InsertOrUpdateDebtor(Debtor debtor, bool newTransaction = true)
        {
            int toReturn = 0;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                if (debtor.Id > 0)
                    toReturn = Connection.Update(debtor);
                else
                    toReturn = Connection.Insert(debtor);

                InserOrUpdateAllDebtorPhones(debtor);
                InserOrUpdateAllDebtorMails(debtor);

                if (newTransaction && Connection.IsInTransaction)
                    Connection.Commit();
            }
            catch (Exception ex)
            {
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }

        public int RemoveDebtor(Debtor debtor, bool newTransaction = true)
        {
            int toReturn = 0;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                toReturn = Connection.Delete(debtor);

                if (newTransaction && Connection.IsInTransaction)
                    Connection.Commit();
            }
            catch (Exception ex)
            {
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }

        public int RemoveDebtor(int id, bool newTransaction = true)
        {
            int toReturn = 0;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                toReturn = Connection.Delete<Debtor>(id);

                if (newTransaction && Connection.IsInTransaction)
                    Connection.Commit();
            }
            catch (Exception ex)
            {
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

        public int InsertOrUpdatePhone(Phone phone, bool newTransaction = true)
        {
            int toReturn = 0;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                if (phone.Id > 0)
                    toReturn = Connection.Update(phone);
                else
                    toReturn = Connection.Insert(phone);

                if (newTransaction && Connection.IsInTransaction)
                    Connection.Commit();
            }
            catch (Exception ex)
            {
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
                    RemovePhone(item, false);
            }
        }

        public int RemovePhone(Phone phone, bool newTransaction = true)
        {
            int toReturn = 0;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                toReturn = Connection.Delete(phone);

                if (newTransaction && Connection.IsInTransaction)
                    Connection.Commit();
            }
            catch (Exception ex)
            {
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }

        public int RemovePhone(int id, bool newTransaction = true)
        {
            int toReturn = 0;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                toReturn = Connection.Delete<Phone>(id);

                if (newTransaction && Connection.IsInTransaction)
                    Connection.Commit();
            }
            catch (Exception ex)
            {
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

        public int InsertOrUpdateMail(Mail mail, bool newTransaction = true)
        {
            int toReturn = 0;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                if (mail.Id > 0)
                    toReturn = Connection.Update(mail);
                else
                    toReturn = Connection.Insert(mail);

                if (newTransaction && Connection.IsInTransaction)
                    Connection.Commit();
            }
            catch (Exception ex)
            {
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
                    RemoveMail(item, false);
            }
        }

        public int RemoveMail(Mail mail, bool newTransaction = true)
        {
            int toReturn = 0;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                toReturn = Connection.Delete(mail);

                if (newTransaction && Connection.IsInTransaction)
                    Connection.Commit();
            }
            catch (Exception ex)
            {
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }

        public int RemoveMail(int id, bool newTransaction = true)
        {
            int toReturn = 0;
            try
            {
                if (newTransaction && !Connection.IsInTransaction)
                    Connection.BeginTransaction();

                toReturn = Connection.Delete<Mail>(id);

                if (newTransaction && Connection.IsInTransaction)
                    Connection.Commit();
            }
            catch (Exception ex)
            {
                if (newTransaction && Connection.IsInTransaction)
                    Connection.Rollback();
            }
            return toReturn;
        }
        #endregion
    }
}
