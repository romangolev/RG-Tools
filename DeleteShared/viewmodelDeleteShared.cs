using Autodesk.Revit.DB;
using RG_Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG_Tools
{
    public class viewmodelDeleteShared
    {
        #region CollectedParams Constructor
        private IList<Element> _CollectedParams;
        public IList<Element> CollectedParams
        {
            get { return _CollectedParams; }
            set { _CollectedParams = value; }
        }
        #endregion

        #region SharedParams Constructor

        private ObservableCollection<SharedParamWrapper> _sharedparamswrapped;
        public ObservableCollection<SharedParamWrapper> SharedParamsWrapped
        {
            get { return _sharedparamswrapped; }
            set { _sharedparamswrapped = value; }
        }
        #endregion
        public viewmodelDeleteShared() 
        {


        }
    }
}
