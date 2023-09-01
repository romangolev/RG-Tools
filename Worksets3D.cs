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
    public class Worksets3D : IExternalCommand
    {
            

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            // Get Document
            Document doc = uidoc.Document;

            Transaction transaction = new Transaction(doc);

            FilteredElementCollector a = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType));
            ViewFamilyType vf = Helper.get_ViewType(transaction, a, doc, "Worksets3D");

            if (doc.IsWorkshared == true)
            {

                FilteredWorksetCollector wsfilter = new FilteredWorksetCollector(doc);
                IList<Workset> wslist = wsfilter.ToWorksets();
                foreach (Workset workset in wslist)
                {
                    if (workset.Kind == WorksetKind.UserWorkset)
                    {
                        transaction.Start("Create View for " + workset.Name + " Workset");
                        try
                        {
                            View3D wview = create3D(doc, workset, wslist, vf);
                            transaction.Commit();
                            uidoc.ActiveView = wview;
                        }
                        catch
                        {
                            transaction.RollBack();
                            continue;
                        }
                    }
                }
                return Result.Succeeded;
            }

            if (doc.IsWorkshared == false)
            {
                TaskDialog.Show("Error", "Model is not Workshared");
                return Result.Cancelled;
            }
            else
            {
                TaskDialog.Show("Error", "Cannot execute command");
                return Result.Failed;
            }
        }

        public static View3D create3D(Document doc, Workset workset, IList<Workset> wsilist, ViewFamilyType vft)
        {
            View3D view = View3D.CreateIsometric(doc, vft.Id);
            view.Name = workset.Name;
            foreach (Workset wss in wsilist)
            {
                if (wss == workset)
                {
                    view.SetWorksetVisibility(wss.Id, WorksetVisibility.Visible);
                    continue;
                }
                else
                {
                    view.SetWorksetVisibility(wss.Id, WorksetVisibility.Hidden);
                    continue;
                }
            }
            return view;
        }
    }
}