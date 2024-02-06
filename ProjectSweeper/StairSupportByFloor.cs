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


namespace ProjectSweeper
{
    [Transaction(TransactionMode.Manual)]
    public class StairSupportByFloor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction transaction = new Transaction(doc, "Create Lines"))
            {
                transaction.Start();

                List<Reference> pickedFloors = null;
                Reference pickedTunnel = null;

                try
                {
                    Selection selection = uidoc.Selection;
                    pickedFloors = selection.PickObjects(ObjectType.Element, "Select Floors").ToList();
                    //pickedTunnel = selection.PickObject(ObjectType.Element, "Select an element");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Cancelled;
                }

                foreach (Reference pick in pickedFloors)
                {
                    Floor floor = GetFloorFromReference(pick, doc);
                    HashSet<Edge> edges = GetEdgesFromFloor(floor);
                    Edge longestEdge = edges.OrderByDescending(e => e.ApproximateLength).OrderByDescending(e => e.AsCurve().GetEndPoint(0).Z).FirstOrDefault();
                    Curve edgeLine = longestEdge.AsCurve();

                    SketchPlane sketchPlane = CreateSketchPlaneByCurve(edgeLine, doc);
                    ModelCurve modelLine = doc.Create.NewModelCurve(edgeLine, sketchPlane);
                    Debug.WriteLine(modelLine == null);
                }
            }

            return Result.Succeeded;
        }
        public SketchPlane CreateSketchPlaneByCurve(Curve curve, Document doc)
        {
            XYZ startXYZ = curve.GetEndPoint(0);
            XYZ endXYZ = curve.GetEndPoint(1);
            XYZ middleXYZ = new XYZ((startXYZ.X + endXYZ.X) / 2, (startXYZ.Y + endXYZ.Y) / 2, (startXYZ.Z + endXYZ.Z) / 2 + 300);
            Plane plane = Plane.CreateByThreePoints(startXYZ, middleXYZ, endXYZ);
            return SketchPlane.Create(doc, plane);
        }


        public Floor GetFloorFromReference(Reference reference, Document doc)
        {
            ElementId floorId = reference.ElementId;
            return doc.GetElement(floorId) as Floor;
        }

        public HashSet<Edge> GetEdgesFromFloor(Floor floor)
        {
            Document doc = floor.Document;
            Options options = new Options();
            options.ComputeReferences = true;
            options.View = doc.ActiveView;

            GeometryElement geomElement = floor.get_Geometry(options);
            HashSet<Edge> floorEdges = new HashSet<Edge>();

            foreach (GeometryObject geomObj in geomElement)
            {
                if (geomObj is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        EdgeArray edges = face.EdgeLoops.get_Item(0);
                        foreach (Edge edge in edges)
                        {
                            floorEdges.Add(edge);
                        }
                    }
                }
            }

            return floorEdges;
        }
    }
}
