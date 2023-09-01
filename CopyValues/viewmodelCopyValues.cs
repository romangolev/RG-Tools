using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RG_Tools.Helpers;
using Autodesk.Revit.UI;

namespace RG_Tools
{
    internal class viewmodelCopyValues : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region instanceParameters Constructor

        private IList<Parameter> _instanceParameters;
        public IList<Parameter> instanceParameters
        {
            get { return _instanceParameters; }
            set { _instanceParameters = value; }
        }
        #endregion

        #region Params Constructor
        private ObservableCollection<ParameterWrapper> _params;
        public ObservableCollection<ParameterWrapper> Params
        {
            get { return _params; }
            set { _params = value; }
        }
        #endregion

        #region FromParam and ToParam Constructor

        private ParameterWrapper _fromparam;
        private ParameterWrapper _toparam;
        public ParameterWrapper FromParam
        {
            get { return _fromparam; }
            set { _fromparam = value; }
        }
        public ParameterWrapper ToParam
        {
            get { return _toparam; }
            set { _toparam = value; }
        }
        #endregion

        #region Selected Constructor
        private ICollection<ElementId> _selected;
        public ICollection<ElementId> Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        #endregion


        public viewmodelCopyValues()
        {

        }

    }
}

