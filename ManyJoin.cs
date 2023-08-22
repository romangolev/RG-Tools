using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Exceptions;
using System.Collections;
using System.Collections.Generic;

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
            ISelectionFilter iselectionFilter = new Filter_category();
            IList<Reference> referenceList1 = sel.PickObjects(ObjectType.Element, iselectionFilter, "Выбор объекта К КОТОРУМУ ПРИСОЕДЕНЯЮТ (СТЕНА, ПЕРЕКРЫТИЕ, ПОТОЛОК)");
            IList<Reference> referenceList2 = sel.PickObjects(ObjectType.Element, iselectionFilter, "Выбор объекта КОТОРЫЙ ПРИСОЕДЕНЯЕТСЯ");

            Transaction transaction = new Transaction(doc);
            transaction.Start("Many join");
            try
            {
                using (IEnumerator<Reference> enumerator1 = ((IEnumerable<Reference>)referenceList1).GetEnumerator())
                {
                    while (((IEnumerator)enumerator1).MoveNext())
                    {
                        Reference current1 = enumerator1.Current;
                        using (IEnumerator<Reference> enumerator2 = ((IEnumerable<Reference>)referenceList2).GetEnumerator())
                        {
                            while (((IEnumerator)enumerator2).MoveNext())
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
                    }
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
            catch (OperationCanceledException e)
            {
                message = e.Message;
                transaction.RollBack();
                return Result.Cancelled;

            }
        }
    }
}
