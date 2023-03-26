using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using RG_Tools.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RG_Tools
{
    internal class RibbonDesigner
    {
        // This method Creates Tab by defined name
        public static void CreateOrCheckPanel(UIControlledApplication application, string tabName)
        {
            var ribbonPanel = application.GetRibbonPanels(Tab.AddIns).Find(x => x.Name == tabName);
            if (ribbonPanel == null)
                application.CreateRibbonTab(tabName);
        }

        public static string GetLocalizedResource(LanguageType lang, string name)
        {
            switch (lang)
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

    }
}
