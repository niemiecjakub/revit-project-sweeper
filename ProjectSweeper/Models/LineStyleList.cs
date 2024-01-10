using ProjectSweeper.Services.ElementRemover;
using ProjectSweeper.Services.LineStyleProvider;
using ProjectSweeper.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class LineStyleList : ViewModelBase
    {
        private readonly ILineStyleProvider _lineStyleProvider;
        private readonly IRemoveElement _elementRemover;
        public LineStyleList(ILineStyleProvider lineStyleProvider, IRemoveElement elementRemover)
        {
            _lineStyleProvider = lineStyleProvider;
            _elementRemover = elementRemover;
        }

        public IEnumerable<LineStyle> GetAllLineStyles()
        {
            return _lineStyleProvider.GetAllElements();
        }

        public async Task DeleteLineStyle(LineStyle lineStyle)
        {
            Debug.WriteLine($"LINE STYLE LIST: {lineStyle.Name}");
            await _elementRemover.Remove(lineStyle.Id);
        }
    }
}
