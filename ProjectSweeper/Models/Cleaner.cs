using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class Cleaner
    {
        private readonly ElementModelList _elementModelList;
        public Cleaner(ElementModelList elementModelList)
        {
            _elementModelList = elementModelList;
        }

        public async Task<IEnumerable<IElement>> GetAllElements(ModelTypes modelType)
        {
            return await _elementModelList.GetAllElements(modelType);
        }

        public void DeleteElements(IEnumerable<IElement> elementsToBeDeleted)
        {
            _elementModelList.DeleteElements(elementsToBeDeleted);
        }
    }
}
