using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class Cleaner
    {
        private readonly LineStyleList _lineStyleList;
        public Cleaner(LineStyleList lineStyleList)
        {
            _lineStyleList = lineStyleList;
        }

        public IEnumerable<LineStyle> GetAllLineStyles()
        {
            return _lineStyleList.GetAllLineStyles();
        }
        public void LineStyleDeleted(LineStyle lineStyle)
        {
            _lineStyleList.DeleteLineStyle(lineStyle);
        }

    }
}
