using System.Windows;
using System.Windows.Controls;

namespace ProdigyFramework.TemplateSelector
{
    public class InlineDataTemplateSelector : DataTemplateSelector
    {
        public System.Windows.ResourceDictionary DataTemplates { get; set; } = new System.Windows.ResourceDictionary();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate selectedTemplate = null;
            foreach (var template in DataTemplates.Values)
            {
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                if ((template as DataTemplate).DataType == item.GetType())
                {
                    selectedTemplate = template as DataTemplate;
                    break;
                }
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            }

            return selectedTemplate;
        }
    }
}
