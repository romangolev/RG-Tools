﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using RG_Tools;
using System.Runtime.Remoting.Messaging;
using System.Windows.Input;
using System.Windows;
using Autodesk.Revit.DB.Architecture;

namespace RG_Tools
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class NavisView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            // Get Document
            Document doc = uidoc.Document;

            List<object> nw_ex = Find_ex(doc);
            Transaction transaction = new Transaction(doc);

            FilteredElementCollector a = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType));

            ViewFamilyType vf = Helper.get_ViewType(transaction, a, doc, "Navisworks");

            bool SHIFT = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            

            if (nw_ex.Count == 0)
            {
                //check if model editable
                if (CheckReadOnly(doc)) { return Result.Cancelled; }
                //proceed with creating new view
                transaction.Start("Create Navis 3D View");
                View3D view = CreateNavisView(doc, vf);
                View3D adjview = AdjustNavisView(doc, view, SHIFT);
                transaction.Commit();
                uidoc.ActiveView = adjview;
                return Result.Succeeded;
            }
            else if (nw_ex.Any())
            {
                string taskName = "Multiple existed Navis view detected" + nw_ex.Count();
                string taskDescription = "Delete all existed Navis views and create new";
                if (nw_ex.Count() == 1)
                {
                    taskName = "Existed Navis view detected";
                    taskDescription = "Delete existed Navis view and create new one";
                }

                TaskDialog mainDialog = new TaskDialog(taskName)
                {
                    MainInstruction = "Navis View Detected!",
                    MainContent =
                    "What would you like to do with existing 'Navis' view?"
                };

                // Add commmandLink options to task dialog
                mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1,
                                          "Keep existed and open it");

                mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2,
                                          taskDescription);
                if (nw_ex.Count() == 1)
                {
                    mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink3,
                                          "Restore existed Navis view settings");
                }

                mainDialog.FooterText = "Deleting all existed may affect cloud publishing settings";
                TaskDialogResult tResult = mainDialog.Show();
                if (TaskDialogResult.CommandLink1 == tResult)
                {
                    uidoc.ActiveView = (View)nw_ex[0];
                    return Result.Succeeded;
                }
                else if (TaskDialogResult.CommandLink2 == tResult)
                {
                    if (CheckReadOnly(doc)) { return Result.Cancelled; }
                    using TransactionGroup transGroup = new TransactionGroup(doc);
                    transGroup.Start("Delete all existed and create new Navis view");
                    using (Transaction trans1 = new Transaction(doc))
                    {
                        trans1.Start("Create dummy");
                        View3D dummy3d = CreateDefault3D(doc);
                        trans1.Commit();
                        uidoc.ActiveView = dummy3d;

                        using (Transaction trans2 = new Transaction(doc))
                        {
                            trans2.Start("Delete existed Navis view and create new");
                            foreach (View vie in nw_ex.Cast<View>())
                            {
                                doc.Delete(vie.Id);
                            }
                            View3D nwnew = CreateNavisView(doc, vf);
                            View3D nwadj = AdjustNavisView(doc, nwnew, SHIFT);
                            trans2.Commit();
                            uidoc.ActiveView = nwadj;
                        }
                        using Transaction trans3 = new Transaction(doc);
                        trans3.Start("Delete dummy");
                        doc.Delete(dummy3d.Id);
                        trans3.Commit();
                    }
                    transGroup.Assimilate();
                    return Result.Succeeded;
                }
                else if (TaskDialogResult.CommandLink3 == tResult)
                {
                    if (CheckReadOnly(doc)) { return Result.Cancelled; }
                    try
                    {
                        transaction.Start("Change Navis view settings");
                        View3D vv = (View3D)nw_ex[0];
                        Parameter par = vv.get_Parameter(BuiltInParameter.VIEW_TEMPLATE);
                        par?.Set(ElementId.InvalidElementId);

                        vv = AdjustNavisView(doc, vv, SHIFT);
                        transaction.Commit();
                        uidoc.ActiveView = vv;
                    }
                    catch { return Result.Failed; };
                    return Result.Succeeded;
                }
            }
            return Result.Succeeded;

        }

        // Function for finding existed Navis View in a Document
        public static List<object> Find_ex(Document doc)
        {

            List<object> navis3D = new List<object>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> elems = collector.OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType().ToElements();
            foreach (View elem in elems.Cast<View>())
            {
                if (elem.ViewType == ViewType.ThreeD && elem.IsTemplate == false)
                {
                    if (elem.Name is string name && name.Contains("Navis")) { navis3D.Add(elem); }
                    else if (elem.Name is string name1 && name1.Contains("navis")) { navis3D.Add(elem); }
                    else continue;
                }
            }

            return navis3D;
        }

        // Creates Navis View
        public static View3D CreateNavisView(Document doc, ViewFamilyType vf)
        {
            //View3D view3D = View3D.CreateIsometric(doc, Helper.get3DviewType(doc));
            View3D view3D = View3D.CreateIsometric(doc, vf.Id);
            //Rename view
            view3D.Name = "Navisworks";
            return view3D;
        }

        // Adjust Settings of Navis View
        public static View3D AdjustNavisView(Document doc, View3D view3D, bool SHIFT)
        {
            //Hide all annotations, import, point clouds on view
            view3D.AreAnnotationCategoriesHidden = true;
            view3D.AreImportCategoriesHidden = true;
            view3D.ArePointCloudsHidden = true;
            //Changes Display Style to "FlatColors"
            view3D.DisplayStyle = DisplayStyle.FlatColors;
            //Change Detail level
            view3D.DetailLevel = ViewDetailLevel.Medium;
            //Change override detail level for High
            OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
            overrideGraphicSettings.SetDetailLevel(ViewDetailLevel.Fine);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_PipeAccessory).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_PipeInsulations).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_PipeFitting).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_PlaceHolderPipes).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_MechanicalEquipment).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_DuctTerminal).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_PlumbingFixtures).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_DuctAccessory).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_FlexPipeCurves).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_FlexDuctCurves).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_LightingFixtures).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_LightingDevices).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_Conduit).Id, overrideGraphicSettings);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_ConduitFitting).Id, overrideGraphicSettings);
            //Change override — hide lines
            view3D.SetCategoryHidden(Category.GetCategory(doc, BuiltInCategory.OST_Lines).Id, true);
            //Make wall and Curtain walls half-transparent
            OverrideGraphicSettings transp = new OverrideGraphicSettings();
            transp.SetSurfaceTransparency(50);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_Walls).Id, transp);
            view3D.SetCategoryOverrides(Category.GetCategory(doc, BuiltInCategory.OST_CurtainWallPanels).Id, transp);

            // Show only original
            view3D.PartsVisibility = PartsVisibility.ShowOriginalOnly;

            //Set View disciplene
            view3D.Discipline = ViewDiscipline.Coordination;
            //Hide sub-categories including lines

            //Hide Revit links for 
            if (SHIFT == false)
            {
                try
                {
                    view3D.HideElements(Helper.Collect_Links(doc));
                }
                catch 
                {
                    ;
                }
            }



            return view3D;
        }

        public static View3D CreateDefault3D(Document doc)
        {
            View3D view3D = View3D.CreateIsometric(doc, Helper.get3DviewType(doc));
            return view3D;
        }

        public static bool CheckReadOnly(Document doc)
        {
            if (doc.IsReadOnlyFile)
            {
                MessageBox.Show("The operattion cannot be completed, because the model is in 'Read Only' mode", "Naviw View - Cancelled");
                return true;
            }
            else
            {
                return false;
                //continue code execution
            }
        }

    }
}
