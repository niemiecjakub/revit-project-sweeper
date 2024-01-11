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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IElement>> GetAllLineStyles()
        {
            return await _lineStyleList.GetAllLineStyles();
        }
        public void LineStyleDeleted(IEnumerable<IElement> lineStyles)
        {
            Debug.WriteLine("CLEANER: Inside cleaner");
            _lineStyleList.DeleteLineStyle(lineStyles);
        }



        public async Task<IEnumerable<IElement>> GetAllLinePatterns()
        {
            return await _linePatternList.GetAllLinePatterns();
        }
        public void LinePatternDeleted(IEnumerable<IElement> linePatterns)
        {
            Debug.WriteLine("CLEANER: Inside cleaner");
            _linePatternList.DeleteLinePatetrn(linePatterns);
        }

    }
}
