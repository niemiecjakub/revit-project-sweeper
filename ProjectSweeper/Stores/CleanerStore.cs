using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjectSweeper.Stores
{
    public class CleanerStore
    {
        private readonly Document _doc;
        private readonly Cleaner _cleaner;

        private readonly Dictionary<ModelTypes, Lazy<Task<IEnumerable<IElement>>>> _lazyInitializationTasks;
        private Dictionary<ModelTypes, List<IElement>> _elementCollections;


        public event Action<IEnumerable<IElement>> ElementDeleted;

        public CleanerStore(Cleaner cleaner, Document doc)
        {
            _doc = doc;
            _cleaner = cleaner;

            _lazyInitializationTasks = Enum.GetValues(typeof(ModelTypes))
                .Cast<ModelTypes>()
                .ToDictionary(
                    modelType => modelType,
                    modelType => new Lazy<Task<IEnumerable<IElement>>>(async () => await _cleaner.GetAllElements(modelType))
                );

            _elementCollections = _lazyInitializationTasks.Keys.ToDictionary(key => key, key => new List<IElement>());
        }

        public IEnumerable<IElement> GetElementCollection(ModelTypes modelType)
        {
            return _elementCollections[modelType];
        }

        private async Task InitializeElementCollection(ModelTypes modelType)
        {
            Debug.WriteLine($"Initializing lazy {modelType}s");
            IEnumerable<IElement> elements = await _lazyInitializationTasks[modelType].Value;
            IEnumerable<IElement> validElements = elements.Where(e =>
            {
                Element element = _doc.GetElement(e.Id);
                Category category = Category.GetCategory(_doc, e.Id);
                return element != null || category != null;
            });

            _elementCollections[modelType].Clear();
            _elementCollections[modelType].AddRange(validElements);
        }

        public async Task LoadElements(ModelTypes modelType) => await InitializeElementCollection(modelType);

        public void DeleteElements(ModelTypes modelType)
        {
            Debug.WriteLine("STORE:");

            IEnumerable<IElement> elementsToBeDeleted = _elementCollections[modelType].Where(x => !x.IsUsed && x.CanBeRemoved);
            if (elementsToBeDeleted.Count() == 0)
            {
                Debug.WriteLine("Nothing to delete");
                return;
            }

            Debug.WriteLine($"{_elementCollections[modelType].Count} elements");
            Debug.WriteLine($"{elementsToBeDeleted.Count()} elements to be deleted");

            _cleaner.DeleteElements(elementsToBeDeleted);

            _elementCollections[modelType].RemoveAll(element => elementsToBeDeleted.Contains(element));

            Debug.WriteLine($"STORE: Left {_elementCollections[modelType].Count} {modelType}s");

            OnElementDeleted(_elementCollections[modelType]);
        }


        private void OnElementDeleted(IEnumerable<IElement> elements)
        {
            ElementDeleted?.Invoke(elements);
        }
    }
}