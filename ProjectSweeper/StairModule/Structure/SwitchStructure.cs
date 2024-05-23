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

namespace ProjectSweeper.StairModule.Structure
{
    [Transaction(TransactionMode.Manual)]
    public class SwitchStructure : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction transaction = new Transaction(doc, "Create switch stair elements"))
            {
                transaction.Start();

                IList<Element> floorList = null;
                Reference pickedTunnel = null;
                Reference pickedSlab = null;
                Reference selectedAlignment;
                try
                {
                    //floors
                    ISelectionFilter floorFilter = new FloorFilter();
                    floorList = uidoc.Selection.PickElementsByRectangle(floorFilter, "Select Floors");
                    //alignment
                    selectedAlignment = uidoc.Selection.PickObject(ObjectType.Element, "Select Alignment");
                    //tunnel
                    pickedTunnel = uidoc.Selection.PickObject(ObjectType.Element, "Select Tunnel Lining");
                    //salab
                    pickedSlab = uidoc.Selection.PickObject(ObjectType.Element, "Select Tunnel Slab");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return Result.Cancelled;
                }


                FloorSelection floorSelection = new FloorSelection(uidoc, doc);
                StairModuleUI stairUI = new StairModuleUI(commandData);
                stairUI.ShowDialog();
                string SIDE = stairUI.Side.ToString().ToUpper();

                double BEAM_HEIGHT = Utils.MMToFeetConverter(254);
                double BEAM_WIDTH = Utils.MMToFeetConverter(70);
                double STEP_WIDTH = Utils.MMToFeetConverter(296);
                //double OFFSET_VALUE = SIDE == "R" ? BEAM_WIDTH / 2 : BEAM_WIDTH / 2;
                double OFFSET_VALUE_TO_COLUMN_AX = Utils.MMToFeetConverter(77);

                SolidCurveIntersectionOptions intersectOptions = new SolidCurveIntersectionOptions();

                List<Solid> tunnelSolids = Utils.GetSolidFromElement(pickedTunnel, doc);
                List<Solid> slabSolids = Utils.GetSolidFromElement(pickedSlab, doc);
                List<Curve> floorCurves = floorSelection.GetClosesToAlignmentCurveBySelection(floorList, selectedAlignment);

                double maxSpacing = Utils.MMToFeetConverter(2100);

                string IPE_FAMILY_FAME = "HBEAM";
                FamilySymbol horizontalBeamFamily = Utils.GetFamilySymbolByName(IPE_FAMILY_FAME, BuiltInCategory.OST_StructuralFraming, doc);
                HorizontalBeamFactory horizontalBeamFactory = new HorizontalBeamFactory(doc, maxSpacing, horizontalBeamFamily, tunnelSolids);


                string SUPPORT_FAMILY_NAME = "SUPPORTCOLUMN";
                FamilySymbol supportFamily = Utils.GetFamilySymbolByName(SUPPORT_FAMILY_NAME, BuiltInCategory.OST_StructuralColumns, doc);
                SupportColumnFactory supportColumnFactory = new SupportColumnFactory(doc, maxSpacing, supportFamily, slabSolids);

                foreach (Curve edgeCurve in floorCurves)
                {
                    double trimStartValue = SIDE == "R" ? Utils.MMToFeetConverter(296 + 50 + 60) : Utils.MMToFeetConverter(60);
                    double trimEndValue = SIDE == "R" ? Utils.MMToFeetConverter(60) : Utils.MMToFeetConverter(296 + 50 + 60);
                    Curve edgeCurveTrimmed = Utils.TrimStartEndByValue(edgeCurve as Line, trimStartValue, trimEndValue);
                    Line edgeLineOffset = Utils.LineOffset(edgeCurveTrimmed as Line, OFFSET_VALUE_TO_COLUMN_AX);

                    //SUPPORT BUILD
                    List<FamilyInstance> supportColumns = supportColumnFactory.BuildAlSwitch(SIDE, BEAM_HEIGHT, edgeLineOffset);

                    //HBEAM BUILDER 
                    List<FamilyInstance> horizontalBeams = horizontalBeamFactory.BuildAllSwitch(SIDE, edgeCurveTrimmed);

                    supportColumnFactory.ColumnPlacementlines.Clear();
                }


                //// L BEAMS BUILDER
                //List<Curve> floorLandingSideCurves = floorSelection.GetFloorSideLines(floorList, selectedAlignment);
                //string FAMILY_SYMBOL_LANDING_SUPPORT_BEAM = "LBEAM";
                //FamilySymbol landingSupportBeamSymbol = Utils.GetFamilySymbolByName(FAMILY_SYMBOL_LANDING_SUPPORT_BEAM, BuiltInCategory.OST_StructuralFraming, doc);
                //foreach (Curve landingCurve in floorLandingSideCurves)
                //{
                //    Line profileLine = Utils.LineOffsetVerically(landingCurve as Line, Utils.MMToFeetConverter(-30));
                //    Level datumLevel = doc.GetElement(Level.GetNearestLevelId(doc, 0)) as Level;
                //    FamilyInstance landingSupportBeam = doc.Create.NewFamilyInstance(profileLine, landingSupportBeamSymbol, datumLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
                //}


                transaction.Commit();
            }
            return Result.Succeeded;
        }
    }
}
