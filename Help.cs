using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RG_Tools
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Help : IExternalCommand
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
            Application application = uiapp.Application;

            TaskDialog help = new TaskDialog("RG Tools Help")
            {
                MainInstruction = "Ask for help" + "\r\n" +
                "Roman Golev" + "\r\n" +
                "https://www.romangolev.com",
                MainContent = "Revit Version Name is: " + application.VersionName + "\n"
                + "Revit Version Number is: " + application.VersionNumber + "\n"
                + "Revit Version Build is: " + application.VersionBuild + "\n"
                + "Active document: " + doc.Title + "\n"
                + "Active view name: " + doc.ActiveView.Name + "\n"
                + "Revit Language: " + application.Language + "\n"
                + "Username: " + application.Username
            };
            help.Show();
            return Result.Succeeded;
        }
    }
}
