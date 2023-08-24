using Autodesk.Revit.DB;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using RG_Tools.CopyValues;
using static RG_Tools.CopyValues.CopyValues;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.Windows.Controls;
using System.Windows;

namespace RG_Tools.CopyValues
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private IList<Parameter> _instanceParameters;

        public IList<Parameter> instanceParameters
        {
            get { return _instanceParameters;}
            set {_instanceParameters = value;}
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender,e) => { };

        public string Test { get ; set; }

        private ObservableCollection<ParameterWrapper> _params;

        public ObservableCollection<ParameterWrapper> Params
        {
            get { return _params; }
            set { _params = value; }
        }
        private ParameterWrapper _sparams;

        public ParameterWrapper SParam
        {
            get { return _sparams; }
            set { _sparams = value; }
        }

        public BaseViewModel()
        {

            //MessageBox.Show(_instanceParameters.Count.ToString());
            //Params = new ObservableCollection<ParameterWrapper>()
            //{

            //    new ParameterWrapper(){Id=1,  Name="Nirav", Param = null},
            //    new ParameterWrapper(){Id=2,  Name="Kapil", Param = null},
            //    new ParameterWrapper(){Id=3,  Name="Arvind", Param = null},
            //    new ParameterWrapper(){Id=4,  Name="Rajan", Param = null}
            //};


            Task.Run(async () =>
            {
                int i = 0;
                while (true)
                {
                    await Task.Delay(200);
                    Test = (i++).ToString();
                }
            });
        }


    }
}
