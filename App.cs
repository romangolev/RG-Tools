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
using AW = Autodesk.Windows;


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
                new PushButtonData("Button2_1", "Review\nLinks", path, "RG_Tools.ReviewLinks");
            PushButton button2_1 = (PushButton)Panel2.AddItem(buttondata2_1);
            button2_1.LargeImage = Helper.GetIconSource(Icons.reviewlinks24);
            button2_1.Image = Helper.GetIconSource(Icons.reviewlinks16);
            button2_1.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "ReviewLinksTooltip");

            // Create pushbutton Review Links
            PushButtonData buttondata2_2 =
                new PushButtonData("Button2_2", "Worksets\n3D", path, "RG_Tools.Worksets3D");
            PushButton button2_2 = (PushButton)Panel2.AddItem(buttondata2_2);
            button2_2.LargeImage = Helper.GetIconSource(Icons.workset3d24);
            button2_2.Image = Helper.GetIconSource(Icons.workset3d16);
            button2_2.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "ReviewLinksTooltip");


            // Declare pushbutton Navis
            PushButtonData buttondata2_3 =
                new PushButtonData("Button2_3", "Navis View", path, "RG_Tools.NavisView");
            //PushButton button2_3 = pullbutton2_1.AddPushButton(buttondata2_3);

            // Declare pushbutton Coord Plan
            PushButtonData buttondata2_4 =
                new PushButtonData("Button2_4", "Coord Plan", path, "RG_Tools.CoordPlan");

            // Create StackedItem
            IList<Autodesk.Revit.UI.RibbonItem> ribbonItem = Panel2.AddStackedItems(buttondata2_3, buttondata2_4);

            // Find Autodes.Windows.RibbonItems
            var button2_3 = RibbonDesigner.GetButton(tabName, Panel2.Name, buttondata2_3.Name);
            var button2_4 = RibbonDesigner.GetButton(tabName, Panel2.Name, buttondata2_4.Name);



            //PushButton button2_3 = (PushButton)Panel2.AddItem(buttondata2_3);
            button2_3.LargeImage = Helper.GetIconSource(Icons.navis24);
            button2_3.Image = Helper.GetIconSource(Icons.navis16);
            button2_3.ToolTip = RibbonDesigner.GetLocalizedResource(lang,"NavisViewTooltip");
            

            //PushButton button2_4 = (PushButton)Panel2.AddItem(buttondata2_4);
            button2_4.LargeImage = Helper.GetIconSource(Icons.coordplan24);
            button2_4.Image = Helper.GetIconSource(Icons.coordplan16);
            button2_4.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "CoordPlanTooltip");

            button2_3.Size = AW.RibbonItemSize.Large;
            button2_3.ShowText = true;
            button2_4.Size = AW.RibbonItemSize.Large;
            button2_4.ShowText = true;




            // Create Select Elements Button
            PushButtonData buttondata2_5 =
                new PushButtonData("Button2_5", "Select All ..", path, "RG_Tools.SelectElementsByCategory");
            // Create Select Elements Button
            PushButtonData buttondata2_6 =
                new PushButtonData("Button2_6", "Link More ..", path, "RG_Tools.LinkMore");
            // Create Select Elements Button
            PushButtonData buttondata2_7 =
                new PushButtonData("Button2_7", "Copy Values", path, "RG_Tools.CopyValues");

            IList<RibbonItem> stackedGroup2_1 = Panel2.AddStackedItems(buttondata2_5, buttondata2_6, buttondata2_7);

            PushButton button2_5 = stackedGroup2_1[0] as PushButton;
            button2_5.LargeImage = Helper.GetIconSource(Icons.allelems24);
            button2_5.Image = Helper.GetIconSource(Icons.allelems16);
            button2_5.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "SelectElementsByCategoryTooltip");

            // Create Select Elements Button
            PushButtonData buttondata2_8 =
                new PushButtonData("Button2_8", "Many Join", path, "RG_Tools.ManyJoin");
            // Create Select Elements Button
            PushButtonData buttondata2_9 =
                new PushButtonData("Button2_9", "Links 3D", path, "RG_Tools.Links3D");
            // Create Select Elements Button
            PushButtonData buttondata2_10 =
                new PushButtonData("Button2_10", "Copy Parameters", path, "RG_Tools.CopyParameters");

            IList<RibbonItem> stackedGroup2_2 = Panel2.AddStackedItems(buttondata2_8, buttondata2_9, buttondata2_10);




            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}