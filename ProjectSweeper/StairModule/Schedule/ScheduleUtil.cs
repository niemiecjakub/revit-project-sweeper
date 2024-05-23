using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule.Schedule
{
    public static class ScheduleUtil
    {
        public static XYZ GetAdaptiveFamilyXYZ(Element element)
        {
            LocationPoint location = element.Location as LocationPoint;
            return location.Point;
        }

        public static XYZ GetAdaptiveFamilyXYZ(Element element, Document doc)
        {
            IList<ElementId> placePointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(element as FamilyInstance);
            ReferencePoint point1 = doc.GetElement(placePointIds[0]) as ReferencePoint;
            XYZ p1XYZ = point1.Position;

            ReferencePoint point2 = doc.GetElement(placePointIds[1]) as ReferencePoint;
            XYZ p2XYZ = point2.Position;

            Line line = Line.CreateBound(p1XYZ, p2XYZ);
            //return line.Evaluate(0.5, true); ;
            return new XYZ((p1XYZ.X + p2XYZ.X) / 2, 0, 0);
        }


        public static XYZ GetFramingXYZ(Element element)
        {
            LocationCurve locationCurve = element.Location as LocationCurve;
            Curve curve = locationCurve.Curve;
            return curve.Evaluate(0.5, true);
        }

        public static XYZ GetHbeamXYZ(Element element)
        {
            LocationCurve locationCurve = element.Location as LocationCurve;
            Curve curve = locationCurve.Curve;
            return curve.Evaluate(0, true);
        }

        public static XYZ GetColumnXYZ(Element element)
        {
            LocationPoint location = element.Location as LocationPoint;
            return location.Point;
        }
    }
}
