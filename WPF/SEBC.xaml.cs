﻿using System;
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
    public partial class SEBC : System.Windows.Window
    {
        private Document doc;
        private Application app;
        private UIDocument uidoc;
        private IList<Category> allModelCategories;

        public SEBC(Document doc, Application app, UIDocument uidoc)
        {
            this.doc = doc;
            this.app = app;
            this.uidoc = uidoc;
            InitializeComponent();
            allModelCategories = GetAllModelCategories(doc);
            AddCheckboxesForCategories(listBox, doc, allModelCategories);
        }

        private IList<Category> GetAllModelCategories(Document doc)
        {
            Categories cats = doc.Settings.Categories;
            IList<Category> cats_listed = new List<Category>();
            foreach (Category cat in cats)
            {
                if (cat.SubCategories.Size != 0 && cat.CategoryType == CategoryType.Model)
                {
                    cats_listed.Add(cat);
                }

            }
            IEnumerable<Category> sortedEnum = cats_listed.OrderBy(f => f.Name);
            IList<Category> sortedList = sortedEnum.ToList();
            return sortedList;
        }

        private IList<Category> GetCategoriesFromView(Document doc)
        {
            IList<Category> categories = new List<Category>();
            foreach (Element elem in collectCurrent(doc))
            {
                try
                {
                    bool match = false;
                    if (categories.Count > 0)
                    {
                        foreach (Category cat in categories)
                        {
                            if (elem.Category != null && cat.Name == elem.Category.Name)
                            {
                                match = true;
                            }
                        }
                    }


                    if (elem.Category != null &&
                        elem.Category.CategoryType == CategoryType.Model
                            && elem.Category.AllowsBoundParameters == true
                            && elem.Category.Id.ToString() != "-2008043"
                            && match == false)
                    {
                        categories.Add(elem.Category);
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message,"Error");
                }


            }

            //MessageBox.Show("12");
            return categories;
        }




        private IList<Element> collectCurrent(Document doc)
        {
            FilteredElementCollector allElementsInView =
                new FilteredElementCollector(doc, doc.ActiveView.Id);
            return allElementsInView.WhereElementIsNotElementType().ToElements();
        }

        private void AddCheckboxesForCategories(ListBox lstbox, Document doc, IList<Category> cats)
        {
            foreach (Category c in cats)
            {
                try
                {
                    var txtMsgConversation = new CheckBox()
                    {

                        Padding = new Thickness(1),
                        IsEnabled = true,
                        //IsReadOnly = true,
                        Background = Brushes.Transparent,
                        //Foreground = Brushes.White,
                        Width = 180,
                        //Height = 15,
                        VerticalAlignment = VerticalAlignment.Top,
                        VerticalContentAlignment = VerticalAlignment.Top,
                        //Content = UserData.Name, //+ "\n" + UserData.ContactNo,
                        Content = c.Name //+ "\n" + c.Id.ToString(),
                        //Margin = new Thickness(10, 10, 10, 10)
                    };
                    txtMsgConversation.Name = string.Concat(c.Name.Where(c => !Char.IsWhiteSpace(c)));
                    var SpConversation = new StackPanel() { Orientation = Orientation.Horizontal };

                    SpConversation.Children.Add(txtMsgConversation);

                    var item = new ListBoxItem()
                    {
                        //Name = c.Name,
                        Content = SpConversation,
                        //Uid = UserData.Id.ToString(CultureInfo.InvariantCulture),
                        //Background = Brushes.Black,
                        //Foreground = Brushes.White,
                        //BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Gray
                    };

                    //item.Tag = "q123";
                    //item.Name = c.Name;
                    lstbox.Items.Add(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Couldn't create a checkbox");
                }
            }


        }
        private void Click(object sender, RoutedEventArgs e)
        {

            IList<Category> checkedcats = new List<Category>();
            IList<Element> elements = new List<Element>();

            foreach (Category c in allModelCategories)
            {
                CheckBox foundTextBox = null;
                try
                {
                    foundTextBox = FindChild<CheckBox>(listBox, string.Concat(c.Name.Where(c => !Char.IsWhiteSpace(c))));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error finding checkbox");
                }


                if (foundTextBox != null && foundTextBox.IsChecked.ToString() == "True")
                {
                    checkedcats.Add(c);
                    FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
                    IList<Element> elems = new List<Element>();
                    elems = viewCollector.OfCategoryId(c.Id).WhereElementIsNotElementType().ToElements();
                    foreach (Element elem in elems)
                    {
                        if (elem.Id != null) { elements.Add(elem); }
                    }
                }
            }


            try
            {
                List<ElementId> elementIds = elements.Select(e => e.Id).ToList();
                uidoc.Selection.SetElementIds(elementIds);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed selecting elements");
            }
            this.Close();
        }
        public event EventHandler? CheckedChanged;

        private void CheckBox1_CheckedChanged(Object sender, EventArgs e)
        {
            bool? state = this.CheckBox1.IsChecked;
            if (state == true)
            {
                listBox.Items.Clear();
                AddCheckboxesForCategories(listBox, doc, GetCategoriesFromView(doc));
            }
            else if (state == false)
            {
                listBox.Items.Clear();
                AddCheckboxesForCategories(listBox, doc, allModelCategories);
            }
            MessageBox.Show(state.ToString());
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}
