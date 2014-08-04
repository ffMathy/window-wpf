using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using AeroColor;

namespace Controls
{
    [ContentProperty("Content")]
    public class WindowControl : Window
    {
        private readonly Border _border;

        public WindowControl()
        {
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;

            //create a fake border.
            var border = new Border();
            border.BorderThickness = new Thickness(2);

            base.Content = border;

            _border = border;

            Loaded += WindowControl_Loaded;
        }

        void WindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            AeroResourceInitializer.Initialize();
            _border.SetResourceReference(Border.BorderBrushProperty, "AeroBrushDark");
        }

        public new object Content
        {
            get { return _border.Child; }
            set { _border.Child = value as UIElement; }
        }

        public new static DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof (object),
            typeof (WindowControl), new PropertyMetadata(null));

    }
}
