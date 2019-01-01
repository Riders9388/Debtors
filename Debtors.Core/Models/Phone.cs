using Debtors.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Models
{
    public class Phone : BaseModel
    {
        public int DebtorId { get; set; }
        public PhoneNumberType Type { get; set; }

        private string number;
        public string Number
        {
            get { return number; }
            set
            {
                number = value == null ? value : value.Trim();
                RaisePropertyChanged(() => Number);
            }
        }

    }
}
