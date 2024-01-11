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
        private readonly List<LineStyleModel> _lineStyles;
        private readonly List<LinePatternModel> _linePatterns;

        public IEnumerable<LineStyleModel> LineStyles => _lineStyles;
        public IEnumerable<LinePatternModel> LinePatterns => _linePatterns;

        public event Action<IEnumerable<LineStyleModel>> LineStyleDeleted;
        public event Action<IEnumerable<LinePatternModel>> LinePatternDeleted;

        public CleanerStore(Cleaner cleaner)
        {
            _cleaner = cleaner;
            _initializeLazy = new Lazy<Task>(Initialize);
            _lineStyles = new List<LineStyleModel>();
            _linePatterns = new List<LinePatternModel>();
        }
        private async Task Initialize()
        {
            Debug.WriteLine("Initializing lazy");
            IEnumerable<LineStyleModel> lineStyles = await _cleaner.GetAllLineStyles();
            _lineStyles.Clear();
            _lineStyles.AddRange(lineStyles);

            IEnumerable<LinePatternModel> linePatterns = await _cleaner.GetAllLinePatterns();
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

        public async Task DeleteLineStyle(IEnumerable<LineStyleModel> lineStyles)
        {
            Debug.WriteLine("STORE: Inside store");
            await _cleaner.LineStyleDeleted(lineStyles);

            List<LineStyleModel> lineStylesCopy = new List<LineStyleModel>(_lineStyles);

            foreach (LineStyleModel ls in lineStyles)
            {
                lineStylesCopy.Remove(ls);
            }

            // Assign the updated collection back to _lineStyles
            _lineStyles.Clear();
            _lineStyles.AddRange(lineStylesCopy);

            Debug.WriteLine($"STORE: Left {_lineStyles.Count} linestyles");

            OnLineStyleDeleted(_lineStyles);
        }

        private void OnLineStyleDeleted(IEnumerable<LineStyleModel> lineStyles)
        {
            LineStyleDeleted?.Invoke(lineStyles);
        }


        public async Task DeleteLinePattern(IEnumerable<LinePatternModel> linePatterns)
        {
            Debug.WriteLine("STORE: Inside store");
            await _cleaner.LinePatternDeleted(linePatterns);

            List<LinePatternModel> linePatternsCopy = new List<LinePatternModel>(_linePatterns);

            foreach (LinePatternModel lp in linePatterns)
            {
                linePatternsCopy.Remove(lp);
            }

            // Assign the updated collection back to _lineStyles
            _linePatterns.Clear();
            _linePatterns.AddRange(linePatternsCopy);

            Debug.WriteLine($"STORE: Left {_linePatterns.Count} linepatterns");

            OnLinePatternDeleted(_linePatterns);
        }

        private void OnLinePatternDeleted(IEnumerable<LinePatternModel> linePatterns)
        {
            LinePatternDeleted?.Invoke(linePatterns);
        }


    }
}