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

        public async Task<IEnumerable<LineStyle>> GetAllLineStyles()
        {
            return await _lineStyleList.GetAllLineStyles();
        }
        public async Task LineStyleDeleted(IEnumerable<LineStyle> lineStyle)
        {
            Debug.WriteLine("CLEANER: Inside cleaner");
            await _lineStyleList.DeleteLineStyle(lineStyle);
        }

    }
}
