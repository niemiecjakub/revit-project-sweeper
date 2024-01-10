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
        private readonly LineStyleList _lineStyleList;
        public Cleaner(LineStyleList lineStyleList)
        {
            _lineStyleList = lineStyleList;
        }

        public IEnumerable<LineStyle> GetAllLineStyles()
        {
            return _lineStyleList.GetAllLineStyles();
        }
        public async Task LineStyleDeleted(LineStyle lineStyle)
        {
            Debug.WriteLine("CLEANER: Inside cleaner");
            await _lineStyleList.DeleteLineStyle(lineStyle);
        }

    }
}
