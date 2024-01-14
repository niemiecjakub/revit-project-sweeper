﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.ViewModels
{
    public interface IPageViewModel : INotifyPropertyChanged
    {
        bool IsActive { get; set; }
    }
}
