﻿using System.Windows;
using System.Windows.Media.Animation;

namespace SilverlightMappingToolBasic.UI.Extensions
{
    public class ThicknessAnimation
    {
        public static readonly DependencyProperty ElementProperty = DependencyProperty.RegisterAttached("Element", typeof(DependencyObject), typeof(DoubleAnimation), new PropertyMetadata(OnElementPropertyChanged));

        // The time along the animation from 0-1
        public static DependencyProperty TimeProperty = DependencyProperty.RegisterAttached("Time", typeof(double), typeof(DoubleAnimation), new PropertyMetadata(OnTimeChanged));

        // The object being animated
        public static DependencyProperty TargetProperty = DependencyProperty.RegisterAttached("Target", typeof(DependencyObject), typeof(ThicknessAnimation), null);
        public static DependencyProperty TargetPropertyProperty = DependencyProperty.RegisterAttached("TargetProperty", typeof(DependencyProperty), typeof(DependencyObject), null);

        public static readonly DependencyProperty FromProperty = DependencyProperty.RegisterAttached("From", typeof(Thickness), typeof(DoubleAnimation), null);
        public static readonly DependencyProperty ToProperty = DependencyProperty.RegisterAttached("To", typeof(Thickness), typeof(DoubleAnimation), null);

        public static void SetElement(DependencyObject o, DependencyObject value)
        {
            o.SetValue(ElementProperty, value);
        }

        public static DependencyObject GetElement(DependencyObject o)
        {
            return (DependencyObject)o.GetValue(ElementProperty);
        }

        private static void OnElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var doubleAnimation = (DoubleAnimation)d;

                doubleAnimation.SetValue(TargetProperty, e.NewValue);
                doubleAnimation.From = 0;
                doubleAnimation.To = 1;
                doubleAnimation.SetValue(TargetPropertyProperty, FrameworkElement.MarginProperty);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(ThicknessAnimation.Time)"));
                Storyboard.SetTarget(doubleAnimation, doubleAnimation);
            }
        }


        private static void OnTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var animation = (DoubleAnimation)sender;
            var time = GetTime(animation);
            var from = (Thickness)sender.GetValue(FromProperty);
            var to = (Thickness)sender.GetValue(ToProperty);
            var targetProperty = (DependencyProperty)sender.GetValue(TargetPropertyProperty);
            var target = (DependencyObject)sender.GetValue(TargetProperty);
            target.SetValue(targetProperty, new Thickness((to.Left - from.Left) * time + from.Left,
                                                          (to.Top - from.Top) * time + from.Top,
                                                          (to.Right - from.Right) * time + from.Right,
                                                          (to.Bottom - from.Bottom) * time + from.Bottom));
        }

        public static double GetTime(DoubleAnimation animation)
        {
            return (double)animation.GetValue(TimeProperty);
        }

        public static void SetTime(DoubleAnimation animation, double value)
        {
            animation.SetValue(TimeProperty, value);
        }

        public static Thickness GetFrom(DoubleAnimation animation)
        {
            return (Thickness)animation.GetValue(FromProperty);
        }

        public static void SetFrom(DoubleAnimation animation, Thickness value)
        {
            animation.SetValue(FromProperty, value);
        }

        public static Thickness GetTo(DoubleAnimation animation)
        {
            return (Thickness)animation.GetValue(ToProperty);
        }

        public static void SetTo(DoubleAnimation animation, Thickness value)
        {
            animation.SetValue(ToProperty, value);
        }
    }   
}
