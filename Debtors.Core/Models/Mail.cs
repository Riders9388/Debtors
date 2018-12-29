using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Models
{
    public class Mail : BaseModel
    {
        public int DebtorId { get; set; }
        public string Address { get; set; }
    }
}
