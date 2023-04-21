using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RG_Tools
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class ReviewLinks : IExternalCommand
    {


        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            // Get Document
            Document doc = uidoc.Document;
            //Fix unpinned
            List<object> unp = GetUnpinned(CollectLinks(doc));
            if (unp.Count > 0)
            {
                Transaction transaction1 = new Transaction(doc);
                transaction1.Start("Fix Pinned Links");
                TaskDialog fixunpinned = new TaskDialog("Instance(s) Not Pinned")
                {
                    MainInstruction = "Instance(s) Not Pinned",
                    MainContent = ConcatList(unp)
                };
                fixunpinned.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Pin link(s)");
                fixunpinned.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Ignore warning");
                fixunpinned.CommonButtons = TaskDialogCommonButtons.Close;
                fixunpinned.DefaultButton = TaskDialogResult.Close;
                TaskDialogResult t1Result = fixunpinned.Show();
                if (TaskDialogResult.CommandLink1 == t1Result)
                {
                    try
                    {
                        foreach (Element link in unp)
                        {
                            link.Pinned = true;
                        }
                        transaction1.Commit();
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
                    {
                        message = e.Message;
                        transaction1.RollBack();
                    }
                }
                else if (TaskDialogResult.CommandLink2 == t1Result)
                {
                    transaction1.RollBack();
                }
                else
                {
                    transaction1.RollBack();
                }
            }
            else
            {
                TaskDialog.Show("Check Unpinned", "There are no unpinned instances in project");
            }

            // Add Worksets
            if (doc.IsWorkshared == true)
            {
                List<object> unw = GetLinksWithoutWorkset(doc, CollectLinks(doc));
                if (unw.Any() == true)
                {
                    Transaction transaction2 = new Transaction(doc);
                    transaction2.Start("Fix Worksets");
                    TaskDialog fixworksets = new TaskDialog("Instance(s) Not Pinned")
                    {
                        MainInstruction = "Links are not in a separated worksets",
                        MainContent = ConcatList(unw)
                    };
                    fixworksets.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Create separate worksets for links");
                    fixworksets.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Ignore warning");
                    fixworksets.CommonButtons = TaskDialogCommonButtons.Close;
                    fixworksets.DefaultButton = TaskDialogResult.Close;
                    TaskDialogResult t2Result = fixworksets.Show();
                    if (TaskDialogResult.CommandLink1 == t2Result)
                    {
                        foreach (Element link in unw.Cast<Element>())
                        {
                            //Get LinkType
                            RevitLinkType linktype = (RevitLinkType)doc.GetElement(link.GetTypeId());
                            //Defy the new Workset Name
                            string workset_name = ConvertWSName(linktype);
                            //Create new workset
                            Workset wrks = Workset.Create(doc, workset_name);
                            //Put link type and link instance into new workset
                            Parameter wsparam = link.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                            Parameter wstypeparam = linktype.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                            try
                            {
                                wsparam.Set(wrks.Id.IntegerValue);
                                wstypeparam.Set(wrks.Id.IntegerValue);

                            }
                            catch { throw; }
                            /*
                            catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
                            {
                                message = e.Message;
                                transaction2.RollBack();
                            }*/
                        }
                        transaction2.Commit();
                    }
                    else if (TaskDialogResult.CommandLink2 == t2Result)
                    {
                        transaction2.RollBack();
                    }
                    else
                    {
                        transaction2.RollBack();
                    }

                    //TaskDialog.Show("Worksets Created", ConcatList(unw));

                }
                else if (unw.Any() == false)
                {
                    TaskDialog.Show("Link Worksets", "All links are in individual worksets");
                }

            }
            else if (doc.IsWorkshared == false)
            {
                TaskDialog.Show("Not Workshared", "The documenet is not workshared!");
            }

            return Result.Succeeded;
        }

        public static List<object> CollectLinks(Document doc)
        {
            ICollection<Element> cl_links = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().ToElements();
            List<object> collected = new List<object>();

            foreach (Element link in cl_links)
            {
                Element e = doc.GetElement(link.Id);
                //RevitLinkType rvtType = e.GetType();
                RevitLinkType revitLinkType = (RevitLinkType)doc.GetElement(e.GetTypeId());
                if (cl_links.Contains(revitLinkType))
                {
                    collected.Add(link);
                }
            }


            foreach (RevitLinkInstance link in cl_links)
            {
                RevitLinkType revitLinkType = (RevitLinkType)doc.GetElement(link.GetTypeId());
                if (revitLinkType.IsNestedLink == false)
                {
                    collected.Add(link);
                }
            }

            return collected;
        }

        public static List<object> GetUnpinned(ICollection<object> links)
        {
            List<object> unpinned = new List<object>();
            foreach (Element link in links)
            {
                if (link.Pinned == false)
                {
                    unpinned.Add(link);
                }
            }
            return unpinned;
        }

        public static List<object> GetLinksWithoutWorkset(Document doc, ICollection<object> links)
        {
            List<object> unwrk = new List<object>();

            //Get Workset and convers List to list of names
            FilteredWorksetCollector wsfilter = new FilteredWorksetCollector(doc);
            IList<Workset> wslist = wsfilter.ToWorksets();
            List<object> wsnames = new List<object>();

            foreach (Workset workset in wslist)
            {
                string wsname = workset.Name.ToString();
                wsnames.Add(wsname);
            }

            foreach (Element link in links)
            {
                bool alreadyExist = wsnames.Contains(ConvertWSName((RevitLinkType)doc.GetElement(link.GetTypeId())));
                if (alreadyExist == false)
                {
                    unwrk.Add(link);
                }
            }
            return unwrk;
        }

        public static string ConcatList(List<object> list1)
        {
            List<string> list2 = new List<string>();
            foreach (Element obj in list1)
            {
                list2.Add(obj.Name.ToString());
            }
            string outputstring = string.Join(Environment.NewLine, list2.ToArray());
            return outputstring;
        }

        public static string ConvertWSName(RevitLinkType linktype)
        {
            string workset_name = "##RVT_" + linktype.Name.ToString();
            string newname = workset_name.Substring(0, workset_name.Length - 4);
            return newname;
        }
    }
}