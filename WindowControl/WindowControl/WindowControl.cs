using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using AeroColor;
using Cursors = System.Windows.Input.Cursors;

namespace Controls
{
    [ContentProperty("Content")]
    public class WindowControl : Window
    {
        private readonly Border _coloredBorder;
        private readonly Grid _outerContainer;
        private readonly ContentPresenter _windowContentContainer;

        private IntPtr _windowHandle;

        public WindowControl()
        {
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;

            SnapsToDevicePixels = true;

            var outerContainer = new Grid();

            outerContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            outerContainer.RowDefinitions.Add(new RowDefinition());
            outerContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            outerContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            outerContainer.ColumnDefinitions.Add(new ColumnDefinition());
            outerContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            var borderSize = SystemInformation.BorderSize;

            var windowContentContainer = new ContentPresenter();
            windowContentContainer.Margin = new Thickness(borderSize.Width, borderSize.Height, borderSize.Width,
                borderSize.Height);
            Grid.SetRowSpan(windowContentContainer, 3);
            Grid.SetColumnSpan(windowContentContainer, 3);

            var coloredBorder = new Border();

            coloredBorder.BorderThickness = new Thickness(borderSize.Width, borderSize.Height, borderSize.Width,
                borderSize.Height);
            Grid.SetRowSpan(coloredBorder, 3);
            Grid.SetColumnSpan(coloredBorder, 3);

            outerContainer.Children.Add(coloredBorder);
            outerContainer.Children.Add(windowContentContainer);

            _windowContentContainer = windowContentContainer;
            _outerContainer = outerContainer;
            _coloredBorder = coloredBorder;

            base.Content = outerContainer;

            CreateResizingBorder();

            Loaded += WindowControl_Loaded;
        }

        private Rectangle CreateResizingEdge()
        {
            var border = new Rectangle();
            border.Cursor = Cursors.SizeAll;
            border.Fill = Brushes.Transparent;
            border.ForceCursor = true;
            border.IsHitTestVisible = true;
            return border;
        }

        private void CreateResizingBorder()
        {
            var resizeBorderSize = SystemInformation.FrameBorderSize;

            //corners.
            var upperLeft = CreateResizingEdge();
            upperLeft.Width = resizeBorderSize.Width;
            upperLeft.Height = resizeBorderSize.Height;
            upperLeft.Cursor = Cursors.SizeNWSE;
            upperLeft.MouseDown += MouseDownUpperLeft;
            Grid.SetRow(upperLeft, 0);
            Grid.SetColumn(upperLeft, 0);

            var upperRight = CreateResizingEdge();
            upperRight.Width = resizeBorderSize.Width;
            upperRight.Height = resizeBorderSize.Height;
            upperRight.Cursor = Cursors.SizeNESW;
            upperRight.MouseDown += MouseDownUpperRight;
            Grid.SetRow(upperRight, 0);
            Grid.SetColumn(upperRight, 2);

            var lowerLeft = CreateResizingEdge();
            lowerLeft.Width = resizeBorderSize.Width;
            lowerLeft.Height = resizeBorderSize.Height;
            lowerLeft.Cursor = Cursors.SizeNESW;
            lowerLeft.MouseDown += MouseDownLowerLeft;
            Grid.SetRow(lowerLeft, 2);
            Grid.SetColumn(lowerLeft, 0);

            var lowerRight = CreateResizingEdge();
            lowerRight.Width = resizeBorderSize.Width;
            lowerRight.Height = resizeBorderSize.Height;
            lowerRight.Cursor = Cursors.SizeNWSE;
            lowerRight.MouseDown += MouseDownLowerRight;
            Grid.SetRow(lowerRight, 2);
            Grid.SetColumn(lowerRight, 2);

            //edges.
            var left = CreateResizingEdge();
            left.Width = resizeBorderSize.Width;
            left.Cursor = Cursors.SizeWE;
            left.MouseDown += MouseDownLeft;
            Grid.SetRow(left, 1);
            Grid.SetColumn(left, 0);

            var right = CreateResizingEdge();
            right.Width = resizeBorderSize.Width;
            right.Cursor = Cursors.SizeWE;
            right.MouseDown += MouseDownRight;
            Grid.SetRow(right, 1);
            Grid.SetColumn(right, 2);

            var upper = CreateResizingEdge();
            upper.Height = resizeBorderSize.Height;
            upper.Cursor = Cursors.SizeNS;
            upper.MouseDown += MouseDownUpper;
            Grid.SetRow(upper, 0);
            Grid.SetColumn(upper, 1);

            var lower = CreateResizingEdge();
            lower.Height = resizeBorderSize.Height;
            lower.Cursor = Cursors.SizeNS;
            lower.MouseDown += MouseDownLower;
            Grid.SetRow(lower, 2);
            Grid.SetColumn(lower, 1);

            //add everything.
            _outerContainer.Children.Add(upperLeft);
            _outerContainer.Children.Add(upper);
            _outerContainer.Children.Add(upperRight);
            _outerContainer.Children.Add(left);
            _outerContainer.Children.Add(right);
            _outerContainer.Children.Add(lowerLeft);
            _outerContainer.Children.Add(lower);
            _outerContainer.Children.Add(lowerRight);
        }

        private void ResizeWindow(NativeMethods.ResizeDirection direction)
        {
            if (_windowHandle != default(IntPtr))
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(_windowHandle, NativeMethods.WM_NCLBUTTONDOWN,
                    61440 + (int)direction, 0);
            }
        }

        private void MouseDownLowerLeft(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ResizeWindow(NativeMethods.ResizeDirection.BottomLeft);
        }

        private void MouseDownLowerRight(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ResizeWindow(NativeMethods.ResizeDirection.BottomRight);
        }

        private void MouseDownRight(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ResizeWindow(NativeMethods.ResizeDirection.Right);
        }

        private void MouseDownLower(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ResizeWindow(NativeMethods.ResizeDirection.Bottom);
        }

        private void MouseDownLeft(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ResizeWindow(NativeMethods.ResizeDirection.Left);
        }

        private void MouseDownUpper(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ResizeWindow(NativeMethods.ResizeDirection.Top);
        }

        private void MouseDownUpperRight(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ResizeWindow(NativeMethods.ResizeDirection.TopRight);
        }

        private void MouseDownUpperLeft(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ResizeWindow(NativeMethods.ResizeDirection.TopLeft);
        }

        void WindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            AeroResourceInitializer.Initialize();
            _coloredBorder.SetResourceReference(Border.BorderBrushProperty, "AeroBrushDark"); 
            
            _windowHandle = new WindowInteropHelper(this).Handle;
        }

        public new object Content
        {
            get { return base.Content; }
            set { base.Content = value; }
        }

        public new static DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object),
            typeof(WindowControl), new PropertyMetadata(null, ContentPropertyChangedCallback));

        private static void ContentPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            //HACK: avoid swapping contents.
            var windowControl = (WindowControl)dependencyObject;
            windowControl.Content = windowControl._outerContainer;
            windowControl._windowContentContainer.Content = dependencyPropertyChangedEventArgs.NewValue as UIElement;
        }
    }
}
