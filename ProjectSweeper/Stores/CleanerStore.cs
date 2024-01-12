using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.Stores
{
    public class CleanerStore
    {
        private readonly Cleaner _cleaner;
        private Lazy<Task> _initializeLazy;
        private readonly List<IElement> _lineStyles;
        private readonly List<IElement> _linePatterns;

        public IEnumerable<IElement> LineStyles => _lineStyles;
        public IEnumerable<IElement> LinePatterns => _linePatterns;

        public event Action<IEnumerable<IElement>> LineStyleDeleted;
        public event Action<IEnumerable<IElement>> LinePatternDeleted;


        public CleanerStore(Cleaner cleaner)
        {
            _cleaner = cleaner;
            _initializeLazy = new Lazy<Task>(Initialize);
            _lineStyles = new List<IElement>();
            _linePatterns = new List<IElement>();
        }
        private async Task Initialize()
        {
            Debug.WriteLine("Initializing lazy");
            IEnumerable<IElement> lineStyles = await _cleaner.GetAllLineStyles();
            _lineStyles.Clear();
            _lineStyles.AddRange(lineStyles);

            IEnumerable<IElement> linePatterns = await _cleaner.GetAllLinePatterns();
            _linePatterns.Clear();
            _linePatterns.AddRange(linePatterns);
        }

        public async Task Load()
        {
            Debug.WriteLine("Loading");
            try
            {
                await _initializeLazy.Value;
            }
            catch (Exception)
            {
                _initializeLazy = new Lazy<Task>(Initialize);
            }
        }



        public void DeleteElements(IEnumerable<IElement> elements)
        {
            Debug.WriteLine("STORE: Inside store");

            ModelTypes modelType = elements.First().ModelType;
            List<IElement> collection;
            Action<IEnumerable<IElement>> eventAction;

            switch (modelType)
            {
                case ModelTypes.LinePattern:
                    collection = _linePatterns;
                    eventAction = OnLinePatternDeleted;
                    _cleaner.LinePatternDeleted(elements);
                    break;

                case ModelTypes.LineStyle:
                    collection = _lineStyles;
                    eventAction = OnLineStyleDeleted;
                    _cleaner.LineStyleDeleted(elements);
                    break;

                default:
                    Debug.WriteLine("Elements are not the same model type");
                    return;
            }

            List<IElement> collectionCopy = new List<IElement>(collection);

            foreach (IElement element in elements)
            {
                collectionCopy.Remove(element);
            }

            // Assign the updated collection back
            collection.Clear();
            collection.AddRange(collectionCopy);

            Debug.WriteLine($"STORE: Left {collection.Count} {modelType}s");

            eventAction?.Invoke(collection);
        }


        private void OnLineStyleDeleted(IEnumerable<IElement> lineStyles)
        {
            LineStyleDeleted?.Invoke(lineStyles);
        }


        private void OnLinePatternDeleted(IEnumerable<IElement> linePatterns)
        {
            LinePatternDeleted?.Invoke(linePatterns);
        }


    }
}