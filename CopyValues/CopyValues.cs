using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Application = Autodesk.Revit.ApplicationServices.Application;
using RG_Tools.Helpers;
using static RG_Tools.Helpers.SelectionAndFiltering;

namespace RG_Tools.CopyValues
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    internal class CopyValues : IExternalCommand
    {
        // ModelessForm instance
        private CopyValuesWPF _mMyForm;

        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            // Get Document
            Document doc = uidoc.Document;
            // Get UIApplication
            UIApplication uiapp = commandData.Application;
            // Get app
            //Application app = uiapp.Application;

            try
            {

                //EXTERNAL EVENTS WITH ARGUMENTS
                EventHandlerWithStringArg evStr = new EventHandlerWithStringArg();
                CopyValuesWPFWrappedEvent evWpf = new CopyValuesWPFWrappedEvent();
                
                ICollection<ElementId> selectedElements = GetSelection(uidoc);

                if (selectedElements != null)
                {
                    ObservableCollection<ParameterWrapper> parametersAll = GetWrappedParameters(doc, selectedElements);

                    // The dialog becomes the owner responsible for disposing the objects given to it.
                    _mMyForm = new CopyValuesWPF(uiapp, evStr, evWpf);
                    _mMyForm.Show(); //Modeless
                                     //_mMyForm.ShowDialog(); //Modal
                    viewmodelCopyValues vModel = new viewmodelCopyValues
                    {
                        Params = parametersAll,
                        Selected = selectedElements
                    };
                    _mMyForm.Selectedui = selectedElements;
                    _mMyForm.DataContext = vModel;
                    _mMyForm.ComboFrom.SelectedIndex = 0;
                    _mMyForm.ComboTo.SelectedIndex = 1;

                    return Result.Succeeded;
                } else
                {
                    return Result.Cancelled;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error occured");
                return Result.Failed;
            }

        }


        public static ObservableCollection<ParameterWrapper> GetWrappedParameters(Document doc, ICollection<ElementId> selectedElements)
        {
            ObservableCollection<ParameterWrapper> parametersAll = new ObservableCollection<ParameterWrapper>();


            List<Parameter> instParams = GetInstParameters( doc, selectedElements);
            foreach (Parameter param in instParams)
            {
                if (param.IsShared)
                {
                    parametersAll.Add(new ParameterWrapper() { Id = param.Id.IntegerValue, Name = param.Definition.Name + " [" + param.Id.ToString() + "]", Param = param, IsInstance = true });
                }
                else
                {
                    parametersAll.Add(new ParameterWrapper() { Id = param.Id.IntegerValue, Name = param.Definition.Name, Param = param, IsInstance = true });
                }
            }
            List<Parameter> typeParams = GetTypeParameters( doc, selectedElements);
            foreach (Parameter param in typeParams)
            {
                if (param.IsShared)
                {
                    parametersAll.Add(new ParameterWrapper() { Id = param.Id.IntegerValue, Name = param.Definition.Name + " [" + param.Id.ToString() + "]", Param = param, IsInstance = true });
                }
                else
                {
                    parametersAll.Add(new ParameterWrapper() { Id = param.Id.IntegerValue, Name = param.Definition.Name, Param = param, IsInstance = false });
                }
            }
            return parametersAll;
        }

        public static List<Parameter> GetInstParameters(Document doc, ICollection<ElementId> selectedElements)
        {
            List<Parameter> instParams = new List<Parameter>();
            List<ElementId> instParamsIds = new List<ElementId>();
            foreach (ElementId elementId in selectedElements)
            {
                Element elem = doc.GetElement(elementId);
                if (elem != null)
                {
                    foreach (Parameter param in elem.GetOrderedParameters())
                    {
                        if (param != null & !instParamsIds.Contains(param.Id))
                        {
                            instParams.Add(param);
                            instParamsIds.Add(param.Id);
                        }
                    }
                }
            }
            return instParams;

        }
        public static List<Parameter> GetTypeParameters(Document doc, ICollection<ElementId> selectedElements)
        {
            List<Parameter> typeParams = new List<Parameter>();
            List<ElementId> typeParamsIds = new List<ElementId>();
            foreach (ElementId elementId in selectedElements)
            {
                Element elem = doc.GetElement(elementId);
                Element elemType = doc.GetElement(elem.GetTypeId());
                if (elemType != null)
                {
                    foreach (Parameter param in elemType.GetOrderedParameters())
                    {
                        if (param != null & !typeParamsIds.Contains(param.Id))
                        {
                            //List<ElementId> typeParamsIds = typeParams.Select(p => p.Id).ToList();
                            typeParams.Add(param);
                            typeParamsIds.Add(param.Id);
                        }
                    }
                }
            }
            return typeParams;
        }

    }
}