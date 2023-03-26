using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using RG_Tools;
using RG_Tools.Properties;
using System.Reflection;
using System.Resources;

namespace RG_Tools
{ 

    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            //Get current dll location
            string path = Assembly.GetExecutingAssembly().Location;

            // Determing what Language does the current instance of application use
            LanguageType lang = application.ControlledApplication.Language;

            // Introduce Tab name and make sure it does exist
            string tabName = "RG Tools";
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch
            {
                //If not possible to create Tab, then Tab already exists
            }

            // Create Panels
            RibbonPanel Panel1 = application.CreateRibbonPanel(tabName, "Info");
            RibbonPanel Panel2 = application.CreateRibbonPanel(tabName, "Coordination");
            //RibbonPanel Panel3 = application.CreateRibbonPanel(tabName, "Validation");
            //RibbonPanel Panel4 = application.CreateRibbonPanel(tabName, "Misc Tools");
            //RibbonPanel Panel6 = application.CreateRibbonPanel(tabName, "Systems");



            /// Panel 1 Info

            // Create pushbutton Help
            PushButtonData buttondata1_1 =
                new PushButtonData("Button1_1", "Help", path, "RG_Tools.Help");
            PushButton button1_1 = (PushButton)Panel1.AddItem(buttondata1_1);
            button1_1.LargeImage = Helper.GetIconSource(Icons.info24);
            button1_1.Image = Helper.GetIconSource(Icons.info16);
            button1_1.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "InfoTooltip");

            ///Panel 1 Info
            ///Panel 2 Coordination

            // Create pushbutton Review Links
            PushButtonData buttondata2_1 =
                new PushButtonData("Button2_1", "Review\nLinks", path, "RevitToolsRHI.Tools_Coordination.ReviewLinks");
            PushButton button2_1 = (PushButton)Panel2.AddItem(buttondata2_1);
            //button2_1.LargeImage = Helper.GetIconSource(Resources.reviewlinks24);
            //button2_1.Image = Helper.GetIconSource(Resources.reviewlinks16);
            button2_1.ToolTip = "Checks if Revit Links are pinned and if " +
                "they belong to a separated individual worksets " +
                "(Workset name starts from ##RVT_). Fixes those issues.";

            // Create Coordination Plan
            PushButtonData buttondata2_5 =
                new PushButtonData("Button13", "Coordination\nPlan", path, "RevitToolsRHI.Tools_Coordination.CoordPlan");

            // Declare new pulldown button
            PulldownButtonData pullDownButtonData2_1 = new PulldownButtonData("PullButton2_1", "Navis");
            // Declare new pulldown button
            PulldownButtonData pullDownButtonData2_2 = new PulldownButtonData("PullButton2_2", "Worksets");

            // Declare stacked pulldown button
            IList<RibbonItem> stackedItems2_1 = Panel2.AddStackedItems(buttondata2_5, pullDownButtonData2_1, pullDownButtonData2_2);

            PushButton button2_5 = stackedItems2_1[0] as PushButton;
            button2_5.LargeImage = Helper.GetIconSource(Icons.coordplan24);
            button2_5.Image = Helper.GetIconSource(Icons.coordplan16);
            button2_5.ToolTip = "Creates Coordination Plan for opened project";

            PulldownButton pullbutton2_1 = stackedItems2_1[1] as PulldownButton;
            Helper.StackSplitButton(tabName, Panel2, pullbutton2_1);
            PulldownButton pullbutton2_2 = stackedItems2_1[2] as PulldownButton;
            Helper.StackSplitButton(tabName, Panel2, pullbutton2_2);

            // Create pushbutton Navis
            PushButtonData buttondata2_3 =
                new PushButtonData("Button2_3", "Navis\nView", path, "RG_Tools.Tools_Coordination.NavisView");
            PushButton button2_3 = pullbutton2_1.AddPushButton(buttondata2_3);
            button2_3.LargeImage = Helper.GetIconSource(Icons.navis24);
            button2_3.Image = Helper.GetIconSource(Icons.navis16);
            button2_3.ToolTip = RibbonDesigner.GetLocalizedResource(lang,"NavisViewTooltip");


               
            // Create pushbutton Navis FM
            PushButtonData buttondata2_6 =
                new PushButtonData("Button2_6", "Create Navis\nView FM", path, "RevitToolsRHI.Tools_Coordination.NavisViewFM");
            PushButton button2_6 = pullbutton2_1.AddPushButton(buttondata2_6);
            //button2_6.LargeImage = Helper.GetIconSource(Resources.navis24);
            //button2_6.Image = Helper.GetIconSource(Resources.navis16);
            button2_6.ToolTip = "Создаёт 3D вид Navis для координационного файла";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}