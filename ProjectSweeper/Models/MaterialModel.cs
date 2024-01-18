using Autodesk.Revit.DB;

namespace ProjectSweeper.Models
{
    public class MaterialModel : IElement
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public bool IsUsed { get; set; }
        public bool CanBeRemoved { get; set; }
        public ModelTypes ModelType { get; set; } = ModelTypes.Material;

        public MaterialModel(Material material)
        {
            Name = material.Name;
            Id = material.Id;
        }

        public MaterialModel(ElementId id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
