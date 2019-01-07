﻿using Debtors.Core.Extensions;
using MvvmCross.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        private byte[] image;
        public byte[] Image
        {
            get { return image; }
            set
            {
                image = value;
                RaisePropertyChanged(() => Image);
            }
        }


        [Ignore]
        public MvxObservableCollection<Phone> Phones { get; set; }
        [Ignore]
        public MvxObservableCollection<Mail> Mails { get; set; }
        [Ignore]
        public MvxObservableCollection<Debt> Debts { get; set; }
        [Ignore]
        public Dictionary<string, decimal> DebtsValuses { get; set; }
        [Ignore]
        public string DebtsValuesText
        {
            get
            {
                if (DebtsValuses.IsNullOrEmpty())
                    return "No debts";

                return string.Join("\n", DebtsValuses.Select(x => x.Key + x.Value));
            }
        }
    }
}
