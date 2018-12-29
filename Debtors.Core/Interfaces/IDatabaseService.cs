using Debtors.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Interfaces
{
    public interface IDatabaseService
    {
        List<Debtor> GetDebtors();
        int InsertOrUpdateDebtor(Debtor debtor);
        int RemoveDebtor(Debtor debtor);
        int RemoveDebtor(int debtorId);
    }
}
