using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace RG_Tools
{
    /// <summary>
    /// Interaction logic for SEBC.xaml
    /// </summary>
    public partial class viewRevitBridge : System.Windows.Window
    {

        public viewRevitBridge()
        {

            InitializeComponent();

        }

    }
}
