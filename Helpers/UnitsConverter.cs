using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG_Tools.Helpers
{
    internal class UnitsConverter
    {
        public static double ConvertDouble(UIApplication uiapp, Parameter param)
        {
            string versionNumber = uiapp.Application.VersionNumber;

            #if R2020 || R2019
            double value = UnitUtils.ConvertToInternalUnits(param.AsDouble(), param.DisplayUnitType);
            #elif R2021 || R2022 || R2023 || R2024
            double value = UnitUtils.ConvertFromInternalUnits(param.AsDouble(), param.GetUnitTypeId());

            #endif
            return value;
        }


    }
}
