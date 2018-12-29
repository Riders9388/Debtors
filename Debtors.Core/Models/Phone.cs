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
        public string Number { get; set; }
    }
}
