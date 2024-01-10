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
        private readonly List<LineStyle> _lineStyles;
        private readonly Cleaner _cleaner;
        private Lazy<Task> _initializeLazy;

        public IEnumerable<LineStyle> LineStyles => _lineStyles;
        public event Action<LineStyle> LineStyleDeleted;

        public CleanerStore(Cleaner cleaner)
        {
            _cleaner = cleaner;
            _initializeLazy = new Lazy<Task>(Initialize);
            _lineStyles = new List<LineStyle>();
        }
        private async Task Initialize()
        {
            Debug.WriteLine("Initializing lazy");
            IEnumerable<LineStyle> lineStyles = _cleaner.GetAllLineStyles();
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

        private void DeleteLineStyle(LineStyle lineStyle)
        {
            _lineStyles.Remove(lineStyle);

            OnLineStyleDeleted(lineStyle);
        }

        private void OnLineStyleDeleted(LineStyle lineStyle)
        {
            LineStyleDeleted?.Invoke(lineStyle);
        }


    }
}
