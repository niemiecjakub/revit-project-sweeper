using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ProjectSweeper.StairModule.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule.Handrail
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction transaction = new Transaction(doc, "Create handrails"))
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
                double BEAM_WIDTH = Utils.MMToFeetConverter(70);
                double HANDRAIL_DIA = Utils.MMToFeetConverter(40);
                double HANDRAIL_HEIGHT = Utils.MMToFeetConverter(850);

                StairModuleUI stairUI = new StairModuleUI(commandData);
                stairUI.ShowDialog();
                string SIDE = stairUI.Side.ToString().ToUpper();
           

                List<Curve> floorCurves = floorSelection.GetFarToAlignmentCurveBySelection(floorList, selectedAlignment).OrderBy(c => c.GetEndPoint(0).X).ToList();
                string SUPPORT_FAMILY_NAME = "HAND";
                FamilySymbol handrailFamily = Utils.GetFamilySymbolByName(SUPPORT_FAMILY_NAME, BuiltInCategory.OST_StructuralFraming, doc);
                
                //Build handrail on stair landing
                HandrailFactory handrailFactory = new HandrailFactory(doc, handrailFamily, floorCurves);
                handrailFactory.BuildLandingHandrails(SIDE, HANDRAIL_HEIGHT, BEAM_WIDTH);
                handrailFactory.BuildRunHandrails();

                transaction.Commit();
            }
            return Result.Succeeded;
        }
    }
}
