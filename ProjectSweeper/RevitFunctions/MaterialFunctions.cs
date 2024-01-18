using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.RevitFunctions
{
    public static class MaterialFunctions
    {

        public static ISet<MaterialModel> GetAllMaterials(Document doc)
        {
            ISet<MaterialModel> materialList = new HashSet<MaterialModel>();

            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Material));
            IList<Material> materials = collector.Cast<Material>().ToList();

            foreach (Material material in materials)
            {
                MaterialModel materialModel = new MaterialModel(material);
                materialModel.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, material.Id);
                materialList.Add(materialModel);
            }

            SetUnusedMaterials(doc, materialList);

            return materialList;
        }

        public static ISet<MaterialAppearanceAssetModel> GetAllMaterialAppearances(Document doc)
        {
            ISet<MaterialAppearanceAssetModel> appearanceAssetList = new HashSet<MaterialAppearanceAssetModel>();

            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(AppearanceAssetElement));
            IList<AppearanceAssetElement> appearanceAssets = collector.Cast<AppearanceAssetElement>().ToList();
            foreach (AppearanceAssetElement asset in appearanceAssets)
            {
                MaterialAppearanceAssetModel assetModel = new MaterialAppearanceAssetModel(asset);
                assetModel.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, asset.Id);
                appearanceAssetList.Add(assetModel);
            }

            SetUnusedMaterialAppearances(doc, appearanceAssetList);

            return appearanceAssetList;
        }



        private static void SetUnusedMaterials(Document doc, ISet<MaterialModel> materialList)
        {
            MethodInfo getUnusedMaterialsMethod = typeof(Document).GetMethod("GetUnusedMaterials", BindingFlags.NonPublic | BindingFlags.Instance);
            if (getUnusedMaterialsMethod != null)
            {
                ICollection<ElementId> unusedMaterialIds = (ICollection<ElementId>)getUnusedMaterialsMethod.Invoke(doc, null);
                foreach (ElementId unusedMaterialId in unusedMaterialIds)
                {
                    Element e = doc.GetElement(unusedMaterialId);
                    MaterialModel materialModel = materialList.FirstOrDefault(e => e.Id == unusedMaterialId);
                    if (materialModel != null)
                    {
                        materialModel.IsUsed = true;
                    }
                }
            }
        }

        private static void SetUnusedMaterialAppearances(Document doc, ISet<MaterialAppearanceAssetModel> materialAppearanceList)
        {
            MethodInfo getUnusedMaterialAppearancesMethod = typeof(Document).GetMethod("GetUnusedAppearances", BindingFlags.NonPublic | BindingFlags.Instance);
            if (getUnusedMaterialAppearancesMethod != null)
            {
                ICollection<ElementId> unusedAppearanceIds = (ICollection<ElementId>)getUnusedMaterialAppearancesMethod.Invoke(doc, null);
                foreach (ElementId unusedAppearanceId in unusedAppearanceIds)
                {
                    Element e = doc.GetElement(unusedAppearanceId);
                    MaterialAppearanceAssetModel materialModel = materialAppearanceList.FirstOrDefault(e => e.Id == unusedAppearanceId);
                    if (materialModel != null)
                    {
                        materialModel.IsUsed = true;
                    }
                }
            }
        }

    }

}
