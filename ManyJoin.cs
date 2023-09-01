using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Exceptions;
using System.Collections;
using System.Collections.Generic;
using static RG_Tools.Helpers.SelectionAndFiltering;

namespace RG_Tools
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ManyJoin : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            //UIDocument uiDocument = new UIDocument(document);
            Selection sel = uiApp.ActiveUIDocument.Selection;
            ISelectionFilter iselectionFilter = new FilterCategoryJoinable();

            string refpickmessage1 = RibbonDesigner.GetLocalizedResource("ManyJoinPrompt1");
            string refpickmessage2 = RibbonDesigner.GetLocalizedResource("ManyJoinPrompt2");

            Transaction transaction = new Transaction(doc);
            transaction.Start("Many join");
            try
            {
                
                try
                {
                    IList<Reference> referenceList1 = sel.PickObjects(ObjectType.Element, iselectionFilter, refpickmessage1);
                    IList<Reference> referenceList2 = sel.PickObjects(ObjectType.Element, iselectionFilter, refpickmessage2);
                    using IEnumerator<Reference> enumerator1 = referenceList1.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        Reference current1 = enumerator1.Current;
                        using IEnumerator<Reference> enumerator2 = referenceList2.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Reference current2 = enumerator2.Current;
                            try
                            {
                                JoinGeometryUtils.JoinGeometry(doc, doc.GetElement(current1), doc.GetElement(current2));
                            }
                            catch
                            {

                            }
                        }
                    }

                } catch (OperationCanceledException)
                {
                    transaction.RollBack();
                    return Result.Cancelled;
                }

                transaction.Commit();
                message = "Succesess";
                return Result.Succeeded;
            }

            catch (AutoJoinFailedException e)
            {
                message = e.Message;
                transaction.Commit();
                return Result.Succeeded;
            }
        }
    }
}
