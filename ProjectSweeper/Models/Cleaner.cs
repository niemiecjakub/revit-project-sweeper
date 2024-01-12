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
        private readonly FilledRegionModelList _filledRegionList;
        private readonly FillPatternModelList _fillPatternList;


        public Cleaner(LineStyleModelList lineStyleList, LinePatternModelList linePatternList, FilledRegionModelList filledRegionList, FillPatternModelList fillPatternList)
        {
            _lineStyleList = lineStyleList;
            _linePatternList = linePatternList;
            _filledRegionList = filledRegionList;
            _fillPatternList = fillPatternList;
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

        public async Task<IEnumerable<IElement>> GetAllFilledRegions()
        {
            return await _filledRegionList.GetAllFilledRegions();
        }
        public void FilledRegionDeleted(IEnumerable<IElement> filledRegions)
        {
            Debug.WriteLine("CLEANER: Inside cleaner");
            _filledRegionList.DeleteFilledRegion(filledRegions);
        }

        public async Task<IEnumerable<IElement>> GetAllFillPatterns()
        {
            return await _fillPatternList.GetAllFillPatterns();
        }
        public void FillPatternDeleted(IEnumerable<IElement> fillPatterns)
        {
            Debug.WriteLine("CLEANER: Inside cleaner");
            _fillPatternList.DeleteFillPattern(fillPatterns);
        }

    }
}
