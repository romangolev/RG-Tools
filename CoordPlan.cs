using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
//using Autodesk.Windows;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace RG_Tools
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CoordPlan : IExternalCommand
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
            // Get transaction
            Transaction transaction = new Transaction(doc);


            // View coordPlan = (View)doc.GetElement(el).Duplicate(ViewDuplicateOption.Duplicate);
            Level lvl = new FilteredElementCollector(doc)
                                                .OfCategory(BuiltInCategory.OST_Levels)
                                                .WhereElementIsNotElementType()
                                                .ToElements().Cast<Level>().FirstOrDefault<Level>();
            // Get Family View Types 
            FilteredElementCollector a = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType));
            // Get View type
            ViewFamilyType vf = get_ViewType(transaction, a);
            // Find existed views
            List<object> cp = FindCoordinationView(doc);
            // Creating a view plan based on View Family Type and level in current document
            if (cp.Count == 0)
            {
                transaction.Start("Create Coordination Plan");
                ViewPlan coordPlan = createCoordPlan(doc, vf, lvl);
                transaction.Commit();
                // Make new plan active in viewer
                uidoc.ActiveView = (View)doc.GetElement(coordPlan.Id);
                return Result.Succeeded;

            }
            // Work on exceptions if plan existed 
            else if (cp.Any())
            {
                string taskName = "Existed Coordination Plan detected";
                string taskDescription = "Delete existed Coordination Plan and create new";
                TaskDialog mainDialog = new TaskDialog(taskName)
                {
                    MainInstruction = "Coordination Plan Detected!",
                    MainContent =
                    "What would you like to do with existing Coordination Plan?"
                };
                // Add commmandLink options to task dialog
                mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1,
                                          "Keep existed and open it");

                mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2,
                                          taskDescription);
                TaskDialogResult tResult = mainDialog.Show();
                if (TaskDialogResult.CommandLink1 == tResult)
                {
                    uidoc.ActiveView = (View)cp[0];
                    return Result.Succeeded;
                }
                else if (TaskDialogResult.CommandLink2 == tResult)
                {
                    using TransactionGroup transGroup = new TransactionGroup(doc);
                    transGroup.Start("Delete all existed and create new Coordination plan");
                    ViewPlan _dummy = null;
                    using (Transaction trans0 = new Transaction(doc))
                    {
                        trans0.Start("Creating Dummy");
                        ViewPlan dummy = ViewPlan.Create(doc, vf.Id, lvl.Id);
                        trans0.Commit();
                        uidoc.ActiveView = dummy;
                        _dummy = dummy;
                    }

                    using (Transaction trans1 = new Transaction(doc))
                    {
                        trans1.Start("Delete existed Coordination Plan and Create new");
                        foreach (View c in cp.Cast<View>())
                        {
                            doc.Delete(c.Id);
                        }
                        ViewPlan vp = createCoordPlan(doc, vf, lvl);
                        trans1.Commit();
                        uidoc.ActiveView = vp;
                    }
                    using (Transaction trans2 = new Transaction(doc))
                    {
                        trans2.Start("Delete dummy");
                        doc.Delete(_dummy.Id);
                        trans2.Commit();
                    }
                    transGroup.Assimilate();
                }
                return Result.Succeeded;
            }
            return Result.Failed;
        }

        // Function that finds existed Navis View in a Document
        public static List<object> FindCoordinationView(Document doc)
        {

            List<object> coord_plan = new List<object>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> elems = collector.OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType().ToElements();
            foreach (View elem in elems.Cast<View>())
            {
                if (elem.ViewType == ViewType.FloorPlan && elem.IsTemplate == false)
                {
                    if (elem.Name is string name && name.Contains("Coordination Plan")) { coord_plan.Add(elem); }
                    else continue;
                }
            }

            return coord_plan;
        }


        // Find an existed ViewFamilyType
        // or Try to create new ViewFamilyType called Coordination
        public static ViewFamilyType get_ViewType(Transaction transaction, FilteredElementCollector a)
        {

            foreach (ViewFamilyType vft in a.ToElements())
            {
                if (vft.Name.Equals("Coordination Plan"))
                {
                    try
                    {
                        vft.DefaultTemplateId = null;
                    }
                    catch { };
                    return vft;
                }
            }


            try
            {
                transaction.Start("Create View Family Type");

                ViewFamilyType old_v = get_viewTypePlan(a.ToElements());

                ViewFamilyType new_v = old_v.Duplicate("Coordination Plan") as ViewFamilyType;
                try
                {
                    new_v.DefaultTemplateId = null;
                }
                catch { };
                transaction.Commit();
                return new_v;
            }
            catch
            {
                transaction.RollBack();
            }


            return null;
        }

        // Gets the first available TypePlan
        public static ViewFamilyType get_viewTypePlan(IEnumerable<Element> elems)
        {
            foreach (ViewFamilyType elem in elems)
            {
                if (elem.FamilyName == "Floor Plan")
                {
                    return elem;
                }
            }
            return null;
        }

        public static ViewPlan createCoordPlan(Document doc, ViewFamilyType vft, Level lvl)
        {
            //Create ViewPlan
            ViewPlan floorPlan = ViewPlan.Create(doc, vft.Id, lvl.Id);
            floorPlan.ViewTemplateId = ElementId.InvalidElementId;
            if (floorPlan.ViewTemplateId != null)
            {
                floorPlan.ViewTemplateId = ElementId.InvalidElementId;
            }
            //Adjust coordination plan settings
            floorPlan.Name = "Coordination Plan";
            //rendering options
            floorPlan.DetailLevel = ViewDetailLevel.Coarse;
            floorPlan.DisplayStyle = DisplayStyle.Wireframe;
            floorPlan.Discipline = ViewDiscipline.Coordination;
            //show coordination points
            floorPlan.SetCategoryHidden(Category.GetCategory(doc, BuiltInCategory.OST_ProjectBasePoint).Id, false);
            floorPlan.SetCategoryHidden(Category.GetCategory(doc, BuiltInCategory.OST_Site).Id, false);
            floorPlan.SetCategoryHidden(Category.GetCategory(doc, BuiltInCategory.OST_SharedBasePoint).Id, false);
            //removing cropboxes
            floorPlan.AreAnnotationCategoriesHidden = false;
            floorPlan.CropBoxActive = false;
            floorPlan.CropBoxVisible = false;
            floorPlan.get_Parameter(BuiltInParameter.VIEWER_ANNOTATION_CROP_ACTIVE).Set(0);
            //editing view range
            PlanViewRange vr = floorPlan.GetViewRange();
            vr.SetLevelId(PlanViewPlane.TopClipPlane, PlanViewRange.Unlimited);
            vr.SetLevelId(PlanViewPlane.BottomClipPlane, PlanViewRange.Unlimited);
            vr.SetLevelId(PlanViewPlane.ViewDepthPlane, PlanViewRange.Unlimited);
            floorPlan.SetViewRange(vr);
            return floorPlan;
        }
    }
}

// TODO: Add options for new plan (Base point visibility, etc...)