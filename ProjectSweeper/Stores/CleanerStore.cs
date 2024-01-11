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
        private readonly List<LineStyle> _lineStyles;

        public IEnumerable<LineStyle> LineStyles => _lineStyles;

        public event Action<IEnumerable<LineStyle>> LineStyleDeleted;

        public CleanerStore(Cleaner cleaner)
        {
            _cleaner = cleaner;
            _initializeLazy = new Lazy<Task>(Initialize);
            _lineStyles = new List<LineStyle>();
        }
        private async Task Initialize()
        {
            Debug.WriteLine("Initializing lazy");
            IEnumerable<LineStyle> lineStyles = await _cleaner.GetAllLineStyles();
            _lineStyles.Clear();
            _lineStyles.AddRange(lineStyles);
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

        public async Task DeleteLineStyle(IEnumerable<LineStyle> lineStyle)
        {
            Debug.WriteLine("STORE: Inside store");
            await _cleaner.LineStyleDeleted(lineStyle);

            // Create a copy of the collection to avoid "collection was modified" error
            List<LineStyle> lineStylesCopy = new List<LineStyle>(_lineStyles);

            foreach (LineStyle ls in lineStyle)
            {
                lineStylesCopy.Remove(ls);
            }

            // Assign the updated collection back to _lineStyles
            _lineStyles.Clear();
            _lineStyles.AddRange(lineStylesCopy);

            Debug.WriteLine($"STORE: Left {_lineStyles.Count} linestyles");

            OnLineStyleDeleted(_lineStyles);
        }

        private void OnLineStyleDeleted(IEnumerable<LineStyle> lineStyle)
        {
            LineStyleDeleted?.Invoke(lineStyle);
        }


    }
}
