using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Models
{
    public class Mail : BaseModel
    {
        public int DebtorId { get; set; }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value == null ? value : value.Trim();
                RaisePropertyChanged(() => Address);
            }
        }
    }
}
