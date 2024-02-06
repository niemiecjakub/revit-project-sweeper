using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule
{
    public class HorizontalBeamBuilder
    {
        public static FamilyInstance CreateHorizontalBeam(Document doc, FamilySymbol familySymbol, Line placementLine, double tunnelPlateRotation)
        {
            Level datumLevel = doc.GetElement(Level.GetNearestLevelId(doc, 0)) as Level;

            FamilyInstance horizontalBeam = doc.Create.NewFamilyInstance(placementLine, familySymbol, datumLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
            Parameter zJustification = horizontalBeam.get_Parameter(BuiltInParameter.Z_JUSTIFICATION);
            Parameter startJoinCutBack = horizontalBeam.get_Parameter(BuiltInParameter.START_JOIN_CUTBACK);

            Parameter rightCutAngle = horizontalBeam.LookupParameter("RightCutAngle");
            Parameter leftCutAngle = horizontalBeam.LookupParameter("LeftCutAngle");

            Parameter bhRight = horizontalBeam.LookupParameter("bh_right");
            Parameter bwRight = horizontalBeam.LookupParameter("bw_right");
            Parameter btRight = horizontalBeam.LookupParameter("bt_right");

            Parameter bhLeft = horizontalBeam.LookupParameter("bh_left");
            Parameter bwLeft = horizontalBeam.LookupParameter("bw_left");
            Parameter btLeft = horizontalBeam.LookupParameter("bt_left");

            bhRight.Set(Utils.MMToFeetConverter(580));
            bwRight.Set(Utils.MMToFeetConverter(580));
            btRight.Set(Utils.MMToFeetConverter(16));

            bhLeft.Set(Utils.MMToFeetConverter(140));
            bwLeft.Set(Utils.MMToFeetConverter(73));
            btLeft.Set(Utils.MMToFeetConverter(10));

            leftCutAngle.Set(90 * (Math.PI / 180.0));
            rightCutAngle.Set((tunnelPlateRotation * (Math.PI / 180.0)) - Math.PI / 2);

            startJoinCutBack.Set(Utils.MMToFeetConverter(-92.5));
            zJustification.Set(2);

            return horizontalBeam;

        }

    }
}
