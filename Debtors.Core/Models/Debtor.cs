using MvvmCross.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Debtors.Core.Models
{
    public class Debtor : BaseModel
    {
        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value == null ? value : value.Trim(); }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set { lastName = value == null ? value : value.Trim(); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { description = value == null ? value : value.Trim(); }
        }

        [Ignore]
        public MvxObservableCollection<Phone> Phones { get; set; }
        [Ignore]
        public MvxObservableCollection<Mail> Mails { get; set; }
        [Ignore]
        public MvxObservableCollection<Debt> Debts { get; set; }
    }
}
