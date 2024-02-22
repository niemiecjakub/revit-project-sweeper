using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.StairModule.Structure
{
    public class SideBeamFactory
    {

        private readonly Document _doc;
        private readonly FamilySymbol _edgeBeamFamily;

        public SideBeamFactory(Document doc, FamilySymbol edgeBeamFamily)
        {
            _doc = doc;
            _edgeBeamFamily = edgeBeamFamily;
        }

        public FamilyInstance Build(Curve placementLine, int orientationValue, string purpose)
        {
            Level datumLevel = _doc.GetElement(Level.GetNearestLevelId(_doc, 0)) as Level;
            //FamilyInstance edgeBeam = _doc.Create.NewFamilyInstance(_placementLine, _edgeBeamFamily, datumLevel, StructuralType.NonStructural);

            FamilyInstance edgeBeam = AdaptiveComponentInstanceUtils.CreateAdaptiveComponentInstance(_doc, _edgeBeamFamily);
            IList<ElementId> placePointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(edgeBeam);
            ReferencePoint point1 = _doc.GetElement(placePointIds[0]) as ReferencePoint;
            point1.Position = placementLine.GetEndPoint(0);

            ReferencePoint point2 = _doc.GetElement(placePointIds[1]) as ReferencePoint;
            point2.Position = placementLine.GetEndPoint(1);

            Parameter cutAngle1 = edgeBeam.LookupParameter("cutAngle_1");
            Parameter cutAngle2 = edgeBeam.LookupParameter("cutAngle_2");
            Parameter p1Extend = edgeBeam.LookupParameter("p1_Extend");
            Parameter p2Extend = edgeBeam.LookupParameter("p2_Extend");
            Parameter yOffset = edgeBeam.LookupParameter("y_Offset");
            Parameter zOffset = edgeBeam.LookupParameter("z_Offset");
            Parameter orientation = edgeBeam.LookupParameter("Orientation");

            if (purpose == "landing")
            {
                cutAngle1.Set(13.437 * (Math.PI / 180.0));
                cutAngle2.Set(-13.437 * (Math.PI / 180.0));
                p1Extend.Set(Utils.MMToFeetConverter(188.041));
                p2Extend.Set(Utils.MMToFeetConverter(-188.041));
                zOffset.Set(Utils.MMToFeetConverter(-93));
                yOffset.Set(Utils.MMToFeetConverter(0));
            }

            if (purpose == "run")
            {
                cutAngle1.Set(-13.437 * (Math.PI / 180.0));
                cutAngle2.Set(13.437 * (Math.PI / 180.0));
                p1Extend.Set(Utils.MMToFeetConverter(188.041));
                p2Extend.Set(Utils.MMToFeetConverter(-188.041));
                zOffset.Set(Utils.MMToFeetConverter(-8));
                yOffset.Set(Utils.MMToFeetConverter(0));
            }
            orientation.Set(orientationValue);
            return edgeBeam;
        }

    }
}
