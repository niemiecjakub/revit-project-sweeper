﻿using Autodesk.Revit.Attributes;
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

        public List<Curve> GetFloorLongestLines(IList<Element> floorList, Reference selectedAlignment) 
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
                                    longestCurves.OrderBy(curve => alignmentSpline.Distance(curve.Evaluate(0.5, true)));
                                    longestCurves.ForEach(curve => floorCurves.Add(curve));
                                }
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
    }
}