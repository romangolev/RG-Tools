using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RG_Tools.CopyValues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG_Tools
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class RevitBridgeCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            // Get transaction
            Transaction transaction = new Transaction(doc);
            try
            {
                // Get all the wall types in the current project and convert them in a Dictionary.
                FilteredElementCollector felc = new FilteredElementCollector(doc).OfClass(typeof(WallType));
                Dictionary<string, int> dicwtypes = felc.ToDictionary(x => x.Name, y => y.Id.IntegerValue);
                felc.Dispose();

                List<Parameter> instParams = getInstParameters(uidoc, doc);
                List<Parameter> typeParams = getTypeParameters(uidoc, doc);

                List<Parameter> mergedParams = new List<Parameter>();
                mergedParams.AddRange(instParams);
                mergedParams.AddRange(typeParams);
                ObservableCollection<ParameterWrapper> parametersAll = new ObservableCollection<ParameterWrapper>();
                foreach (Parameter param in mergedParams)
                {
                    parametersAll.Add(new ParameterWrapper() { Id = 1, Name = param.Definition.Name, Param = param });
                }

                // Create a view model that will be associated to the DataContext of the view.
                viewmodelRevitBridge vmod = new viewmodelRevitBridge();
                vmod.DicWallType = dicwtypes;
                vmod.SelectedWallType = dicwtypes.First().Value;
                vmod.Params = parametersAll;
                // Create a new Revit model and assign it to the Revit model variable in the view model.
                vmod.RevitModel = new modelRevitBridge(uiapp);

                System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();

                // Load the WPF window viewRevitbridge.
                viewRevitBridge view = new viewRevitBridge();
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(view);
                helper.Owner = proc.MainWindowHandle;

                // Assign the view model to the DataContext of the view.
                view.DataContext = vmod;

                if (view.ShowDialog() != true)
                {
                    return Result.Cancelled;
                }
                

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        public static List<Parameter> getInstParameters(UIDocument uidoc, Document doc)
        {
            ICollection<ElementId> selectedElements = uidoc.Selection.GetElementIds();
            List<Parameter> instParams = new List<Parameter>();
            foreach (ElementId elementId in selectedElements)
            {
                Element elem = doc.GetElement(elementId);
                if (elem != null)
                {
                    foreach (Parameter param in elem.GetOrderedParameters())
                    {
                        if (param != null)
                        {
                            instParams.Add(param);
                        }
                    }
                }
            }
            return instParams;

        }
        public static List<Parameter> getTypeParameters(UIDocument uidoc, Document doc)
        {
            ICollection<ElementId> selectedElements = uidoc.Selection.GetElementIds();
            List<Parameter> instParams = new List<Parameter>();
            List<Parameter> typeParams = new List<Parameter>();


            foreach (ElementId elementId in selectedElements)
            {
                Element elem = doc.GetElement(elementId);
                Element elemType = doc.GetElement(elem.GetTypeId());
                if (elemType != null)
                {
                    foreach (Parameter param in elemType.GetOrderedParameters())
                    {
                        if (param != null)
                        {
                            typeParams.Add(param);
                        }
                    }
                }
            }
            return typeParams;
        }
    }
}
