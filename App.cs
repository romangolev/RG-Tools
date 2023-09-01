using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using System.Reflection;

namespace RG_Tools
{

    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            //Get current dll location
            string path = Assembly.GetExecutingAssembly().Location;


            // Pass language type to the ribbin designer which will preserve it for the futere queries
            RibbonDesigner.LangtypeApp = application.ControlledApplication.Language;
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
            RibbonPanel Panel3 = application.CreateRibbonPanel(tabName, "Misc Tools");


            /// Panel 1 Info
            /// 
            // Create pushbutton Help
            PushButtonData buttondata1_1 =
                new PushButtonData("Button1_1", "Help", path, "RG_Tools.Help");
            PushButton button1_1 = (PushButton)Panel1.AddItem(buttondata1_1);
            button1_1.LargeImage = Helper.GetIconSource(Properties.Icons.info24);
            button1_1.Image = Helper.GetIconSource(Properties.Icons.info16);
            button1_1.ToolTip = RibbonDesigner.GetLocalizedResource( "InfoTooltip");

            ///Panel 1 Info
            ///Panel 2 Coordination


            // Declare and add pushbutton Coord Plan
            PushButtonData buttondata2_3 =
                new PushButtonData("Button2_4", "Coord\nPlan", path, "RG_Tools.CoordPlan");
            // Declare and add pushbutton Navis View

            PushButton button2_3 = (PushButton)Panel2.AddItem(buttondata2_3);
            button2_3.LargeImage = Helper.GetIconSource(Properties.Icons.coordplan24);
            button2_3.Image = Helper.GetIconSource(Properties.Icons.coordplan16);
            button2_3.ToolTip = RibbonDesigner.GetLocalizedResource( "CoordPlanTooltip");

            PushButtonData buttondata2_4 =
                new PushButtonData("Button2_3", "Navis\nView", path, "RG_Tools.NavisView");
            PushButton button2_4 = (PushButton)Panel2.AddItem(buttondata2_4);
            button2_4.LargeImage = Helper.GetIconSource(Properties.Icons.navis24);
            button2_4.Image = Helper.GetIconSource(Properties.Icons.navis16);
            button2_4.ToolTip = RibbonDesigner.GetLocalizedResource("NavisViewTooltip");


            // Create pushbutton Worksets 3D
            PushButtonData buttondata2_2 =
                new PushButtonData("Button2_2", "Worksets\n3D Views", path, "RG_Tools.Worksets3D");
            PushButton button2_2 = (PushButton)Panel2.AddItem(buttondata2_2);
            button2_2.LargeImage = Helper.GetIconSource(Properties.Icons.workset3d24);
            button2_2.Image = Helper.GetIconSource(Properties.Icons.workset3d16);
            button2_2.ToolTip = RibbonDesigner.GetLocalizedResource("Worksets3DTooltip");

            // Declare
            PushButtonData buttondata2_5 =
                new PushButtonData("Button2_5", "Links 3D", path, "RG_Tools.Links3D");
            // Declare
            PushButtonData buttondata2_6 =
                new PushButtonData("Button2_6", "Link more ..", path, "RG_Tools.LinkMore");
            // Declare
            PushButtonData buttondata2_7 =
                new PushButtonData("Button2_7", "Review Links", path, "RG_Tools.ReviewLinks");

            IList<RibbonItem> stackedGroup2_1 = Panel2.AddStackedItems(buttondata2_5, buttondata2_6, buttondata2_7);

            PushButton button2_5 = stackedGroup2_1[0] as PushButton;
            button2_5.LargeImage = Helper.GetIconSource(Properties.Icons.viewlink24);
            button2_5.Image = Helper.GetIconSource(Properties.Icons.viewlink16);
            button2_5.ToolTip = RibbonDesigner.GetLocalizedResource("Links3DTooltip");
            PushButton button2_6 = stackedGroup2_1[1] as PushButton;
            button2_6.LargeImage = Helper.GetIconSource(Properties.Icons.limany24);
            button2_6.Image = Helper.GetIconSource(Properties.Icons.limany16);
            button2_6.ToolTip = RibbonDesigner.GetLocalizedResource("LinkMoreTooltip");
            PushButton button2_7 = stackedGroup2_1[2] as PushButton;
            button2_7.LargeImage = Helper.GetIconSource(Properties.Icons.reviewlinks24);
            button2_7.Image = Helper.GetIconSource(Properties.Icons.reviewlinks16);
            button2_7.ToolTip = RibbonDesigner.GetLocalizedResource("ReviewLinksTooltip");
            
            
            // Declare
            PushButtonData buttondata2_8 =
                new PushButtonData("Button2_8", "Join Many Elements", path, "RG_Tools.ManyJoin");
            // Declare
            PushButtonData buttondata2_9 =
                new PushButtonData("Button2_9", "All Elements of Category", path, "RG_Tools.SelectElementsByCategory");
            // Declare
            PushButtonData buttondata2_10 = 
                new PushButtonData("Button2_10", "Delete Shared Parameter", path, "RG_Tools.DeleteShared");

            IList<RibbonItem> stackedGroup2_2 = Panel3.AddStackedItems(buttondata2_8, buttondata2_9, buttondata2_10);
            PushButton button2_8 = stackedGroup2_2[0] as PushButton;
            button2_8.LargeImage = Helper.GetIconSource(Properties.Icons.join24);
            button2_8.Image = Helper.GetIconSource(Properties.Icons.join16);
            button2_8.ToolTip = RibbonDesigner.GetLocalizedResource("ManyJoinTooltip");
            PushButton button2_9 = stackedGroup2_2[1] as PushButton;
            button2_9.LargeImage = Helper.GetIconSource(Properties.Icons.allelems24);
            button2_9.Image = Helper.GetIconSource(Properties.Icons.allelems16);
            button2_9.ToolTip = RibbonDesigner.GetLocalizedResource("SelectElementsByCategoryTooltip");
            PushButton button2_10 = stackedGroup2_2[2] as PushButton;
            button2_10.LargeImage = Helper.GetIconSource(Properties.Icons.delpar24);
            button2_10.Image = Helper.GetIconSource(Properties.Icons.delpar16);
            button2_10.ToolTip = RibbonDesigner.GetLocalizedResource("DeleteSharedTooltip");
            // Declare CopyValues Button
            PushButtonData buttondata2_11 =
                new PushButtonData("Button2_11", "Copy Values", path, "RG_Tools.CopyValues.CopyValues");
            PushButton button2_11 = (PushButton)Panel3.AddItem(buttondata2_11);
            button2_11.LargeImage = Helper.GetIconSource(Properties.Icons.cp24);
            button2_11.Image = Helper.GetIconSource(Properties.Icons.cp16);
            button2_11.ToolTip = RibbonDesigner.GetLocalizedResource("CopyValuesTooltip");


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}