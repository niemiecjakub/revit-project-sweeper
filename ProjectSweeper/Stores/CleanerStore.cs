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

        public event Action<IEnumerable<IElement>> ElementDeleted;

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



        public void DeleteElemenets(IEnumerable<IElement> elements)
        {
            Debug.WriteLine("STORE: Inside store");
            IEnumerable<ModelTypes> modelTypes = elements.Select(e => e.ModelType);
            bool allSameModelType = modelTypes.All(mt => mt.Equals(modelTypes.First()));
            if (!allSameModelType)
            {
                Debug.WriteLine("Elements are not the same model type");
                return;
            }

            ModelTypes modelType = modelTypes.First();
            switch (modelType)
            {
                case ModelTypes.LinePattern:
                    _cleaner.LineStyleDeleted(elements);

                    List<IElement> linePatternsCopy = new List<IElement>(_linePatterns);

                    foreach (IElement lp in elements)
                    {
                        linePatternsCopy.Remove(lp);
                    }

                    // Assign the updated collection back to _lineStyles
                    _linePatterns.Clear();
                    _linePatterns.AddRange(linePatternsCopy);

                    Debug.WriteLine($"STORE: Left {_linePatterns.Count} linepatterns");

                    OnLinePatternDeleted(_linePatterns);

                    break;


                case ModelTypes.LineStyle:
                    _cleaner.LinePatternDeleted(elements);

                    List<IElement> lineStylesCopy = new List<IElement>(_lineStyles);

                    foreach (IElement ls in elements)
                    {
                        lineStylesCopy.Remove(ls);
                    }

                    // Assign the updated collection back to _lineStyles
                    _lineStyles.Clear();
                    _lineStyles.AddRange(lineStylesCopy);

                    Debug.WriteLine($"STORE: Left {_lineStyles.Count} linestyles");

                    OnLineStyleDeleted(_lineStyles);

                    break;
            }
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