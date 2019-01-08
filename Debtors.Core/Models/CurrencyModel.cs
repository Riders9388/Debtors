using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Models
{
    public class CurrencyModel : BaseModel
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
