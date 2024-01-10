using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.ViewModels
{
    public class LinePatternViewModel : ViewModelBase
    {
        public string Name { get; set; }

        public LinePatternViewModel()
        {
            Debug.WriteLine("new line pattern");
            Name = "pattern";
        }
    }
}
