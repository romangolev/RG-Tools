using System;
using System.Collections.Generic;
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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RG_Tools;

namespace RG_Tools
{
    /// <summary>
    /// Interaction logic for DeleteSharedWPF.xaml
    /// </summary>
    public partial class DeleteSharedWPF : Window
    {
        private readonly Document _doc;
        private readonly UIApplication _uiApp;
        private readonly Autodesk.Revit.ApplicationServices.Application _app;
        private readonly UIDocument _uiDoc;

        private readonly EventHandlerWithStringArg _mExternalMethodStringArg;
        private readonly DeleteSharedWPFWrappedEvent _mExternalMethodWpfArg;


        public DeleteSharedWPF(UIApplication uiApp, EventHandlerWithStringArg evExternalMethodStringArg,
            DeleteSharedWPFWrappedEvent eExternalMethodWpfArg)
        {
            _uiDoc = uiApp.ActiveUIDocument;
            _doc = _uiDoc.Document;
            _app = _doc.Application;
            _uiApp = _uiDoc.Application;
            Closed += MainWindow_Closed;
            InitializeComponent();

            _mExternalMethodStringArg = evExternalMethodStringArg;
            _mExternalMethodWpfArg = eExternalMethodWpfArg;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Close();
        }
        private void Click(object sender, RoutedEventArgs e)
        {
            _mExternalMethodWpfArg.Raise(this);
        }
    }
}
