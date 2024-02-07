using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ProjectSweeper.StairModule
{
    public class SupportColumnBuilder
    {
        public static FamilyInstance CreateColumnByIntersection(Document doc, Line placementLine, FamilySymbol familySymbol)
        {
            Level datumLevel = doc.GetElement(Level.GetNearestLevelId(doc, 0)) as Level;
            FamilyInstance column = doc.Create.NewFamilyInstance(placementLine.GetEndPoint(0), familySymbol, datumLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);

            FilteredElementCollector basePointCollector = new FilteredElementCollector(doc).OfClass(typeof(BasePoint));

            BasePoint projectBasePoint = basePointCollector.Cast<BasePoint>().FirstOrDefault();
            double projectBasePointElevation = projectBasePoint.Position.Z;
            //Debug.WriteLine("PBP: " + Utils.FeetToMMConverter(projectBasePointElevation));

            Parameter baseOffset = column.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);
            Parameter topOffset = column.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);

            baseOffset.Set(placementLine.GetEndPoint(0).Z - projectBasePointElevation);
            topOffset.Set(placementLine.GetEndPoint(1).Z - projectBasePointElevation);

            return column;
        }

        public static void Rotate(FamilyInstance column, Line axisLine, double RotationAngleInDegrees, string side)
        {
            RotationAngleInDegrees = side == "R" ? RotationAngleInDegrees : -RotationAngleInDegrees;
            Location locationCurve = column.Location;

            if (locationCurve != null)
            {
                XYZ rotationPoint = (axisLine.GetEndPoint(0) + axisLine.GetEndPoint(1)) / 2.0;
                ElementTransformUtils.RotateElement(column.Document, column.Id, axisLine, RotationAngleInDegrees * (Math.PI / 180.0));
            }
        }
    }
}
