using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using RG_Tools.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Xml.Linq;
using AW = Autodesk.Windows;

namespace RG_Tools
{
    public static class RibbonDesigner
    {
        
        #region LangtypeApp Constructor
        private static LanguageType _LangtypeApp ;
        public static LanguageType LangtypeApp 
        {
            get { return _LangtypeApp; }
            set { _LangtypeApp = value; } 
        }
        #endregion

        // This method Creates Tab by defined name
        public static void CreateOrCheckPanel(UIControlledApplication application, string tabName)
        {
            var ribbonPanel = application.GetRibbonPanels(Tab.AddIns).Find(x => x.Name == tabName);
            if (ribbonPanel == null)
                application.CreateRibbonTab(tabName);
        }

        public static string GetLocalizedResource(string name)
        {
            switch (LangtypeApp)
            {
                case LanguageType.English_USA:
                    return ResourcesEN.ResourceManager.GetString(name);
                case LanguageType.English_GB:
                    return ResourcesEN.ResourceManager.GetString(name);
                case LanguageType.Spanish:
                    return ResourcesES.ResourceManager.GetString(name);
                case LanguageType.Russian:
                    return ResourcesRU.ResourceManager.GetString(name);
                default:
                    return ResourcesEN.ResourceManager.GetString(name);
            }
            
        }
        public static void StackSplitButton(string tabname, RibbonPanel panel, RibbonItem ribbonItem)
        {
            string sid = string.Join(
             "%",
             "CustomCtrl_",
             "CustomCtrl_",
             tabname,
             panel.Name,
             ribbonItem.Name);

            var splitButton = (Autodesk.Windows.RibbonSplitButton)
              UIFramework.RevitRibbonControl.RibbonControl
                .findRibbonItemById(sid);

            splitButton.IsSplit = true;
            splitButton.IsSynchronizedWithCurrentItem = true;
        }

        public static string GetId(this RibbonItem ribbonItem)
        {
            var type = typeof(RibbonItem);

            var parentId = type
                .GetField("m_parentId", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(ribbonItem) ?? string.Empty;

            var generateIdMethod = type
                .GetMethod("generateId", BindingFlags.Static | BindingFlags.NonPublic);

            return (string)generateIdMethod?.Invoke(ribbonItem, new[] { parentId, ribbonItem.Name });
        }
        public static AW.RibbonItem GetButton(string tabName, string panelName, string itemName)
        {
            AW.RibbonControl ribbon = AW.ComponentManager.Ribbon;
            foreach (AW.RibbonTab tab in ribbon.Tabs)
            {
                if (tab.Name == tabName)
                {
                    foreach (AW.RibbonPanel panel in tab.Panels)
                    {
                        if (panel.Source.Title == panelName)
                        {
                            return panel.FindItem("CustomCtrl_%CustomCtrl_%" + tabName + "%" + panelName + "%" + itemName, true) as AW.RibbonItem;
                        }
                    }
                }
            }
            return null;
        }
    }
}
