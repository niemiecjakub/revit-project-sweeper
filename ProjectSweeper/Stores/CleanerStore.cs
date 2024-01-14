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
        private readonly Dictionary<ModelTypes, List<IElement>> _elementCollections;

        public IEnumerable<IElement> LineStyles => GetElementCollection(ModelTypes.LineStyle);
        public IEnumerable<IElement> LinePatterns => GetElementCollection(ModelTypes.LinePattern);
        public IEnumerable<IElement> FilledRegions => GetElementCollection(ModelTypes.FilledRegion);
        public IEnumerable<IElement> FillPatterns => GetElementCollection(ModelTypes.FillPattern);

        public event Action<IEnumerable<IElement>> ElementDeleted;

        public CleanerStore(Cleaner cleaner)
        {
            _cleaner = cleaner;

            _lazyInitializationTasks = new Dictionary<ModelTypes, Lazy<Task<IEnumerable<IElement>>>>
            {
                { ModelTypes.LineStyle, new Lazy<Task<IEnumerable<IElement>>>(async () => await _cleaner.GetAllLineStyles()) },
                { ModelTypes.LinePattern, new Lazy<Task<IEnumerable<IElement>>>(async () => await _cleaner.GetAllLinePatterns()) },
                { ModelTypes.FilledRegion, new Lazy<Task<IEnumerable<IElement>>>(async () => await _cleaner.GetAllFilledRegions()) },
                { ModelTypes.FillPattern, new Lazy<Task<IEnumerable<IElement>>>(async () => await _cleaner.GetAllFillPatterns()) },
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
            var elements = await _lazyInitializationTasks[modelType].Value;
            _elementCollections[modelType].Clear();
            _elementCollections[modelType].AddRange(elements);
        }

        public async Task LoadLineStyles() => await InitializeElementCollection(ModelTypes.LineStyle);
        public async Task LoadLinePatterns() => await InitializeElementCollection(ModelTypes.LinePattern);
        public async Task LoadFilledRegions() => await InitializeElementCollection(ModelTypes.FilledRegion);
        public async Task LoadFillPatterns() => await InitializeElementCollection(ModelTypes.FillPattern);
        public async Task LoadElements(ModelTypes modelType) => await InitializeElementCollection(modelType);


        public void DeleteElements(ModelTypes modelType)
        {
            Debug.WriteLine("STORE: Inside store");
            //IEnumerable<IElement> elementsToBeDeleted = _elementsToBeDeleted.Where(x => !x.IsUsed && x.CanBeRemoved);
            //Debug.WriteLine($"Got {elementsToBeDeleted.Count()} to be deleted");
            //_cleanerStore.DeleteElements(_modelType, elementsToBeDeleted);


            //List<IElement> currentElementList = _elementCollections[modelType];
            //_cleaner.DeleteElements(modelType, currentElementList);

            //List<IElement> collectionCopy = new List<IElement>(currentElementList);

            //foreach (IElement element in elements)
            //{
            //    collectionCopy.Remove(element);
            //}
            //collection.Clear();
            //collection.AddRange(collectionCopy);

            //Debug.WriteLine($"STORE: Left {collection.Count} {modelType}s");

            //OnElementDeleted(collection);
        }

        private void OnElementDeleted(IEnumerable<IElement> elements)
        {
            ElementDeleted?.Invoke(elements);
        }
    }
}