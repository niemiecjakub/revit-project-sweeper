using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.RevitFunctions
{
    public class SampleExternalEvent : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            Debug.WriteLine("SAMPLE EVENT");
            UIDocument uidoc = uiapp.ActiveUIDocument;
            if (null == uidoc)
            {
                Debug.WriteLine("No document");
                return; // no document, nothing to do
            }
            Document doc = uidoc.Document;
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("MyEvent");
                Debug.WriteLine("transaction started");
                // Action within valid Revit API context thread
                tx.Commit();
            }
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }
    }

}

