﻿using System;
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
        private readonly Dictionary<ModelTypes, IModelList> _modelLists;

        public Cleaner(LineStyleModelList lineStyleList, LinePatternModelList linePatternList, FilledRegionModelList filledRegionList, FillPatternModelList fillPatternList)
        {
            _lineStyleList = lineStyleList;
            _linePatternList = linePatternList;
            _filledRegionList = filledRegionList;
            _fillPatternList = fillPatternList;
            _modelLists = new Dictionary<ModelTypes, IModelList>
                {
                    { ModelTypes.LineStyle, _lineStyleList },
                    { ModelTypes.LinePattern, _linePatternList },
                    { ModelTypes.FilledRegion, _filledRegionList },
                    { ModelTypes.FillPattern, _fillPatternList },
                };
        }

        public async Task<IEnumerable<IElement>> GetAllElements(ModelTypes modelType)
        {
            return await _modelLists[modelType].GetAllElements();
        }

        public void DeleteElements(ModelTypes modelType, IEnumerable<IElement> elements)
        {
            Debug.WriteLine("CLEANER: Inside cleaner");
            _modelLists[modelType].DeleteElements(elements);
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
