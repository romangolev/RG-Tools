using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RG_Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;


namespace RG_Tools
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class DeleteShared : IExternalCommand
    {
        public static IList<Element> _collector = null;
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            // Get Document
            Document doc = uidoc.Document;
            // Get UIApplication
            UIApplication uiapp = commandData.Application;
            // Get app
            Application app = uiapp.Application;

            IList<Element> cParams = GetSharedParameters(doc);

            if (cParams.Count() != 0)
            {

                ObservableCollection<SharedParamWrapper> sharedparamswrapped = new ObservableCollection<SharedParamWrapper>();
                foreach (SharedParameterElement element in cParams)
                {
                    sharedparamswrapped.Add(new SharedParamWrapper() { Id = element.Id.IntegerValue, Name = element.GetDefinition().Name + " [" + element.Id.ToString() + "]", Param = element, IsInstance = true });
                }

                //EXTERNAL EVENTS WITH ARGUMENTS
                EventHandlerWithStringArg evStr = new EventHandlerWithStringArg();
                DeleteSharedWPFWrappedEvent evWpf = new DeleteSharedWPFWrappedEvent();

                var window = new DeleteSharedWPF(uiapp, evStr, evWpf);
                //window.ExEvent = exEvent;
                window.Show();

                viewmodelDeleteShared vModel = new viewmodelDeleteShared
                {
                    CollectedParams = cParams,
                    SharedParamsWrapped = sharedparamswrapped
                };
                window.DataContext = vModel;

                return Result.Succeeded;
            } else
            {
                System.Windows.MessageBox.Show("There are no shared parameters in the project", "Cancelled");
                return Result.Cancelled;
            }


        }

        public static IList<Element> GetSharedParameters(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).
                                            WhereElementIsNotElementType();
            IList<Element> elements = collector.OfClass(typeof(SharedParameterElement)).ToElements();
            return elements;
        }

    }
}
