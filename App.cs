﻿using System;
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
            RibbonPanel Panel3 = application.CreateRibbonPanel(tabName, "Misc Tools");
            //RibbonPanel Panel4 = application.CreateRibbonPanel(tabName, "Validation");
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



            // Declare and add pushbutton Navis View
            PushButtonData buttondata2_3 =
                new PushButtonData("Button2_3", "Navis\nView", path, "RG_Tools.NavisView");
            PushButton button2_3 = (PushButton)Panel2.AddItem(buttondata2_3);
            button2_3.LargeImage = Helper.GetIconSource(Icons.navis24);
            button2_3.Image = Helper.GetIconSource(Icons.navis16);
            button2_3.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "NavisViewTooltip");

            // Declare and add pushbutton Coord Plan
            PushButtonData buttondata2_4 =
                new PushButtonData("Button2_4", "Coord\nPlan", path, "RG_Tools.CoordPlan");
            PushButton button2_4 = (PushButton)Panel2.AddItem(buttondata2_4);
            button2_4.LargeImage = Helper.GetIconSource(Icons.coordplan24);
            button2_4.Image = Helper.GetIconSource(Icons.coordplan16);
            button2_4.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "CoordPlanTooltip");

            /*
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
            */

            // Create pushbutton Worksets 3D
            PushButtonData buttondata2_2 =
                new PushButtonData("Button2_2", "Worksets\n3D Views", path, "RG_Tools.Worksets3D");
            PushButton button2_2 = (PushButton)Panel2.AddItem(buttondata2_2);
            button2_2.LargeImage = Helper.GetIconSource(Icons.workset3d24);
            button2_2.Image = Helper.GetIconSource(Icons.workset3d16);
            button2_2.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "Worksets3DTooltip");

            // Declare
            PushButtonData buttondata2_5 =
                new PushButtonData("Button2_5", "Links 3D", path, "RG_Tools.Links3D");
            // Declare
            PushButtonData buttondata2_6 =
                new PushButtonData("Button2_6", "Link More ..", path, "RG_Tools.LinkMore");
            // Declare
            PushButtonData buttondata2_7 =
                new PushButtonData("Button2_7", "Review Links", path, "RG_Tools.ReviewLinks");

            IList<RibbonItem> stackedGroup2_1 = Panel2.AddStackedItems(buttondata2_5, buttondata2_6, buttondata2_7);

            PushButton button2_5 = stackedGroup2_1[0] as PushButton;
            button2_5.LargeImage = Helper.GetIconSource(Icons.viewlink24);
            button2_5.Image = Helper.GetIconSource(Icons.viewlink16);
            button2_5.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "SelectElementsByCategoryTooltip");
            PushButton button2_6 = stackedGroup2_1[1] as PushButton;
            button2_6.LargeImage = Helper.GetIconSource(Icons.limany24);
            button2_6.Image = Helper.GetIconSource(Icons.limany16);
            button2_6.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "LinkMoreTooltip");
            PushButton button2_7 = stackedGroup2_1[2] as PushButton;
            button2_7.LargeImage = Helper.GetIconSource(Icons.reviewlinks24);
            button2_7.Image = Helper.GetIconSource(Icons.reviewlinks16);
            button2_7.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "ReviewLinksTooltip");
            
            
            // Declare
            PushButtonData buttondata2_8 =
                new PushButtonData("Button2_8", "Many Join", path, "RG_Tools.ManyJoin");
            // Declare
            PushButtonData buttondata2_9 =
                new PushButtonData("Button2_9", "Select All ..", path, "RG_Tools.SelectElementsByCategory");
            // Declare
            PushButtonData buttondata2_10 = 
                new PushButtonData("Button2_10", "Review Links", path, "RG_Tools.ReviewLinks");

            IList<RibbonItem> stackedGroup2_2 = Panel3.AddStackedItems(buttondata2_8, buttondata2_9, buttondata2_10);

            // Declare
            PushButtonData buttondata2_11 =
                new PushButtonData("Button2_11", "Copy Values", path, "RG_Tools.CopyValues");
            PushButton button2_11 = (PushButton)Panel3.AddItem(buttondata2_11);
            button2_11.LargeImage = Helper.GetIconSource(Icons.reviewlinks24);
            button2_11.Image = Helper.GetIconSource(Icons.reviewlinks16);
            button2_11.ToolTip = RibbonDesigner.GetLocalizedResource(lang, "CopyValuesTooltip");

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}