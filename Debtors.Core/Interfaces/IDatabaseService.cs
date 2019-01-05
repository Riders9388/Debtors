using Debtors.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Interfaces
{
    public interface IDatabaseService
    {
        #region Debtor
        List<Debtor> GetDebtors();
        Debtor GetDebtor(int id);
        bool InsertOrUpdateDebtor(Debtor debtor, bool newTransaction = true);
        bool RemoveDebtor(int debtorId, bool newTransaction = true);
        #endregion

        #region Phone
        List<Phone> GetPhones(int debtorId);
        Phone GetPhone(int id);
        bool InsertOrUpdatePhone(Phone phone, bool newTransaction = true);
        bool RemovePhone(int id, bool newTransaction = true);
        #endregion

        #region Mail
        List<Mail> GetMails(int debtorId);
        Mail GetMail(int id);
        bool InsertOrUpdateMail(Mail mail, bool newTransaction = true);
        bool RemoveMail(int id, bool newTransaction = true);
        #endregion

        #region Debts
        List<Debt> GetDebts(int debtorId);
        Debt GetDebt(int id);
        bool InsertOrUpdateDebt(Debt debt, bool newTransaction = true);
        bool RemoveDebt(int id, bool newTransaction = true);
        #endregion

        #region DebtsBack
        List<DebtBack> GetDebtsBack(int debtId);
        DebtBack GetDebtBack(int id);
        bool InsertOrUpdateDebtBack(DebtBack debtBack, bool newTransaction = true);
        bool RemoveDebtBack(int id, bool newTransaction = true);
        #endregion
    }
}
