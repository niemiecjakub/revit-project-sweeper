﻿using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.ElementRemover
{
    public interface IRemoveElement
    {
        void Remove(IEnumerable<IElement> elementsToBeRemoved);
    }
}
