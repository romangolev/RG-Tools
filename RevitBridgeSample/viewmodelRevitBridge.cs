using RG_Tools.CopyValues;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RG_Tools
{
    internal class viewmodelRevitBridge
    {
        private Dictionary<string, int> _dicWallType;
        private ObservableCollection<string> _listParameters;

        // Declare the Revit model class here.
        // Consequently, create a get-set variable representing this.
        private modelRevitBridge _revitModel;
        private int _selectedwalltype;

        private ObservableCollection<ParameterWrapper> _params;

        public ObservableCollection<ParameterWrapper> Params
        {
            get { return _params; }
            set { _params = value; }
        }


        public Dictionary<string, int> DicWallType
        {
            get
            {
                return _dicWallType;
            }

            set
            {
                _dicWallType = value;
            }
        }

        public int selectedwalltype
        {
            get
            {
                return _selectedwalltype;
            }

            set
            {
                _selectedwalltype = value;
            }
        }

        public ObservableCollection<string> ListParameters
        {
            get
            {
                return _listParameters;
            }

            set
            {
                _listParameters = value;
            }
        }

        //  Commands
        // This will be used by the button in the WPF window.
        public ICommand RetrieveParametersValuesCommand
        {
            get
            {
                return null;
                //return new DelegateCommand(RetrieveParametersValuesAction);
            }
        }

        // The get-set variable
        internal modelRevitBridge RevitModel
        {
            get
            {
                return _revitModel;
            }

            set
            {
                _revitModel = value;
            }
        }

        public int SelectedWallType { get; set; }

        //// The action function for RetrieveParametersValuesCommand
        private void RetrieveParametersValuesAction()
        {
            if (SelectedWallType != -1)
            {
                ListParameters = new ObservableCollection<string>(RevitModel.GenerateParametersAndValues(SelectedWallType));
            }
        }

        // Constructor
        public viewmodelRevitBridge()
        {

        }
    }
}
