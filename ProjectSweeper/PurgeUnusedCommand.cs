using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper
{
    [Transaction(TransactionMode.Manual)]
    public class PurgeUnusedCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
            //https://digitalbbq.au/index.php/2024/01/18/purging-material-assets-using-the-revit-api-the-right-way/
                // Get the MethodInfo for the internal method
                //AddUnusedAssets(doc, GetUnusedAssets(doc, "GetUnusedMaterials"), "Material", unusedAssetNames, unusedAssetIds);
                //AddUnusedAssets(doc, GetUnusedAssets(doc, "GetUnusedAppearances"), "Appearance", unusedAssetNames, unusedAssetIds);
                //AddUnusedAssets(doc, GetUnusedAssets(doc, "GetUnusedStructures"), "Structure", unusedAssetNames, unusedAssetIds);
                //AddUnusedAssets(doc, GetUnusedAssets(doc, "GetUnusedThermals"), "Thermal", unusedAssetNames, unusedAssetIds);
                //AddUnusedAssets(doc, GetUnusedAssets(doc, "GetUnusedSymbols"), "Symbol", unusedAssetNames, unusedAssetIds);
                //AddUnusedAssets(doc, GetUnusedAssets(doc, "GetUnusedFamilies"), "Families", unusedAssetNames, unusedAssetIds);
                //AddUnusedAssets(doc, GetUnusedAssets(doc, "GetUnusedImportCategories"), "Import category", unusedAssetNames, unusedAssetIds);

                MethodInfo getUnusedAppearancesMethod = typeof(Document).GetMethod("GetUnusedAppearances", BindingFlags.NonPublic | BindingFlags.Instance);

                // Invoke the method and get the result
                if (getUnusedAppearancesMethod != null)
                {
                    ICollection<ElementId> unusedAppearanceIds = (ICollection<ElementId>)getUnusedAppearancesMethod.Invoke(doc, null);
                    foreach(ElementId unusedAppearanceId in unusedAppearanceIds)
                    {
                        Element e = doc.GetElement(unusedAppearanceId);
                        Debug.WriteLine(e.Name);
                    }
                    // Now you have the unused appearances and can proceed to delete them
                    // Similar reflection calls would need to be made for the other GetUnused... methods
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                TaskDialog.Show("Error", "An error occurred: " + ex.Message);
            }

            return Result.Succeeded;
        }
    }
}
