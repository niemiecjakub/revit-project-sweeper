using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule
{
    public class SlantedColumnBuilder
    {

        public static FamilyInstance CreateSlantedColumn(Document doc, XYZ startPoint, XYZ endPoint, FamilySymbol columnSymbol)
        {
            Line columnAxis = Line.CreateBound(startPoint, endPoint);
            XYZ columnDirection = (endPoint - startPoint).Normalize();

            //doc.Create.NewModelCurve(columnAxis, Utils.CreateSketchPlaneByCurve(columnAxis, doc));
            Level datumLevel = doc.GetElement(Level.GetNearestLevelId(doc, 0)) as Level;

            StructuralType structuralType = StructuralType.Beam;
            FamilyInstance strElem = doc.Create.NewFamilyInstance(startPoint, columnSymbol, datumLevel, structuralType);

            //Parameter param = strElem.get_Parameter(BuiltInParameter.SLANTED_COLUMN_TYPE_PARAM);
            //param.Set((int)SlantedOrVerticalColumnType.CT_EndPoint);

            //LocationCurve strElemCurve = strElem.Location as LocationCurve;
            //if (strElemCurve != null)
            //{
            //    Line line = Line.CreateBound(startPoint, endPoint);
            //    strElemCurve.Curve = line;
            //}
            return strElem;
        }
    }
}
