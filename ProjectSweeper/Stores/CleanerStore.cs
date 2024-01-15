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
        private readonly Cleaner _cleaner;

        private readonly Dictionary<ModelTypes, Lazy<Task<IEnumerable<IElement>>>> _lazyInitializationTasks;
        private Dictionary<ModelTypes, List<IElement>> _elementCollections;

        //public IEnumerable<IElement> LineStyles => GetElementCollection(ModelTypes.LineStyle);
        //public IEnumerable<IElement> LinePatterns => GetElementCollection(ModelTypes.LinePattern);
        //public IEnumerable<IElement> FilledRegions => GetElementCollection(ModelTypes.FilledRegion);
        //public IEnumerable<IElement> FillPatterns => GetElementCollection(ModelTypes.FillPattern);

        public event Action<IEnumerable<IElement>> ElementDeleted;

        public CleanerStore(Cleaner cleaner)
        {
            _cleaner = cleaner;

            _lazyInitializationTasks = new Dictionary<ModelTypes, Lazy<Task<IEnumerable<IElement>>>>
            {
                { ModelTypes.LineStyle, new Lazy<Task<IEnumerable<IElement>>>(async () => await _cleaner.GetAllElements(ModelTypes.LineStyle)) },
                { ModelTypes.LinePattern, new Lazy<Task<IEnumerable<IElement>>>(async () => await _cleaner.GetAllElements(ModelTypes.LinePattern)) },
                { ModelTypes.FilledRegion, new Lazy<Task<IEnumerable<IElement>>>(async () => await _cleaner.GetAllElements(ModelTypes.FilledRegion)) },
                { ModelTypes.FillPattern, new Lazy<Task<IEnumerable<IElement>>>(async () => await _cleaner.GetAllElements(ModelTypes.FillPattern)) },
            };

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
            _elementCollections[modelType].Clear();
            _elementCollections[modelType].AddRange(elements);
        }

        public async Task LoadElements(ModelTypes modelType) => await InitializeElementCollection(modelType);

        public void DeleteElements(ModelTypes modelType)
        {
            Debug.WriteLine("STORE:");

            IEnumerable<IElement> elementsToBeDeleted = _elementCollections[modelType].Where(x => !x.IsUsed && x.CanBeRemoved);
            if (elementsToBeDeleted.Count() == 0) 
            {
                Console.WriteLine("Nothing to delete");
                return;
            }

            Debug.WriteLine($"{_elementCollections[modelType].Count} elements");
            Debug.WriteLine($"{elementsToBeDeleted.Count()} elements to be deleted");

            _cleaner.DeleteElements(elementsToBeDeleted);

            List<IElement> collectionCopy = new List<IElement>(_elementCollections[modelType]);
            foreach (IElement element in elementsToBeDeleted)
            {
                collectionCopy.Remove(element);
            }
            _elementCollections[modelType].Clear();
            _elementCollections[modelType].AddRange(collectionCopy);

            Debug.WriteLine($"STORE: Left {_elementCollections[modelType].Count} {modelType}s");

            OnElementDeleted(_elementCollections[modelType]);
        }

        private void OnElementDeleted(IEnumerable<IElement> elements)
        {
            ElementDeleted?.Invoke(elements);
        }
    }
}