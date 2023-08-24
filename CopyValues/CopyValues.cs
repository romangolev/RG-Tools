using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Application = Autodesk.Revit.ApplicationServices.Application;


namespace RG_Tools.CopyValues
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CopyValues : IExternalCommand
    {

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

            BaseViewModel vModel = new BaseViewModel();
            vModel.Params = parametersAll;

            CopyValuesWPF dlg = new CopyValuesWPF();
            dlg.ShowDialog();


            return Result.Succeeded;

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
