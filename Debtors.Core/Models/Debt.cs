using MvvmCross.UI;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Debtors.Core.Models
{
    public class Debt : BaseModel
    {
        public int DebtorId { get; set; }
        public decimal? Value { get; set; }
        public int CurrencyId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public decimal PaidBackValue { get; set; }
        public decimal MissingBackValue { get; set; }
        [Ignore]
        public List<DebtBack> ValuesBack { get; set; }

        private Currency currency;
        [Ignore]
        public Currency Currency
        {
            get { return currency; }
            set
            {
                currency = value;
                RaisePropertyChanged(() => Currency);
            }
        }

        [Ignore]
        public MvxColor Color
        {
            get
            {
                if (MissingBackValue <= decimal.Zero)
                    return new MvxColor(50, 205, 50);
                else if (MissingBackValue < Value)
                    return new MvxColor(255, 140, 0);

                return new MvxColor(220, 20, 60);
            }
        }
    }
}
