using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule
{
    public class Utils
    {
        public static Line Reverse(Line line)
        {
            XYZ startPoint = line.GetEndPoint(0);
            XYZ endPoint = line.GetEndPoint(1);
            return Line.CreateBound(endPoint, startPoint);
        }
        public static string GetStairCode(Parameter walkwayOrientation, Parameter walkwayPurpose)
        {
            string orientation = walkwayOrientation.AsString() == "S" ? "S" : "N";
            string purpose = walkwayPurpose.AsString() == "Emergency" ? "E" : "M";
            return $"{orientation}T{purpose}L";
        }
        public static string ToNumberingFormat(int i)
        {
            if (i < 10)
            {
                return $"00{i}";
            }
            if (i < 100)
            {
                return $"0{i}";
            }
            return $"{i}";
        }

        public static double MMToFeetConverter(double value)
        {
            return value / 304.8;
        }

        public static double FeetToMMConverter(double value)
        {
            return value * 304.8;
        }
        public static Line TrimStartEndByValue(Line line, double startTrimValue, double endTrimValue)
        {
            XYZ startPoint = line.GetEndPoint(0);
            XYZ endPoint = line.GetEndPoint(1);

            XYZ trimmedStartPoint = startPoint + line.Direction * startTrimValue;
            XYZ trimmedEndPoint = endPoint - line.Direction * endTrimValue;

            return Line.CreateBound(trimmedStartPoint, trimmedEndPoint);

        }
        public static XYZ OffsetZValueXYZ(XYZ xyz, double zOffsetValue)
        {
            return new XYZ(xyz.X, xyz.Y, xyz.Z + zOffsetValue);
        }

        public static FamilySymbol GetFamilySymbolByName(string name, BuiltInCategory builtInCategory, Document doc)
        {
            var collector = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(builtInCategory)
                .WhereElementIsElementType()
                .Cast<FamilySymbol>()
                .Where(symbol => symbol.Name.Equals(name));

            return collector.FirstOrDefault();
        }

        public static SketchPlane CreateSketchPlaneByCurve(Curve curve, Document doc)
        {
            XYZ startXYZ = curve.GetEndPoint(0);
            XYZ endXYZ = curve.GetEndPoint(1);
            XYZ middleXYZ = new XYZ((startXYZ.X + endXYZ.X) / 2, (startXYZ.Y + endXYZ.Y) / 2, (startXYZ.Z + endXYZ.Z) / 2 + 300);
            Plane plane = Plane.CreateByThreePoints(startXYZ, middleXYZ, endXYZ);
            return SketchPlane.Create(doc, plane);
        }

        public static Plane CreatePlaneByCurve(Curve curve, Document doc)
        {
            XYZ startXYZ = curve.GetEndPoint(0);
            XYZ endXYZ = curve.GetEndPoint(1);
            XYZ middleXYZ = new XYZ((startXYZ.X + endXYZ.X) / 2, (startXYZ.Y + endXYZ.Y) / 2, (startXYZ.Z + endXYZ.Z) / 2 + 300);
            return Plane.CreateByThreePoints(startXYZ, middleXYZ, endXYZ);
        }

        public static Line CreateVerticalCurveFromXYZ(XYZ startXYZ, int zOffset)
        {
            XYZ endXYZ = new XYZ(startXYZ.X, startXYZ.Y, startXYZ.Z - zOffset);
            return Line.CreateBound(startXYZ, endXYZ);
        }

        public static List<Solid> GetSolidFromElement(Reference elementReference, Document doc)
        {
            ElementId elementId = elementReference.ElementId;
            Element element = doc.GetElement(elementId);

            List<Solid> solids = new List<Solid>();
            Options options = new Options();
            options.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement geometryElement = element.get_Geometry(options);
            foreach (GeometryObject geomObj in geometryElement)
            {
                if (geomObj is Solid solid && solid.Volume > 0)
                {
                    solids.Add(solid);
                }
            }
            return solids;
        }
        public static Curve GetCurveFromEdge(Reference pickedEdge, Document doc)
        {
            ElementId edgeId = pickedEdge.ElementId;
            Element edgeElement = doc.GetElement(edgeId);
            Edge edge = edgeElement.GetGeometryObjectFromReference(pickedEdge) as Edge;
            return edge.AsCurve();
        }

        public static Line LineOffset(Line lineToOffset, double distance)
        {
            XYZ lineDirection = lineToOffset.Direction;
            XYZ normal = XYZ.BasisZ.CrossProduct(lineDirection).Normalize();
            XYZ translation = normal.Multiply(distance);

            XYZ startPoint = lineToOffset.GetEndPoint(0).Add(translation);
            XYZ endPoint = lineToOffset.GetEndPoint(1).Add(translation);

            return Line.CreateBound(startPoint, endPoint);
        }

        public static Line LineOffsetVerically(Line lineToOffset, double distance)
        {
            XYZ startPoint = OffsetZValueXYZ(lineToOffset.GetEndPoint(0), distance);
            XYZ endPoint = OffsetZValueXYZ(lineToOffset.GetEndPoint(1), distance);
            return Line.CreateBound(startPoint, endPoint);
        }


        public static XYZ GetTangentVector(Line line)
        {
            return line.Direction;
        }

        public static XYZ GetNormalVector(Line line)
        {
            XYZ lineDirection = line.Direction;
            return XYZ.BasisZ.CrossProduct(lineDirection).Normalize();
        }

        public static double GetRotationFromVectors(XYZ vector1, XYZ vector2)
        {
            XYZ normalizedVector1 = vector1.Normalize();
            XYZ normalizedVector2 = vector2.Normalize();
            double angleRadians = normalizedVector1.AngleTo(normalizedVector2);
            return angleRadians * (180.0 / Math.PI);
        }

        public static List<XYZ> GetXYZlistFromCurve(Curve edgeLine, double minValueToAddMdiddle)
        {
            XYZ startXYZ = edgeLine.GetEndPoint(0);
            XYZ endXYZ = edgeLine.GetEndPoint(1);
            XYZ middleXYZ = edgeLine.Evaluate(0.5, true);

            List<XYZ> xyzList = new List<XYZ>()
                {
                    startXYZ,
                    endXYZ,
            };
            if (edgeLine.Length > minValueToAddMdiddle)
            {
                xyzList.Insert(1, middleXYZ);
            }
            return xyzList;
        }

        public static Line GetPerpendictularLine(Line edge, XYZ xyzPoint, string side)
        {
            XYZ normalVector = GetNormalVector(edge);

            XYZ endPoint = xyzPoint + MMToFeetConverter(10000) * normalVector;

            return Line.CreateBound(xyzPoint, endPoint);
        }

    }
}
