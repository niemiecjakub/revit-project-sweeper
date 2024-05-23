using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule
{
    public class FloorSelection
    {
        private readonly UIDocument _uidoc;
        private readonly Document _doc;
        public FloorSelection(UIDocument uidoc, Document doc)
        {
            _uidoc = uidoc;
            _doc = doc;
        }

        public List<Curve> GetFarToAlignmentCurveBySelection(IList<Element> floorList, Reference selectedAlignment)
        {
            Options options = new Options();
            HermiteSpline alignmentSpline = GetSplineFromAlignment(selectedAlignment, options);

            List<Curve> floorCurves = new List<Curve>();
            foreach (Floor floor in floorList)
            {
                GeometryElement floorgGeometryElements = floor.get_Geometry(options);
                foreach (GeometryObject geometryObject in floorgGeometryElements)
                {
                    if (geometryObject is Solid solid)
                    {
                        foreach (Face face in solid.Faces)
                        {
                            if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                            {
                                foreach (EdgeArray edges in face.EdgeLoops)
                                {
                                    List<Curve> faceCurves = new List<Curve>();
                                    foreach (Edge edge in edges)
                                    {
                                        Curve edgeCurve = edge.AsCurve();
                                        faceCurves.Add(edgeCurve);
                                    }

                                    List<Curve> longestCurves = faceCurves.OrderByDescending(curve => curve.Length).Take(2).ToList();
                                    Curve hostFloorEgde = longestCurves.OrderByDescending(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true))).FirstOrDefault();
                                    floorCurves.Add(hostFloorEgde);
                                }
                            }
                        }
                    }
                }
            }
            return floorCurves;
        }

        public List<Curve> GetFarToAlignmentCurveBySelection(Element floor, Reference selectedAlignment)
        {
            Options options = new Options();
            HermiteSpline alignmentSpline = GetSplineFromAlignment(selectedAlignment, options);

            List<Curve> floorCurves = new List<Curve>();

            GeometryElement floorgGeometryElements = floor.get_Geometry(options);
            foreach (GeometryObject geometryObject in floorgGeometryElements)
            {
                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                        {
                            foreach (EdgeArray edges in face.EdgeLoops)
                            {
                                List<Curve> faceCurves = new List<Curve>();
                                foreach (Edge edge in edges)
                                {
                                    Curve edgeCurve = edge.AsCurve();
                                    faceCurves.Add(edgeCurve);
                                }

                                Curve hostFloorEgde = faceCurves.OrderByDescending(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true))).FirstOrDefault();
                                floorCurves.Add(hostFloorEgde);
                            }
                        }
                    }
                }
            }
            return floorCurves;
        }


        public List<Curve> GetClosesToAlignmentCurveBySelection(IList<Element> floorList, Reference selectedAlignment)
        {
            Options options = new Options();
            HermiteSpline alignmentSpline = GetSplineFromAlignment(selectedAlignment, options);

            List<Curve> floorCurves = new List<Curve>();
            foreach (Floor floor in floorList)
            {
                GeometryElement floorgGeometryElements = floor.get_Geometry(options);
                foreach (GeometryObject geometryObject in floorgGeometryElements)
                {
                    if (geometryObject is Solid solid)
                    {
                        foreach (Face face in solid.Faces)
                        {
                            if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                            {
                                foreach (EdgeArray edges in face.EdgeLoops)
                                {
                                    List<Curve> faceCurves = new List<Curve>();
                                    foreach (Edge edge in edges)
                                    {
                                        Curve edgeCurve = edge.AsCurve();
                                        faceCurves.Add(edgeCurve);
                                    }

                                    List<Curve> longestCurves = faceCurves.OrderByDescending(curve => curve.Length).Take(2).ToList();
                                    Curve hostFloorEgde = longestCurves.OrderBy(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true))).FirstOrDefault();
                                    floorCurves.Add(hostFloorEgde);
                                }
                            }
                        }
                    }
                }
            }
            return floorCurves;
        }

        public List<Curve> GetClosesToAlignmentCurveBySelection(Element floor, Reference selectedAlignment)
        {
            Options options = new Options();
            HermiteSpline alignmentSpline = GetSplineFromAlignment(selectedAlignment, options);

            List<Curve> floorCurves = new List<Curve>();
            GeometryElement floorgGeometryElements = floor.get_Geometry(options);
            foreach (GeometryObject geometryObject in floorgGeometryElements)
            {
                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                        {
                            foreach (EdgeArray edges in face.EdgeLoops)
                            {
                                List<Curve> faceCurves = new List<Curve>();
                                foreach (Edge edge in edges)
                                {
                                    Curve edgeCurve = edge.AsCurve();
                                    faceCurves.Add(edgeCurve);
                                }
                                Curve hostFloorEgde = faceCurves.OrderBy(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true))).FirstOrDefault();
                                floorCurves.Add(hostFloorEgde);
                            }
                        }
                    }
                }
            }
            return floorCurves;
        }

        public List<Curve> GetFloorSideLines(IList<Element> floorList, Reference selectedAlignment)
        {
            Options options = new Options();
            HermiteSpline alignmentSpline = GetSplineFromAlignment(selectedAlignment, options);
            List<Curve> floorCurves = new List<Curve>();
            foreach (Floor floor in floorList)
            {
                GeometryElement floorgGeometryElements = floor.get_Geometry(options);
                foreach (GeometryObject geometryObject in floorgGeometryElements)
                {
                    if (geometryObject is Solid solid)
                    {
                        foreach (Face face in solid.Faces)
                        {
                            if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                            {
                                foreach (EdgeArray edges in face.EdgeLoops)
                                {
                                    List<Curve> faceCurves = new List<Curve>();
                                    foreach (Edge edge in edges)
                                    {
                                        Curve edgeCurve = edge.AsCurve();
                                        faceCurves.Add(edgeCurve);
                                    }

                                    List<Curve> longestCurves = faceCurves.OrderByDescending(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true))).ToList();
                                    floorCurves.Add(longestCurves[0]);
                                    floorCurves.Add(longestCurves[longestCurves.Count - 1]);
                                    floorCurves.OrderBy(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true)));

                                    //List<Curve> longestCurves = faceCurves.OrderByDescending(curve => curve.Length).Take(2).ToList();
                                    //longestCurves.OrderBy(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true)));
                                    //longestCurves.ForEach(curve => floorCurves.Add(curve));
                                }
                            }
                        }
                    }
                }
            }
            return floorCurves;
        }

        public List<Curve> GetFloorSideLines(Element floor, Reference selectedAlignment)
        {
            Options options = new Options();
            HermiteSpline alignmentSpline = GetSplineFromAlignment(selectedAlignment, options);
            List<Curve> floorCurves = new List<Curve>();
            GeometryElement floorgGeometryElements = floor.get_Geometry(options);
            foreach (GeometryObject geometryObject in floorgGeometryElements)
            {
                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                        {
                            foreach (EdgeArray edges in face.EdgeLoops)
                            {
                                List<Curve> faceCurves = new List<Curve>();
                                foreach (Edge edge in edges)
                                {
                                    Curve edgeCurve = edge.AsCurve();
                                    faceCurves.Add(edgeCurve);
                                }

                                List<Curve> longestCurves = faceCurves.OrderByDescending(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true))).ToList();
                                floorCurves.Add(longestCurves[0]);
                                floorCurves.Add(longestCurves[longestCurves.Count - 1]);
                                floorCurves.OrderBy(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true)));

                                //List<Curve> longestCurves = faceCurves.OrderByDescending(curve => curve.Length).Take(2).ToList();
                                //longestCurves.OrderBy(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true)));
                                //longestCurves.ForEach(curve => floorCurves.Add(curve));
                            }
                        }
                    }
                }

            }
            return floorCurves;
        }

        private HermiteSpline GetSplineFromAlignment(Reference selectedAlignment, Options options)
        {
            HermiteSpline alignmentSpline = null;
            Element alignment = _doc.GetElement(selectedAlignment.ElementId);
            GeometryElement alignmentGeometryElements = alignment.get_Geometry(options);
            foreach (GeometryObject geometryElement in alignmentGeometryElements)
            {
                if (geometryElement is GeometryInstance geometryInstance)
                {
                    GeometryElement symbolGeometry = geometryInstance.GetSymbolGeometry();
                    foreach (var item in symbolGeometry)
                    {
                        if (item is HermiteSpline spline)
                        {
                            alignmentSpline = spline;
                        }
                    }
                }
            }

            return alignmentSpline;
        }

        public Curve GetFloorLongestCurve(Floor floor)
        {
            Options options = new Options();
            Curve longestCurve = null;

            GeometryElement floorgGeometryElements = floor.get_Geometry(options);
            foreach (GeometryObject geometryObject in floorgGeometryElements)
            {
                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                        {
                            foreach (EdgeArray edges in face.EdgeLoops)
                            {
                                List<Curve> faceCurves = new List<Curve>();
                                foreach (Edge edge in edges)
                                {
                                    Curve edgeCurve = edge.AsCurve();
                                    faceCurves.Add(edgeCurve);
                                }

                                longestCurve = faceCurves.OrderByDescending(curve => curve.Length).FirstOrDefault();
                            }
                        }
                    }
                }
            }

            return longestCurve;
        }

        public double GetFloorLongestCurveLength(Floor floor)
        {
            Line landingLengthCurve = GetFloorLongestCurve(floor) as Line;
            return landingLengthCurve.Length;

        }

        public XYZ GetFloorAproxLocation(Floor floor)
        {
            Line landingLengthCurve = GetFloorLongestCurve(floor) as Line;
            return landingLengthCurve.Evaluate(0.5, true);
        }

        public List<Curve> GetLandingStartEndLines(IList<Floor> floorList)
        {
            Options options = new Options();
            List<Curve> floorCurves = new List<Curve>();
            foreach (Floor floor in floorList)
            {
                GeometryElement floorgGeometryElements = floor.get_Geometry(options);
                foreach (GeometryObject geometryObject in floorgGeometryElements)
                {
                    if (geometryObject is Solid solid)
                    {
                        foreach (Face face in solid.Faces)
                        {
                            if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                            {
                                foreach (EdgeArray edges in face.EdgeLoops)
                                {
                                    List<Curve> faceCurves = new List<Curve>();
                                    foreach (Edge edge in edges)
                                    {
                                        Curve edgeCurve = edge.AsCurve();
                                        faceCurves.Add(edgeCurve);
                                    }

                                    List<Curve> longestCurves = faceCurves.OrderBy(curve => curve.Evaluate(0.5, true).X).ToList();
                                    floorCurves.Add(longestCurves[1]);
                                    floorCurves.Add(longestCurves[2]);

                                    floorCurves.OrderByDescending(c => c.Evaluate(0.5, true).X).ToList();
                                }
                            }
                        }
                    }
                }
            }
            return floorCurves;
        }

        public List<Curve> GetLandingStartEndLines(Floor floor)
        {
            Options options = new Options();
            List<Curve> floorCurves = new List<Curve>();
            GeometryElement floorgGeometryElements = floor.get_Geometry(options);
            foreach (GeometryObject geometryObject in floorgGeometryElements)
            {
                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                        {
                            foreach (EdgeArray edges in face.EdgeLoops)
                            {
                                List<Curve> faceCurves = new List<Curve>();
                                foreach (Edge edge in edges)
                                {
                                    Curve edgeCurve = edge.AsCurve();
                                    faceCurves.Add(edgeCurve);
                                }

                                List<Curve> longestCurves = faceCurves.OrderBy(curve => curve.Evaluate(0.5, true).X).ToList();
                                floorCurves.Add(longestCurves[0]);
                                floorCurves.Add(longestCurves[longestCurves.Count - 1]);
                            }
                        }
                    }
                }
            }
            return floorCurves;
        }

        public List<Curve> GetLandingStartSideCurves(Floor floor)
        {
            Options options = new Options();
            List<Curve> floorCurves = new List<Curve>();
            GeometryElement floorgGeometryElements = floor.get_Geometry(options);
            foreach (GeometryObject geometryObject in floorgGeometryElements)
            {
                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                        {
                            foreach (EdgeArray edges in face.EdgeLoops)
                            {
                                List<Curve> faceCurves = new List<Curve>();
                                foreach (Edge edge in edges)
                                {
                                    Curve edgeCurve = edge.AsCurve();
                                    faceCurves.Add(edgeCurve);
                                }

                                List<Curve> longestCurves = faceCurves.OrderBy(curve => curve.Evaluate(0.5, true).X).ToList();
                                floorCurves.Add(longestCurves[1]);
                                floorCurves.Add(longestCurves[2]);
                            }
                        }
                    }
                }
            }
            return floorCurves;
        }

        public List<Curve> GetLandingLines(Floor landing)
        {
            Options options = new Options();
            List<Curve> floorCurves = new List<Curve>();

            GeometryElement floorgGeometryElements = landing.get_Geometry(options);
            foreach (GeometryObject geometryObject in floorgGeometryElements)
            {
                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (face.ComputeNormal(UV.BasisV).Z > 0.0)
                        {
                            foreach (EdgeArray edges in face.EdgeLoops)
                            {
                                List<Curve> faceCurves = new List<Curve>();
                                foreach (Edge edge in edges)
                                {
                                    Curve edgeCurve = edge.AsCurve();
                                    floorCurves.Add(edgeCurve);
                                }
                            }
                        }
                    }
                }
            }
            return floorCurves;
        }
    }
}
