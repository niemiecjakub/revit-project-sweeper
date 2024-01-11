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
        private readonly LineStyleModelList _lineStyleList;
        private readonly LinePatternModelList _linePatternList;

        public Cleaner(LineStyleModelList lineStyleList, LinePatternModelList linePatternList)
        {
            _lineStyleList = lineStyleList;
            _linePatternList = linePatternList;
        }

        public async Task<IEnumerable<LineStyleModel>> GetAllLineStyles()
        {
            return await _lineStyleList.GetAllLineStyles();
        }
        public async Task LineStyleDeleted(IEnumerable<LineStyleModel> lineStyles)
        {
            Debug.WriteLine("CLEANER: Inside cleaner");
            await _lineStyleList.DeleteLineStyle(lineStyles);
        }



        public async Task<IEnumerable<LinePatternModel>> GetAllLinePatterns()
        {
            return await _linePatternList.GetAllLinePatterns();
        }
        public async Task LinePatternDeleted(IEnumerable<LinePatternModel> linePatterns)
        {
            Debug.WriteLine("CLEANER: Inside cleaner");
            await _linePatternList.DeleteLinePatetrn(linePatterns);
        }

    }
}
