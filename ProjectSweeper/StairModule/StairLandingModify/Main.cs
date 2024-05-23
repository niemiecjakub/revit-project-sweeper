using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ProjectSweeper.StairModule.Handrail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule.StairLandingModify
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction transaction = new Transaction(doc, "Modify stair landings"))
            {
                transaction.Start();

                IList<Element> floorList = null;
                Reference selectedAlignment;
                try
                {
                    //floors
                    ISelectionFilter floorFilter = new FloorFilter();
                    floorList = uidoc.Selection.PickElementsByRectangle(floorFilter, "Select floors");
                    //alignment
                    selectedAlignment = uidoc.Selection.PickObject(ObjectType.Element, "Select alignment");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Cancelled;
                }

                FloorSelection floorSelection = new FloorSelection(uidoc, doc);
                double BEAM_WIDTH = Utils.MMToFeetConverter(69.6);
                double HANDRAIL_DIA = Utils.MMToFeetConverter(40);
                double HANDRAIL_HEIGHT = Utils.MMToFeetConverter(850);

                StairModuleUI stairUI = new StairModuleUI(commandData);
                stairUI.ShowDialog();
                string SIDE = stairUI.Side.ToString().ToUpper();

                foreach (Element element in floorList)
                {
                    Floor floor = element as Floor;
                    string heightOffset = floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsValueString();
                    //Debug.WriteLine(heightOffset);

                    //curves
                    Curve closeFloorCurve = floorSelection.GetClosesToAlignmentCurveBySelection(floor, selectedAlignment)[0];
                    Curve farFloorCurve = floorSelection.GetFarToAlignmentCurveBySelection(floor, selectedAlignment)[0];

                    double offsetValue = Utils.MMToFeetConverter(180);
                    //offsetValue = SIDE.Equals("L") ? offsetValue : -offsetValue;
                    Line offsetCloseLine = Utils.LineOffset(closeFloorCurve as Line, offsetValue);

                    XYZ p1 = offsetCloseLine.GetEndPoint(0);
                    XYZ p2 = offsetCloseLine.GetEndPoint(1);
                    XYZ p3 = farFloorCurve.GetEndPoint(0);
                    XYZ p4 = farFloorCurve.GetEndPoint(1);

                    CurveLoop curveLoop = new CurveLoop();
                    curveLoop.Append(Line.CreateBound(p1, p2));
                    curveLoop.Append(Line.CreateBound(p2, p3));
                    curveLoop.Append(Line.CreateBound(p3, p4));
                    curveLoop.Append(Line.CreateBound(p4, p1));


                    //Debug.WriteLine("p1: " + p1.X + " - " + p1.Y);
                    //Debug.WriteLine("p2: " + p2.X + " - " + p2.Y);
                    //Debug.WriteLine("p3: " + p3.X + " - " + p3.Y);
                    //Debug.WriteLine("p4: " + p4.X + " - " + p4.Y);

                    //Debug.WriteLine(p1.X > p2.X);
                    //Debug.WriteLine(p3.X > p4.X);

                    IList<CurveLoop> profile = new List<CurveLoop>() { curveLoop};

                    bool isPlanar = curveLoop.HasPlane();
                    //Debug.WriteLine("Is planar: " + isPlanar);

                    bool isValid = BoundaryValidation.IsValidHorizontalBoundary(profile);
                    //Debug.WriteLine("Is valid: " + isValid);

                    Level datumLevel = doc.GetElement(Level.GetNearestLevelId(doc, 0)) as Level;
                    ElementId levelId = datumLevel.Id;
                    ElementId floorTypeId = floor.FloorType.Id;

                    Floor newFloor = Floor.Create(doc, profile, floorTypeId, levelId);
                    double.TryParse(heightOffset, out double heightOffsetDouble);
                    heightOffsetDouble = Utils.MMToFeetConverter(heightOffsetDouble);

                    newFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(heightOffsetDouble);

                    doc.Delete(floor.Id);

                }

                //string SUPPORT_FAMILY_NAME = "TNL-RLG-HANDRAIL-CH2";
                //FamilySymbol landingFamily = Utils.GetFamilySymbolB  yName(SUPPORT_FAMILY_NAME, BuiltInCategory.OST_StructuralFraming, doc);

                ////Build handrail on stair landing
                //HandrailFactory handrailFactory = new HandrailFactory(doc, handrailFamily, floorCurves);
                //handrailFactory.BuildLandingHandrails(SIDE, HANDRAIL_HEIGHT, BEAM_WIDTH);
                //handrailFactory.BuildRunHandrails();

                transaction.Commit();
            }
            return Result.Succeeded;
        }
    }
}
