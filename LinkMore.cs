using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;
using View = Autodesk.Revit.DB.View;

namespace RG_Tools
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class LinkMore : IExternalCommand
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
            Application app = uiapp.Application;
            // new transaction
            Transaction t = new Transaction(doc);

            var Files = ChooseFiles();



            if (Files != null)
            {
                switch (Files[0].Substring(Files[0].Length - 4))
                {
                    case ".rvt":
                        { LinkMethodRVT(doc, Files, t); };
                        break;
                    case ".ifc":
                        { LinkMethodIFC(doc, Files, t, app); };
                        break;
                    case ".dwg":
                        { LinkMethodCAD(doc, Files, t, uidoc, "dwg"); };
                        break;
                    case ".dgn":
                        { LinkMethodCAD(doc, Files, t, uidoc, "dgn"); };
                        break;
                }
                return Result.Succeeded;
            }
            else { return Result.Cancelled; }



        }

        private static void LinkMethodCAD(Document doc, string[] Files, Transaction t, UIDocument uidoc, string format)
        {
            XYZ origin = doc.ActiveProjectLocation.GetTotalTransform().Origin;
            View activeView = uidoc.ActiveView;
            var thisViewOnly = true;
            View targetView = null;


            DialogResult dialogResult = MessageBox.Show("Link CAD to current view only?", "Link Many CAD", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                thisViewOnly = true;
                targetView = activeView;

            }
            else if (dialogResult == DialogResult.No)
            {
                thisViewOnly = false;
                targetView = null;
            }

            foreach (string file in Files)
            {
                if (File.Exists(file))
                {
                    switch (format)
                    {
                        case "dwg":
                            {
                                DWGImportOptions o = new DWGImportOptions();
                                o.AutoCorrectAlmostVHLines = true;
                                o.Unit = ImportUnit.Meter;
                                o.OrientToView = true;
                                o.ReferencePoint = origin;
                                o.ThisViewOnly = thisViewOnly;
                                t.Start("Link more DWG");
                                ElementId eId = null;
                                try
                                {
                                    doc.Link(file, o, targetView, out eId);
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message, "Error");
                                    t.RollBack();
                                }
                                t.Commit();
                            }
                            break;
                        case "dgn":
                            {
                                ElementId eId = null;
                                DGNImportOptions o = new DGNImportOptions();
                                o.AutoCorrectAlmostVHLines = true;
                                o.Unit = ImportUnit.Meter;
                                o.OrientToView = true;
                                o.ReferencePoint = origin;
                                o.ThisViewOnly = thisViewOnly;
                                o.DGNModelViewName = file;
                                t.Start("Link more DGN");
                                try
                                {
                                    doc.Link(file, o, targetView, out eId);
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message, "Error");
                                    t.RollBack();
                                }
                                t.Commit();
                            }
                            break;
                    }
                }
            }
        }

        private static void LinkMethodRVT(Document doc, string[] Files, Transaction t)
        {

            // Set options for linking (absolute or relative)
            RevitLinkOptions opt = new RevitLinkOptions(false);
            // Iteratively link all files
            foreach (var file in Files)
            {
                t.Start("Link Many RVT");
                try
                {
                    var mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(file);
                    LinkLoadResult linktype = RevitLinkType.Create(doc, mp, opt);
                    RevitLinkInstance instance = RevitLinkInstance.Create(doc, linktype.ElementId);
                    instance.Pinned = true;
                    t.Commit();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error");
                    t.RollBack();
                }
            }
        }

        private static void LinkMethodIFC(Document doc, string[] Files, Transaction t, Application app)
        {
            RevitLinkOptions opt = new RevitLinkOptions(false);
            foreach (var file in Files)
            {
                t.Start("Link Many IFC");
                try
                {
                    // Check if converted RVT exists
                    string f_cache = file.Remove(file.Length - 4) + ".rvt";
                    bool recreate = !File.Exists(f_cache);
                    // If not, convert IFC to RVT
                    if (recreate)
                    {
                        Document ifc = app.OpenIFCDocument(file);
                        SaveAsOptions sao = new SaveAsOptions();
                        sao.OverwriteExistingFile = true;
                        ifc.SaveAs(f_cache, sao);
                        ifc.Close();
                    }
                    // Link file
                    LinkLoadResult link = RevitLinkType.CreateFromIFC(doc, file, f_cache, false, opt);
                    RevitLinkInstance instance = RevitLinkInstance.Create(doc, link.ElementId);
                    instance.Pinned = true;
                    t.Commit();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error");
                    t.RollBack();
                }

            }


        }





        private static string[] ChooseFiles()
        {

            string FileTypes = "RVT files (*.rvt)|*.rvt|IFC files (*.ifc)|*.ifc|DWG files (*.dwg)|*.dwg|DXF files (*.dxf)|*.dxf|DGN files (*.dgn)|*.dgn";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\1";
                openFileDialog.Filter = FileTypes;
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //foreach (var file in openFileDialog.FileNames)
                    //{
                    //    MessageBox.Show(file, "123");
                    //}
                    return openFileDialog.FileNames;
                }
                else { return null; }
            }
        }
    }
}