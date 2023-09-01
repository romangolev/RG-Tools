using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using RG_Tools;

namespace RG_Tools
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Links3D : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            // Get Document
            Document doc = uidoc.Document;

            //find the linked files
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<ElementId> elementIdSet =
              collector.OfCategory(BuiltInCategory.OST_RvtLinks)
              .OfClass(typeof(RevitLinkType))
              .ToElementIds();

            ICollection<Element> links =
              collector.OfCategory(BuiltInCategory.OST_RvtLinks)
              //.WhereElementIsNotElementType()
              .ToElements();

            ICollection<ElementId> elementIdSet2 =
              collector.OfCategory(BuiltInCategory.OST_RvtLinks)
              .OfClass(typeof(RevitLinkType))
              .ToElementIds();

            if (links.Count > 0)
            {
                Transaction transaction = new Transaction(doc);

                if (elementIdSet2.Count > 0)
                {
                    foreach (ElementId link in elementIdSet)
                    {
                        if (link != null)
                        {
                            transaction.Start("Views for links");
                            try
                            {
                                View3D lview = create3D(doc, link, elementIdSet2);
                                transaction.Commit();
                                uidoc.ActiveView = lview;
                            }
                            catch
                            {
                                transaction.RollBack();
                                continue;
                            }
                        }
                    }
                }
                return Result.Succeeded;
            }
            else if (links.Count == 0)
            {
                TaskDialog.Show("Error", "There are no RVT links in project");
                return Result.Cancelled;
            }
            else { return Result.Failed; }
        }

        public static View3D create3D(Document doc, ElementId link, ICollection<ElementId> elementIdSet)
        {
            View3D view = View3D.CreateIsometric(doc, Helper.get3DviewType(doc));
            view.Name = doc.GetElement(link).Name;
            elementIdSet.Remove(link);
            view.HideElements(elementIdSet);
            elementIdSet.Add(link);
            return view;
        }
    }
}