using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace RG_Tools
{
    public class DeleteSharedWPFWrappedEvent : RevitEventWrapper<DeleteSharedWPF>
    {
        /// <summary>
        /// The Execute override void must be present in all methods wrapped by the RevitEventWrapper.
        /// This defines what the method will do when raised externally.
        /// </summary>
        public override void Execute(UIApplication uiApp, DeleteSharedWPF ui)
        {
            // SETUP
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            // Get UIApplication
            UIApplication uiapp = uiDoc.Application;
            // Get app
            Application app = uiapp.Application;
            Transaction transaction = new Transaction(doc);
            // Pass necessery predifined parameters from UI 
            ListBox listboxMain = null;

            ui.Dispatcher.Invoke(() => listboxMain = (ListBox)ui.FindName("listbox"));
            
            IList<CheckBox> checkedList = new List<CheckBox>();
            var list = listboxMain.ItemContainerGenerator.ContainerFromIndex(0);
            for (int i = 0; i < listboxMain.ItemContainerGenerator.Items.Count(); i++)
            {

                var item = listboxMain.ItemContainerGenerator.ContainerFromIndex(i);
                CheckBox checkbox = null;
                if (item != null)
                {
                    try
                    {
                        checkbox = Helper.FindVisualChild<CheckBox>(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                if (checkbox != null)
                {
                    checkedList.Add(checkbox);
                }
            }
            var str = String.Join(", ", checkedList.Select(x => x.ToString()).ToArray());
            transaction.Start("Completely Delete Shared parameter");
            foreach (CheckBox checkbox in checkedList)
            {

                if (checkbox.IsChecked == true)
                {
                    SharedParamWrapper spar = (SharedParamWrapper)checkbox.DataContext;
                    doc.Delete(spar.Param.Id);
                    
                }
            }
            MessageBox.Show("Deleted");
            transaction.Commit();
            ui.Close();
        }

    }
}