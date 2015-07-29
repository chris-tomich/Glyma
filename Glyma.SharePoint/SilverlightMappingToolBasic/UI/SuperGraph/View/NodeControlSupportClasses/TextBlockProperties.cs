using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public static class TextBlockProperties
    {
        public static string GetStyledText(DependencyObject obj)
        {
            return (string)obj.GetValue(StyledTextProperty);
        }

        public static void SetStyledText(DependencyObject obj, string value)
        {
            obj.SetValue(StyledTextProperty, value);
        }

        public static readonly DependencyProperty StyledTextProperty =
            DependencyProperty.RegisterAttached("StyledText", typeof(string), typeof(TextBlock), new PropertyMetadata(null, StyledText_Changed));


        private static void StyledText_Changed(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tb = (TextBlock)d;
            var text = (string)args.NewValue;

            if (string.IsNullOrEmpty(text) || !Regex.IsMatch(text, "<Run.*?>.*?</Run>"))
            {
                tb.Text = text;
                return;
            }

            var formattedTextBlockXaml = "<TextBlock xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" + text + "</TextBlock>";
            var formattedTextBlock = (TextBlock)XamlReader.Load(formattedTextBlockXaml);

            // detach parsed inlines from the view tree
            var inlines = formattedTextBlock.Inlines.ToList();
            formattedTextBlock.Inlines.Clear();

            // add inlines to the specified text block
            tb.Inlines.Clear();
            foreach (var inline in inlines)
            {
                tb.Inlines.Add(inline);
            }
        }
    }
}
