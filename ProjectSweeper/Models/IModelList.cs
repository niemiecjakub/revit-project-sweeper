using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public interface IModelList
    {
        Task<IEnumerable<IElement>> GetAllElements();
        void DeleteElements(IEnumerable<IElement> elements);
    }
}
