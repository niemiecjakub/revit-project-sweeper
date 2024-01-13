using Autodesk.Revit.DB;
using ProjectSweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSweeper.RevitFunctions
{
    public class TextFunctions
    {
        public static ISet<TextTypeModel> GetAllText(Document doc)
        {
            ISet<TextTypeModel> textModelList = new HashSet<TextTypeModel>();

            FilteredElementCollector textNoteTypeCollector = new FilteredElementCollector(doc);
            ICollection<TextNoteType> textNoteTypes = textNoteTypeCollector.OfClass(typeof(TextNoteType)).Cast<TextNoteType>().ToList();

            foreach (TextNoteType textNoteType in textNoteTypes)
            {
                TextTypeModel textModel = new TextTypeModel(textNoteType);
                textModel.CanBeRemoved = DocumentFunctions.CanBeRemoved(doc, textNoteType.Id);
                textModelList.Add(textModel);
            }

            SetUsedText(doc, textModelList);

            return textModelList;
        }

        public static void SetUsedText(Document doc, ISet<TextTypeModel> textModelList)
        {
            FilteredElementCollector textNoteCollector = new FilteredElementCollector(doc);
            ICollection<TextNote> textNotes = textNoteCollector.OfClass(typeof(TextNote)).Cast<TextNote>().ToList();

            foreach (TextNote textNote in textNotes)
            {
                TextNoteType textNoteType = textNote.TextNoteType;
                ElementId textNoteTypeId = textNoteType.Id;
                TextTypeModel textModel = textModelList.FirstOrDefault(t => t.Id == textNoteTypeId);
                if (textModel == null) { continue; }
                textModel.IsUsed = true;
            }

            FilteredElementCollector viewScheduleCollector = new FilteredElementCollector(doc);
            IList<ViewSchedule> viewSchedules = viewScheduleCollector.OfClass(typeof(ViewSchedule)).Cast<ViewSchedule>().ToList();

            foreach (ViewSchedule viewSchedule in viewSchedules)
            {
                ISet<ElementId> textList = new HashSet<ElementId>()
                {
                    viewSchedule.BodyTextTypeId,
                    viewSchedule.HeaderTextTypeId,
                    viewSchedule.TitleTextTypeId
                };
                foreach (ElementId textId in textList)
                {
                    TextTypeModel textModel = textModelList.FirstOrDefault(t => t.Id == textId);
                    if (textModel == null) { continue; };
                    textModel.IsUsed = true;
                }
            }


            //FilteredElementCollector collector1 = new FilteredElementCollector(doc);
            //IList<View> views = collector1.OfClass(typeof(View)).Cast<View>().ToList();

            //Debug.WriteLine(views.Where(v => v.IsTemplate).Count());

            //foreach (View view in views)
            //{
            //    if (!view.IsTemplate) { continue; }
            //    ElementId viewTemplateId = view.ViewTemplateId;
            //    var template = doc.GetElement(viewTemplateId);


            //}
        }

    }
}
