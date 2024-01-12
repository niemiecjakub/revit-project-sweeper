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

        private Lazy<Task> _initializeLazyLineStyles;
        private Lazy<Task> _initializeLazyLinePatterns;
        private Lazy<Task> _initializeLazyFilledRegions;
        private Lazy<Task> _initializeLazyFillPatterns;

        private readonly List<IElement> _lineStyles;
        private readonly List<IElement> _linePatterns;
        private readonly List<IElement> _filledRegions;
        private readonly List<IElement> _fillPatterns;

        public IEnumerable<IElement> LineStyles => _lineStyles;
        public IEnumerable<IElement> LinePatterns => _linePatterns;
        public IEnumerable<IElement> FiilledRegions => _filledRegions;
        public IEnumerable<IElement> FillPatterns => _fillPatterns;

        public event Action<IEnumerable<IElement>> LineStyleDeleted;
        public event Action<IEnumerable<IElement>> LinePatternDeleted;
        public event Action<IEnumerable<IElement>> FilledRegionDeleted;
        public event Action<IEnumerable<IElement>> FillPatternDeleted;

        public CleanerStore(Cleaner cleaner)
        {
            _cleaner = cleaner;
            _initializeLazyLineStyles = new Lazy<Task>(InitializeLineStyles);
            _initializeLazyLinePatterns = new Lazy<Task>(InitializeLinePatterns);
            _initializeLazyFilledRegions = new Lazy<Task>(InitializeFilledRegions);
            _initializeLazyFillPatterns = new Lazy<Task>(InitializeFillPatterns);

            _lineStyles = new List<IElement>();
            _linePatterns = new List<IElement>();
            _filledRegions = new List<IElement>();
            _fillPatterns = new List<IElement>();
        }
        private async Task InitializeLineStyles()
        {
            Debug.WriteLine("Initializing lazy line styles");
            IEnumerable<IElement> lineStyles = await _cleaner.GetAllLineStyles();
            _lineStyles.Clear();
            _lineStyles.AddRange(lineStyles);
        }
        private async Task InitializeLinePatterns()
        {
            Debug.WriteLine("Initializing lazy line patterns");
            IEnumerable<IElement> linePatterns = await _cleaner.GetAllLinePatterns();
            _linePatterns.Clear();
            _linePatterns.AddRange(linePatterns);
        }
        private async Task InitializeFilledRegions()
        {
            Debug.WriteLine("Initializing lazy Filled Regions");
            IEnumerable<IElement> filledRegions = await _cleaner.GetAllFilledRegions();
            _filledRegions.Clear();
            _filledRegions.AddRange(filledRegions);
        }
        private async Task InitializeFillPatterns()
        {
            Debug.WriteLine("Initializing lazy Fill Patterns");
            IEnumerable<IElement> fillPatterns = await _cleaner.GetAllFillPatterns();
            _fillPatterns.Clear();
            _fillPatterns.AddRange(fillPatterns);
        }

        public async Task LoadLineStyles()
        {
            Debug.WriteLine("Loading");
            try
            {
                await _initializeLazyLineStyles.Value;
            }
            catch (Exception)
            {
                _initializeLazyLineStyles = new Lazy<Task>(InitializeLineStyles);
            }
        }
        public async Task LoadLinePatterns()
        {
            Debug.WriteLine("Loading");
            try
            {
                await _initializeLazyLinePatterns.Value;
            }
            catch (Exception)
            {
                _initializeLazyLinePatterns = new Lazy<Task>(InitializeLinePatterns);
            }
        }
        public async Task LoadFilledRegions()
        {
            Debug.WriteLine("Loading");
            try
            {
                await _initializeLazyFilledRegions.Value;
            }
            catch (Exception)
            {
                _initializeLazyFilledRegions = new Lazy<Task>(InitializeFilledRegions);
            }
        }
        public async Task LoadFillPatterns()
        {
            Debug.WriteLine("Loading");
            try
            {
                await _initializeLazyFillPatterns.Value;
            }
            catch (Exception)
            {
                _initializeLazyFillPatterns = new Lazy<Task>(InitializeFillPatterns);
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