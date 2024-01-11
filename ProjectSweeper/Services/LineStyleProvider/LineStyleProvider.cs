﻿using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using ProjectSweeper.RevitFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Services.LineStyleProvider
{
    public class LineStyleProvider : ILineStyleProvider
    {
        private readonly Document _doc;

        public LineStyleProvider(Document doc)
        {
            _doc = doc;
        }

        public async Task<IEnumerable<LineStyle>> GetAllElements()
        {
            Debug.WriteLine("Getting all linestyles in provider");
            IEnumerable<LineStyle> lineStyles = LineFunctions.GetLineStyles(_doc);

            foreach (LineStyle lineStyle in lineStyles)
            {
                string name = lineStyle.Name;
                bool canBeRemoved = lineStyle.CanBeRemoved;

                //Debug.WriteLine($"{name} -- can be removed? == {canBeRemoved}");
            }

            return lineStyles;
        }
    }
}
