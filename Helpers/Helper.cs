using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Linq;

namespace RG_Tools
{
    public class Helper
    {

        private static readonly double FtToMm = 304.8;

        public static ImageSource GetIconSource(Bitmap bmp) => System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(),
            IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        private static FillPatternElement GetSolidFillPatternElement(Document doc)
        {
            using (IEnumerator<Element> enumerator = ((IEnumerable<Element>)Helper.GetElem(doc, typeof(FillPatternElement))).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    FillPatternElement current = (FillPatternElement)enumerator.Current;
                    if (current.GetFillPattern().IsSolidFill)
                        return current;
                }
            }
            return (FillPatternElement)null;
        }

        public static void SetColorToElemOnView(Element elem, View view, Autodesk.Revit.DB.Color fillColor)
        {
            OverrideGraphicSettings overrideGraphicSettings = new OverrideGraphicSettings();
            overrideGraphicSettings.SetSurfaceForegroundPatternColor(fillColor);
            overrideGraphicSettings.SetSurfaceForegroundPatternId(((Element)Helper.GetSolidFillPatternElement(elem.Document)).Id);
            view.SetElementOverrides(elem.Id, overrideGraphicSettings);
        }

        // Get Element
        public static ICollection<Element> GetElem(Document doc, Type T) => (ICollection<Element>)new FilteredElementCollector(doc).OfClass(T).ToElements();
        public static ICollection<Element> GetElem(Document doc, BuiltInCategory B) => (ICollection<Element>)new FilteredElementCollector(doc).OfCategory(B).OfClass(typeof(FamilyInstance)).ToElements();

        public static void CreateButton(
          string name,
          string text,
          string thisAssemblyPath,
          string command,
          ref RibbonPanel panelGroup,
          Bitmap picRes,
          string toolTip)
        {
            PushButtonData pushButtonData = new PushButtonData(name, text, thisAssemblyPath, command);
            PushButton pushButton = (PushButton)panelGroup.AddItem(pushButtonData);
            pushButton.ToolTip = toolTip;
            ImageSource iconSource = Helper.GetIconSource(picRes);
            pushButton.LargeImage = iconSource;

        }


        public static Solid CreateSolidByPoint(XYZ point)
        {
            double num = 1.0 / Helper.FtToMm * 0.5;
            XYZ xyz1 = new XYZ(point.X - num, point.Y - num, point.Z - num);
            XYZ xyz2 = new XYZ(point.X + num, point.Y - num, point.Z - num);
            XYZ xyz3 = new XYZ(point.X + num, point.Y + num, point.Z - num);
            XYZ xyz4 = new XYZ(point.X - num, point.Y + num, point.Z - num);
            List<Curve> curveList = new List<Curve>();
            curveList.Add((Curve)Line.CreateBound(xyz1, xyz2));
            curveList.Add((Curve)Line.CreateBound(xyz2, xyz3));
            curveList.Add((Curve)Line.CreateBound(xyz3, xyz4));
            curveList.Add((Curve)Line.CreateBound(xyz4, xyz1));
            List<CurveLoop> curveLoopList1 = new List<CurveLoop>();
            curveLoopList1.Add(CurveLoop.Create((IList<Curve>)curveList));
            List<CurveLoop> curveLoopList2 = curveLoopList1;
            SolidOptions solidOptions = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);
            return GeometryCreationUtilities.CreateExtrusionGeometry((IList<CurveLoop>)curveLoopList2, XYZ.BasisZ, num * 2.0, solidOptions);
        }


        private static bool CheckPointExist(List<XYZ> list, XYZ newPoint)
        {
            if (list == null || list.Count < 1)
                return true;
            using (List<XYZ>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsAlmostEqualTo(newPoint))
                        return false;
                }
            }
            return true;
        }



        public static CurveArray PathToCurveArray(IList<Curve> curveList)
        {
            CurveArray curveArray = new CurveArray();
            using (IEnumerator<Curve> enumerator = ((IEnumerable<Curve>)curveList).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    Curve current = enumerator.Current;
                    curveArray.Append(current);
                }
            }
            return curveArray;
        }

        public static ElementId get3DviewType(Document doc)
        {
            ICollection<Element> collector = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).ToElements();
            foreach (ViewFamilyType element in collector)
            {
                if (element.ViewFamily == ViewFamily.ThreeDimensional)
                {
                    ElementId viewFamTypeId = element.Id;
                    return viewFamTypeId;
                }
            }
            return null;

        }

        public static ICollection<ElementId> GetLinkedRevitTypes(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<ElementId> elementIdSet =
              collector
              .OfCategory(BuiltInCategory.OST_RvtLinks)
              .OfClass(typeof(RevitLinkType))
              .ToElementIds();
            return elementIdSet;
        }

        public static View3D HideRevitInstances(View3D view3D, Document doc)
        {
            ICollection<ElementId> elementIdSet = GetLinkedRevitTypes(doc);
            foreach (ElementId linkedFileId in elementIdSet)
            {
                if (linkedFileId != null)
                {
                    if (true == doc.GetElement(linkedFileId).IsHidden(view3D))
                    {
                        if (true == doc.GetElement(linkedFileId).CanBeHidden(view3D))
                        {
                            view3D.HideElements(elementIdSet);
                        }
                    }
                    else
                    {
                        view3D.HideElements(elementIdSet);
                    }
                }
            }
            return view3D;
        }

        public static ICollection<ElementId> GetLinkedRevitInstances(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<ElementId> elementIdSet =
              collector
              .OfCategory(BuiltInCategory.OST_RvtLinks)
              .OfClass(typeof(RevitLinkInstance))
              .ToElementIds();
            return elementIdSet;
        }


        // Collect linked RVT files in current document
        public static IList<ElementId> Collect_Links(Document doc)
        {
            FilteredElementCollector cl = new FilteredElementCollector(doc);
            IList<ElementId> links = (IList<ElementId>)cl.OfCategory(BuiltInCategory.OST_RvtLinks).ToElementIds();
            return links;
        }
        public static IList<Element> CollectCurrent(Document doc)
        {
            FilteredElementCollector allElementsInView =
                new FilteredElementCollector(doc, doc.ActiveView.Id);
            return allElementsInView.WhereElementIsNotElementType().ToElements();
        }
        public static IList<Element> collectCurrentTypes(Document doc)
        {
            FilteredElementCollector allElementsInView =
                new FilteredElementCollector(doc, doc.ActiveView.Id);
            return allElementsInView.ToElements();
        }

        // Check if parameter exists in project by inputed GUID 
        public static ParameterElement CheckParamByGUID(Guid param, Document doc)
        {
            FilteredElementCollector spef = new FilteredElementCollector(doc).OfClass(typeof(SharedParameterElement));
            IEnumerable<Element> spe = spef.ToElements();
            foreach (SharedParameterElement elem in spe)
            {
                if (elem.GuidValue == param)
                {
                    return elem;
                }
            }
            return null;
        }
        public static void MakeToast(string appname, string title, string message)
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filename = Path.Combine(assemblyFolder, "toast64.exe");
            string iconlocation = Path.Combine(assemblyFolder, "rhi.ico");

            Process proc2 = new Process();
            proc2.StartInfo.FileName = filename;

            string param1 = "--app-id \"" + appname + "\"";
            string param2 = " --title \"" + title + "\"";
            string param3 = " --message \"" + message + "\"";
            string param4 = " --icon \"" + iconlocation + "\"";
            proc2.StartInfo.CreateNoWindow = true;
            proc2.StartInfo.RedirectStandardInput = true;
            proc2.StartInfo.RedirectStandardOutput = false;
            proc2.StartInfo.Arguments = param1 + param2 + param3 + param4;
            proc2.StartInfo.UseShellExecute = false;
            proc2.Start();
        }

        // Try to create new View Family Type called Coordination or use an existed one
        public static ViewFamilyType get_ViewType(Transaction transaction, FilteredElementCollector a, Document doc, string viewName)
        {
            foreach (ViewFamilyType vft in a.ToElements())
            {
                if (vft.Name.Equals(viewName))
                {
                    foreach (Parameter param in vft.GetOrderedParameters())
                    {
                        if (param.Id.ToString() == "-1008210" && param.AsElementId() != ElementId.InvalidElementId)
                        {
                            //Set template to none
                            transaction.Start("Edit Settings for View Family Type");
                            param.Set(ElementId.InvalidElementId);
                            transaction.Commit();
                        }
                    }
                    return vft;
                }
                else
                {
                    try
                    {
                        transaction.Start("Create ViewFamily Type");
                        ViewFamilyType old_v = (ViewFamilyType)doc.GetElement(Helper.get3DviewType(doc));
                        ViewFamilyType new_v = old_v.Duplicate(viewName) as ViewFamilyType;
                        //Set template to none
                        foreach (Parameter param in new_v.GetOrderedParameters())
                        {
                            if (param.Id.ToString() == "-1008210" && param.AsElementId() != ElementId.InvalidElementId)
                            {
                                param.Set(ElementId.InvalidElementId);
                            }
                        }
                        transaction.Commit();
                        return new_v;
                    }
                    catch
                    {
                        transaction.RollBack();
                    }
                }
            }
            return null;
        }

        public static bool FindSharedParameter(Document doc, string paramname, Guid guid)
        {
            var collector = new FilteredElementCollector(doc).WhereElementIsNotElementType()
                        .OfClass(typeof(SharedParameterElement)).ToElements();
            foreach (SharedParameterElement element in collector)
            {
                InternalDefinition intDef = element.GetDefinition();
                if (intDef.Name == paramname && element.GuidValue == guid)
                {
                    return true;
                }
            }
            return false;
        }



    }
}
