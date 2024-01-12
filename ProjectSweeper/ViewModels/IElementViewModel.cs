using Autodesk.Revit.DB;

namespace ProjectSweeper.ViewModels
{
    public interface IElementViewModel
    {
        ElementId Id { get; }
        bool IsUsed { get; }
        string Name { get; }
        bool CanBeRemoved { get; }

    }
}