using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PGYMiniCooper.CoreModule.View.CustomView
{
    public class SelectionItem<T> : INotifyPropertyChanged
    {
        #region privat fields
        /// <summary>
        /// indicates if the item is selected
        /// </summary>
        private bool _isSelected;
        #endregion

        public SelectionItem(T element, bool isSelected)
        {
            Element = element;
            IsSelected = IsSelected;
        }

        public SelectionItem(T element) : this(element, false)
        {

        }

        #region public properties
        /// <summary>
        /// this UI-aware indicates if the element is selected or not
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        /// <summary>
        /// the element itself
        /// </summary>
        public T Element { get; set; }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public class SelectionList<T> : List<SelectionItem<T>>, INotifyPropertyChanged
    {
        #region private fields
        /// <summary>
        /// the number of selected elements
        /// </summary>
        private int _selectionCount;
        #endregion

        #region private methods
        /// <summary>
        /// this events responds to the "IsSelectedEvent"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as SelectionItem<T>;
            if ((item != null) && e.PropertyName == "IsSelected")
            {
                if (item.IsSelected)
                    SelectionCount = SelectionCount + 1;
                else
                    SelectionCount = SelectionCount - 1;
            }
        }

        #endregion



        public SelectionList()
        { }

        /// <summary>
        /// creates the selection list from an existing simple list
        /// </summary>
        /// <param name="elements"></param>
        public SelectionList(IEnumerable<T> elements)
        {
            foreach (T element in elements)
                AddItem(element);
        }

        #region public methods
        /// <summary>
        /// adds an element to the element and listens to its "IsSelected" property to update the SelectionCount property
        /// use this method insteand of the "Add" one
        /// </summary>
        /// <param name="element"></param>
        public void AddItem(T element)
        {
            var item = new SelectionItem<T>(element);
            item.PropertyChanged += item_PropertyChanged;

            Add(item);
        }

        /// <summary>
        /// gets the selected elements
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetSelection()
        {
            return this.Where(e => e.IsSelected).Select(e => e.Element);
        }

        /// <summary>
        /// uses linq expression to select a part of an object (for example, only id)
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerable<U> GetSelection<U>(Func<SelectionItem<T>, U> expression)
        {
            return this.Where(e => e.IsSelected).Select(expression);
        }

        #endregion


        #region public properties
        /// <summary>
        /// the selection count property is ui-bindable, returns the number of selected elements
        /// </summary>
        public int SelectionCount
        {
            get { return _selectionCount; }

            private set
            {
                _selectionCount = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectionCount"));
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public class Course
    {
        public Course()
        { }

        public Course(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
    /// <summary>
    /// Interaction logic for TestMultiCheck.xaml
    /// </summary>
    public partial class TestMultiCheck : UserControl
    {
        public TestMultiCheck()
        {
            InitializeComponent();
            // create the list
            _list = new SelectionList<Course>(courses);
            // assign the source of the list (not necessary in an MVVM approach)
            myList.ItemsSource = _list;
            // listen to the property changed event to enable or disable the save button
            _list.PropertyChanged += list_PropertyChanged;
        }

        private SelectionList<Course> _list;

        /// <summary>
        /// the list of courses
        /// </summary>
        Course[] courses = new Course[] {
            new Course(1, "CH1"),
            new Course(2, "CH2"),
            new Course(3, "CH3"),
            new Course(4, "CH4"),
            new Course(5, "CH5")

        };

        void list_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // the save button is enabled if at least one item is checked
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // this methods checks or unchecks programmatically an item to view the UI reaction
            _list[1].IsSelected = !_list[1].IsSelected;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // get the names of the selected courses
            IEnumerable<string> selection = _list.GetSelection(elt => elt.Element.Name);
            // concantenate the strings
            var text = string.Join(",", selection);
            MessageBox.Show(text);
        }
    }
}
