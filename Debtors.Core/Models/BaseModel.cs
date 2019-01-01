using MvvmCross.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Models
{
    public class BaseModel : MvxNotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
