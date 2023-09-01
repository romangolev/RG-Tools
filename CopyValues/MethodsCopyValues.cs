using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RG_Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RG_Tools
{
    internal class MethodsCopyValues
    {
        public static void ExecuteCopyValues(UIApplication uiApp, ICollection<ElementId> Selected, ParameterWrapper FromParam, ParameterWrapper ToParam)
        {
            UIApplication uiapp = uiApp;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document DOC = uidoc.Document;
            ICollection <ElementId> selected = Selected;
            IList<string> tasks = new List<string>();
            Transaction transaction = new Transaction(DOC);

            try
            {
                transaction.Start("Copy Parameters");
                if (FromParam.IsInstance)
                {
                    if (ToParam.IsInstance)
                    {
                        //Implementing logic to copy Values from instance to instance
                        foreach (ElementId elemId in selected)
                        {
                            CopyValuesProcessor InstanceToInstance = new CopyValuesProcessor()
                            {
                                Uiapp = uiApp,
                                SelectedFrom = DOC.GetElement(elemId),
                                SelectedTo = DOC.GetElement(elemId),
                                FromParam = FromParam,
                                ToParam = ToParam
                            };
                            InstanceToInstance.RunLogic();
                            tasks.Add(InstanceToInstance.Status);
                        }
                    }
                    else if (!ToParam.IsInstance)
                    {
                        //Implementing logic to copy Values from instance to type
                        MessageBox.Show("Cannot write Instance parameter Values to Type parameter", "Cancelled");
                    }
                }
                else if (!FromParam.IsInstance)
                {
                    if (ToParam.IsInstance)
                    {
                        //Implementing logic to copy Values from type to instance
                        foreach (ElementId elemId in selected)
                        {
                            CopyValuesProcessor InstanceToInstance = new CopyValuesProcessor()
                            {
                                Uiapp = uiApp,
                                SelectedFrom = DOC.GetElement(DOC.GetElement(elemId).GetTypeId()),
                                SelectedTo = DOC.GetElement(elemId),
                                FromParam = FromParam,
                                ToParam = ToParam
                            };
                            InstanceToInstance.RunLogic();
                            tasks.Add(InstanceToInstance.Status);
                        }
                    }
                    else if (!ToParam.IsInstance)
                    {
                        //Implementing logic to copy Values from type to type
                        foreach (ElementId elemId in selected)
                        {
                            CopyValuesProcessor InstanceToInstance = new CopyValuesProcessor()
                            {
                                Uiapp = uiApp,
                                SelectedFrom = DOC.GetElement(DOC.GetElement(elemId).GetTypeId()),
                                SelectedTo = DOC.GetElement(DOC.GetElement(elemId).GetTypeId()),
                                FromParam = FromParam,
                                ToParam = ToParam
                            };
                            InstanceToInstance.RunLogic();
                            tasks.Add(InstanceToInstance.Status);
                            
                        }
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.RollBack();
                MessageBox.Show(ex.ToString());
            }
            SendReport(tasks);
        }

        private static void SendReport(IList<string> tasks)
        {
            string report;
            string success = "Succesufuly copied parameter values : " + tasks.Count(item => (item == "Success")).ToString() + " elements";
            string skipped = "\nSkipped copying for : " + tasks.Count(item => (item == "Skipped")).ToString() + " elements";
            string error = "\nError encounter during copying : " + tasks.Count(item => (item == "Error")).ToString() + " elements";
            string notsupported = "\nCopying not supported for : " + tasks.Count(item => (item == "Not supported")).ToString() + " elements";
            report = success + skipped + error + notsupported ;
            MessageBox.Show(report);
        }


        // CopyValue processor class
        public class CopyValuesProcessor
        {
            public UIApplication Uiapp { get; set; }
            public Element SelectedFrom { get; set; }
            public Element SelectedTo { get; set; }
            public ParameterWrapper FromParam { get; set; }
            public ParameterWrapper ToParam { get; set; }

            public string Status;

            public void RunLogic()
            {
                //Get Value 
                Parameter val = GetValue();
                if (val != null)
                {
                    switch (val.StorageType)
                    {
                        case StorageType.Integer:
                            int valInteger = val.AsInteger();
                            CopyInteger(valInteger);
                            break;
                        case StorageType.Double:
                            double valDouble = val.AsDouble();
                            CopyDouble(val);
                            break;
                        case StorageType.String:
                            string valString = val.AsString();
                            CopyString(valString);
                            break;
                        case StorageType.ElementId:
                            ElementId valElementId = val.AsElementId();
                            CopyElementId(valElementId);
                            break;
                        default:
                            Status = "Error";
                            break;
                    }
                }
                else
                {
                    Status = "Skipped";
                }

            }

            private void CopyElementId(ElementId valElementId)
            {
                switch (ToParam.Param.StorageType)
                {
                    case StorageType.Integer:
                        Status = "Not supported";
                        break;
                    case StorageType.Double:
                        Status = "Not supported";
                        break;
                    case StorageType.String:
                        Status = "Not supported";
                        break;
                    case StorageType.ElementId:
                        try
                        {
                            SetParam().Set(valElementId);
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Skipped";
                        }
                        break;
                    default:
                        Status = "Error";
                        break;
                }
            }

            private void CopyString(string valString)
            {
                switch (ToParam.Param.StorageType)
                {
                    case StorageType.Integer:
                        try
                        {
                            SetParam().Set(System.Math.Round(Convert.ToDouble(valString)));
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Skipped";
                        }
                        break;
                    case StorageType.Double:
                        try
                        {
                            SetParam().Set(Convert.ToDouble(valString));
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Skipped";
                        }
                        break;
                    case StorageType.String:
                        try
                        {
                            SetParam().Set(valString);
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Error";
                        }
                        break;
                    case StorageType.ElementId:
                        Status = "Not supported";
                        break;
                    default:
                        Status = "Error";
                        break;
                }
            }

            private void CopyDouble(Parameter val)
            {
                switch (ToParam.Param.StorageType)
                {
                    case StorageType.Integer:
                        try
                        {
                            double val_converted = UnitsConverter.ConvertDouble(Uiapp, val);
                            SetParam().Set(Convert.ToInt32(val_converted));
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Error";
                        }
                        break;
                    case StorageType.Double:
                        try
                        {
                            SetParam().Set(val.AsDouble());
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Error";
                        }
                        break;
                    case StorageType.String:
                        try
                        {
                            double val_converted = UnitsConverter.ConvertDouble(Uiapp, val);
                            SetParam().Set(val_converted.ToString());  
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Error";
                        }
                        break;
                    case StorageType.ElementId:
                        Status = "Not supported";
                        break;
                    default:
                        Status = "Error";
                        break;
                }
            }

            private void CopyInteger(int valInteger)
            {
                switch (ToParam.Param.StorageType)
                {
                    case StorageType.Integer:
                        try
                        {
                            SetParam().Set(valInteger);
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Skipped";
                        }
                        break;
                    case StorageType.Double:
                        try
                        {
                            SetParam().Set(valInteger);
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Skipped";
                        }
                        break;
                    case StorageType.String:
                        try
                        {
                            SetParam().Set(valInteger.ToString());
                            Status = "Success";
                        }
                        catch
                        {
                            Status = "Skipped";
                        }
                        break;
                    case StorageType.ElementId:
                        Status = "Not supported";
                        break;
                    default:
                        Status = "Error";
                        break;
                }
            }

            private Parameter SetParam()
            {
                if (ToParam.Param.IsShared)
                {

                    Parameter val = SelectedTo.get_Parameter(ToParam.Param.GUID);
                    return val;
                }
                else if (!ToParam.Param.IsShared)
                {
                    Parameter val = SelectedTo.GetParameters(ToParam.Param.Definition.Name)[0];
                    return val;
                }
                else
                {
                    return null;
                }
            }

            private Parameter GetValue()
            {
                if (FromParam.Param.IsShared)
                {
                    Parameter val = SelectedFrom.get_Parameter(FromParam.Param.GUID);
                    return val;
                }
                else if (!FromParam.Param.IsShared)
                {
                    Parameter val = SelectedFrom.GetParameters(FromParam.Param.Definition.Name)[0];
                    return val;
                }
                else
                {
                    return null;
                }
            }

        }
    }
}
