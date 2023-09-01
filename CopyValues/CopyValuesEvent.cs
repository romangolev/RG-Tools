using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RG_Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG_Tools
{
    public class CopyValuesWPFWrappedEvent : RevitEventWrapper<CopyValuesWPF>
    {
        /// <summary>
        /// The Execute override void must be present in all methods wrapped by the RevitEventWrapper.
        /// This defines what the method will do when raised externally.
        /// </summary>
        public override void Execute(UIApplication uiApp, CopyValuesWPF ui)
        {
            // SETUP
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            // Get UIApplication
            UIApplication uiapp = uiDoc.Application;
            // Get app
            Application app = uiapp.Application;

            // Pass necessery predifined parameters from UI 
            ParameterWrapper SelectedFromCombo = null;
            ui.Dispatcher.Invoke(() => SelectedFromCombo = (ParameterWrapper)ui.ComboFrom.SelectedItem);

            ParameterWrapper SelectedToCombo = null;
            ui.Dispatcher.Invoke(() => SelectedToCombo = (ParameterWrapper)ui.ComboTo.SelectedItem);

            ICollection<ElementId> Selected = null;
            ui.Dispatcher.Invoke(() => Selected = (ICollection<ElementId>)ui.Selectedui);


            ui.Close();
            MethodsCopyValues.ExecuteCopyValues(uiApp, Selected, SelectedFromCombo, SelectedToCombo);

        }

    }
}
