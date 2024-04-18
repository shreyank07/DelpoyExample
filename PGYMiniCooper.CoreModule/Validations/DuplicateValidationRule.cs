using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PGYMiniCooper.CoreModule.Validations
{
    public class DuplicateValidationRule : ValidationRule
    {
        private CollectionContainerSource collection;

        public CollectionContainerSource Collection
        {
            get { return collection; }
            set { collection = value; }
        }

        private int key;

        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            object compareValue = value;

            if (value is System.Windows.Data.BindingExpression)
            {
                var bindingExpression = value as System.Windows.Data.BindingExpression;
                compareValue = bindingExpression.DataItem.GetType().GetProperty(bindingExpression.ResolvedSourcePropertyName)
                    .GetValue(bindingExpression.DataItem);
            }

            if (collection != null && collection.Source != null)
            {
                bool isMatches = false;
                foreach (var k in collection.Source.Keys)
                {
                    if (!k.Equals(key))
                    {
                        isMatches = collection.Source[k].Equals(compareValue);

                        if (isMatches)
                            return new ValidationResult(false, "Error: Duplicate value selected.");
                    }
                }
            }
            return new ValidationResult(true, null);
        }
    }

    public class CollectionContainerSource : DependencyObject
    {
        public static DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(IDictionary), typeof(CollectionContainerSource));

        public IDictionary Source
        {
            get { return (IDictionary)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }
    }

    public class BindingProxy : System.Windows.Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}
