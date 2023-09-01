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
using RG_Tools.CopyValues;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace RG_Tools
{
    /// <summary>
    /// Interaction logic for CopyValuesWPF.xaml
    /// </summary>
    public partial class CopyValuesWPF : System.Windows.Window
    {
        private readonly Document _doc;

#pragma warning disable IDE0052 // Remove unread private members
        private readonly UIApplication _uiApp;
        private readonly Autodesk.Revit.ApplicationServices.Application _app;
        private readonly EventHandlerWithStringArg _mExternalMethodStringArg;
#pragma warning restore IDE0052 // Remove unread private members
        private readonly UIDocument _uiDoc;        
        private readonly CopyValuesWPFWrappedEvent _mExternalMethodWpfArg;

        #region Selectedui Constructor
        private ICollection<ElementId> _selectedui;
        public ICollection<ElementId> Selectedui
        {
            get { return _selectedui; }
            set { _selectedui = value; }
        }
        #endregion

        public CopyValuesWPF(UIApplication uiApp, EventHandlerWithStringArg evExternalMethodStringArg,
            CopyValuesWPFWrappedEvent eExternalMethodWpfArg)
        {
            _uiDoc = uiApp.ActiveUIDocument;
            _doc = _uiDoc.Document;
            _app = _doc.Application;
            _uiApp = _uiDoc.Application;
            Closed += MainWindow_Closed;

            InitializeComponent();
            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
            _mExternalMethodStringArg = evExternalMethodStringArg;
            _mExternalMethodWpfArg = eExternalMethodWpfArg;
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            // Raise external event with this UI instance (WPF) as an argument
            _mExternalMethodWpfArg.Raise(this);
        }
    }
}
