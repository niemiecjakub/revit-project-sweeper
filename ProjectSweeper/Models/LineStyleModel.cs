using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Models
{
    public class LineStyleModel : IElement
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public bool IsUsed { get; set; }
        public bool CanBeRemoved {  get; set; }
        public ElementId PatternId { get; set; }
        public int? LineWeight { get; set; }
        public Color Color { get; set; }
        public ModelTypes ModelType { get; set; } = ModelTypes.LineStyle;

        public LineStyleModel(string name, ElementId id, ElementId patternId, int? lineWeight, Color color)
        {
            Id = id;
            Name = name;
            PatternId = patternId;
            LineWeight = lineWeight;
            Color = color;
        }

        public LineStyleModel(Category lineStyle)
        {
            Id = lineStyle.Id;
            Name = lineStyle.Name;
            PatternId = lineStyle.GetLinePatternId(GraphicsStyleType.Projection);
            LineWeight = lineStyle.GetLineWeight(GraphicsStyleType.Projection);
            Color = lineStyle.LineColor;
        }

        public LinePatternElement GetLinePattern(Document doc)
        {
            LinePatternElement pattern = doc.GetElement(PatternId) as LinePatternElement;
            return pattern;
        }
    }
}
