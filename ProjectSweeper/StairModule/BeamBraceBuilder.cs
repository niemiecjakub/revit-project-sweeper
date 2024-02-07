using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule
{
    public class BeamBraceBuilder
    {
        public static FamilyInstance CreateBraceBeam(Document doc, FamilySymbol braceFamilySymbol, XYZ basePoint, XYZ topPoint)
        {
            Level datumLevel = doc.GetElement(Level.GetNearestLevelId(doc, 0)) as Level;

            double offsetValueToIPEcenter = Utils.MMToFeetConverter(70);
            if (topPoint.Z > basePoint.Z)
            {
                topPoint = Utils.OffsetZValueXYZ(topPoint, offsetValueToIPEcenter);
            }
            else
            {
                basePoint = Utils.OffsetZValueXYZ(basePoint, offsetValueToIPEcenter);
            }
            Line bracePlacementLine = Line.CreateBound(basePoint, topPoint);
            SketchPlane braceSketchPlane = Utils.CreateSketchPlaneByCurve(bracePlacementLine, doc);


            FamilyInstance braceBeam = doc.Create.NewFamilyInstance(bracePlacementLine, braceFamilySymbol, datumLevel, StructuralType.Beam);
            Parameter zJustification = braceBeam.get_Parameter(BuiltInParameter.Z_JUSTIFICATION);
            zJustification.Set(2);

            return braceBeam;
        }
    }
}
