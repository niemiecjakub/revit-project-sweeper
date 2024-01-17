using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProjectSweeper
{
    [Transaction(TransactionMode.Manual)]
    public class ExplodeAndDeleteGroupCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Group> groups = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(Group)).Cast<Group>().ToList();
            List<ElementId> groupTypeIds = new FilteredElementCollector(doc).WhereElementIsElementType().OfClass(typeof(GroupType)).Cast<GroupType>().Select(groupTtype => groupTtype.Id).ToList();

            try
            {
                using (Transaction transaction = new Transaction(doc, "Ungroup elements and delete groups"))
                {
                    transaction.Start("Project sweeper - delete groups");

                    try
                    {

                        foreach (Group group in groups)
                        {
                            group.UngroupMembers();
                        }
                        doc.Delete(groupTypeIds);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.RollBack();
                        TaskDialog.Show("Error", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine($"Error: {ex.Message}");
            }

            return Result.Succeeded;
        }
    }
}
