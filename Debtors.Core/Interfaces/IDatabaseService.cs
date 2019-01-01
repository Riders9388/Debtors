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
        int InsertOrUpdateDebtor(Debtor debtor, bool newTransaction = true);
        int RemoveDebtor(Debtor debtor, bool newTransaction = true);
        int RemoveDebtor(int debtorId, bool newTransaction = true);
        #endregion

        #region Phone
        List<Phone> GetPhones(int debtorId);
        Phone GetPhone(int id);
        int InsertOrUpdatePhone(Phone phone, bool newTransaction = true);
        int RemovePhone(Phone phone, bool newTransaction = true);
        int RemovePhone(int id, bool newTransaction = true);
        #endregion

        #region Mail
        List<Mail> GetMails(int debtorId);
        Mail GetMail(int id);
        int InsertOrUpdateMail(Mail mail, bool newTransaction = true);
        int RemoveMail(Mail mail, bool newTransaction = true);
        int RemoveMail(int id, bool newTransaction = true); 
        #endregion
    }
}
