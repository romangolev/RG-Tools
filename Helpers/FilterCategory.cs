using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

public class Filter_category : ISelectionFilter
{
    public bool AllowElement(Element e)
    {
        if (e.Category is null) return false;
        if (e.Category.IsCuttable == true) return true;
        return false;
    }


    public bool AllowReference(Reference refer, XYZ point)
    {
        return false;
    }
}