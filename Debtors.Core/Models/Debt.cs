﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Models
{
    public class Debt : BaseModel
    {
        public int DebtorId { get; set; }
        public decimal Value { get; set; }
    }
}