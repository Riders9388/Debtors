using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Debtors.Core.Models
{
    public class Debtor : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }

        [Ignore]
        public List<Phone> Phones { get; set; }
        [Ignore]
        public List<Mail> Mails { get; set; }
        [Ignore]
        public List<Debt> Debts { get; set; }
    }
}
