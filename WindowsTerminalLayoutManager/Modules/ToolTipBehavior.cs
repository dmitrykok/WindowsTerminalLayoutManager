using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TerminalLayoutManager
{
    public static class ToolTipBehavior
    {
        public static readonly DependencyProperty StaysOpenProperty =
            DependencyProperty.RegisterAttached(
                "StaysOpen",
                typeof(bool),
                typeof(ToolTipBehavior),
                new PropertyMetadata(false, OnStaysOpenChanged));

        public static bool GetStaysOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(StaysOpenProperty);
        }

        public static void SetStaysOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(StaysOpenProperty, value);
        }

        private static void OnStaysOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ToolTip toolTip)
            {
                toolTip.StaysOpen = (bool)e.NewValue;

                if ((bool)e.NewValue)
                {
                    toolTip.MouseEnter += ToolTip_MouseEnter;
                    toolTip.MouseLeave += ToolTip_MouseLeave;
                }
                else
                {
                    toolTip.MouseEnter -= ToolTip_MouseEnter;
                    toolTip.MouseLeave -= ToolTip_MouseLeave;
                }
            }
        }

        private static void ToolTip_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ToolTip toolTip)
            {
                toolTip.IsOpen = true;
            }
        }

        private static void ToolTip_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is ToolTip toolTip)
            {
                var pos = e.GetPosition(toolTip);
                if (pos.X < 0 || pos.X >= toolTip.ActualWidth || pos.Y < 0 || pos.Y >= toolTip.ActualHeight)
                {
                    toolTip.IsOpen = false;
                }
            }
        }
    }
}
