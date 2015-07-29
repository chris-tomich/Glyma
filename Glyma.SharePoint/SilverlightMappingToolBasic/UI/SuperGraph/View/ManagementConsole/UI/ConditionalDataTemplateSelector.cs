using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public class ConditionalDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            object conditionValue = ConditionConverter.Convert(item, null, null, null);
            foreach (ConditionalDataTemplateRule rule in Rules)
            {
                if (Equals(rule.Value, conditionValue))
                {
                    return rule.DataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }

        List<ConditionalDataTemplateRule> _rules;
        public List<ConditionalDataTemplateRule> Rules
        {
            get
            {
                if (_rules == null)
                {
                    _rules = new List<ConditionalDataTemplateRule>();
                }

                return _rules;
            }
        }

        public IValueConverter ConditionConverter { get; set; }
    }

    public class ConditionalDataTemplateRule
    {
        public object Value { get; set; }

        public DataTemplate DataTemplate { get; set; }
    }
}
