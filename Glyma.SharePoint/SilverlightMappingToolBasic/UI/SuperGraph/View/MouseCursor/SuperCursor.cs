using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.MouseCursor
{
    public class SuperCursor : IDisposable
    {
		private readonly FrameworkElement _element;
		private Cursor _originalCursor;
		private Popup _cursorContainer;

        private static readonly DependencyProperty SuperCursorProperty =
			DependencyProperty.RegisterAttached("SuperCursor", typeof(SuperCursor), typeof(SuperCursor), null);

        private SuperCursor(FrameworkElement element, DataTemplate template)
		{
			_element = element;
            element.SetValue(SuperCursorProperty, this);
			_originalCursor = element.Cursor;
			element.Cursor = Cursors.None;
			element.MouseLeave += element_MouseLeave;
			element.MouseMove += element_MouseMove;
            _cursorContainer = new Popup
            {
                IsOpen = false,
                Child = new ContentControl
                {
                    ContentTemplate = template,
                    IsHitTestVisible = false,
                    RenderTransform = new TranslateTransform()
                },
                IsHitTestVisible = false
            };
		}

        public void SetTemplate(DataTemplate template)
        {
            if (template != null)
            {
                if (_cursorContainer != null)
                {
                    var content = _cursorContainer.Child as ContentControl;
                    if (content != null)
                    {
                        content.ContentTemplate = template;
                    }

                    if (!_cursorContainer.IsOpen)
                    {
                        _element.MouseLeave += element_MouseLeave;
                        _element.MouseMove += element_MouseMove;
                        _cursorContainer.IsOpen = true;
                        _originalCursor = _element.Cursor;
                        _element.Cursor = Cursors.None;
                    }
                }

                
            }
            else
            {
                _element.MouseLeave -= element_MouseLeave;
                _element.MouseMove -= element_MouseMove;
                _cursorContainer.IsOpen = false;
                _element.Cursor = _originalCursor;
            }
        }

        public bool IsShowing
        {
            get { return _cursorContainer.IsOpen; }
            set { _cursorContainer.IsOpen = value; }
        }

        private void element_MouseMove(object sender, MouseEventArgs e)
		{
			_cursorContainer.IsOpen = true;
			var p = e.GetPosition(null);
			var t = (_cursorContainer.Child.RenderTransform as TranslateTransform);
		    if (t != null)
		    {
		        t.X = p.X;
		        t.Y = p.Y;
		    }
		}

		private void element_MouseLeave(object sender, MouseEventArgs e)
		{
			_cursorContainer.IsOpen = false;
		}

		public void Dispose()
		{
			_element.MouseLeave -= element_MouseLeave;
			_element.MouseMove -= element_MouseMove;
            _element.ClearValue(SuperCursorProperty);
			_element.Cursor = _originalCursor;
			_cursorContainer.IsOpen = false;
			_cursorContainer = null;
		}


        public static DataTemplate GetCursorTemplate(DependencyObject obj)
		{
			return (DataTemplate)obj.GetValue(CursorTemplateProperty);
		}

		public static void SetCursorTemplate(DependencyObject obj, DataTemplate value)
		{
			obj.SetValue(CursorTemplateProperty, value);
		}

        public static readonly DependencyProperty CursorTemplateProperty =
            DependencyProperty.RegisterAttached("CursorTemplate", typeof(DataTemplate), typeof(SuperCursor), new PropertyMetadata(OnCursorTemplatePropertyChanged));

		private static void OnCursorTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			
			var element = (d as FrameworkElement);
		    if (element != null)
		    {
                var superCursor = element.GetValue(SuperCursorProperty) as SuperCursor;
                if (superCursor != null)
                {
                    superCursor.SetTemplate(e.NewValue as DataTemplate);
                }
                else
                {
                    element.SetValue(SuperCursorProperty, new SuperCursor(element, e.NewValue as DataTemplate));
                }
		    }
		    else
		    {
                throw new ArgumentOutOfRangeException("Property can only be attached to FrameworkElements");
		    }
		    
		}
	}
}
