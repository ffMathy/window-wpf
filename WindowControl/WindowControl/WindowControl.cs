using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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

        public WindowControl()
        {
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;

            var outerContainer = new Grid();

            outerContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            outerContainer.RowDefinitions.Add(new RowDefinition());
            outerContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            outerContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            outerContainer.ColumnDefinitions.Add(new ColumnDefinition());
            outerContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            var windowContentContainer = new ContentPresenter();
            Grid.SetColumn(windowContentContainer, 1);
            Grid.SetRow(windowContentContainer, 1);

            var coloredBorder = new Border();

            var borderSize = SystemInformation.BorderSize;
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
            Grid.SetRow(upperLeft, 0);
            Grid.SetColumn(upperLeft, 0);

            var upperRight = CreateResizingEdge();
            upperRight.Width = resizeBorderSize.Width;
            upperRight.Height = resizeBorderSize.Height;
            upperRight.Cursor = Cursors.SizeNESW;
            Grid.SetRow(upperRight, 0);
            Grid.SetColumn(upperRight, 2);

            var lowerLeft = CreateResizingEdge();
            lowerLeft.Width = resizeBorderSize.Width;
            lowerLeft.Height = resizeBorderSize.Height;
            lowerLeft.Cursor = Cursors.SizeNESW;
            Grid.SetRow(lowerLeft, 2);
            Grid.SetColumn(lowerLeft, 0);

            var lowerRight = CreateResizingEdge();
            lowerRight.Width = resizeBorderSize.Width;
            lowerRight.Height = resizeBorderSize.Height;
            lowerRight.Cursor = Cursors.SizeNWSE;
            Grid.SetRow(lowerRight, 2);
            Grid.SetColumn(lowerRight, 2);

            //edges.
            var left = CreateResizingEdge();
            left.Width = resizeBorderSize.Width;
            left.Cursor = Cursors.SizeWE;
            Grid.SetRow(left, 1);
            Grid.SetColumn(left, 0);

            var right = CreateResizingEdge();
            right.Width = resizeBorderSize.Width;
            right.Cursor = Cursors.SizeWE;
            Grid.SetRow(right, 1);
            Grid.SetColumn(right, 2);

            var upper = CreateResizingEdge();
            upper.Height = resizeBorderSize.Height;
            upper.Cursor = Cursors.SizeNS;
            Grid.SetRow(upper, 0);
            Grid.SetColumn(upper, 1);

            var lower = CreateResizingEdge();
            lower.Height = resizeBorderSize.Height;
            lower.Cursor = Cursors.SizeNS;
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

        void WindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            AeroResourceInitializer.Initialize();
            _coloredBorder.SetResourceReference(Border.BorderBrushProperty, "AeroBrushDark");
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
