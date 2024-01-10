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
        public LineStyleList(ILineStyleProvider lineStyleProvider)
        {
            _lineStyleProvider = lineStyleProvider;
        }

        public IEnumerable<LineStyle> GetAllLineStyles()
        {
            return _lineStyleProvider.GetAllElements();
        }

        public void DeleteLineStyle(LineStyle lineStyle)
        {
            Debug.WriteLine("Deleting line style");
        }
    }
}
