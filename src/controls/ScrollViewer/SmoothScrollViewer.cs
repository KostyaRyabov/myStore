using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Reflection;
using System.Windows.Interactivity;

namespace myStore.src.controls.ScrollViewer
{
    public class ScrollInfoAdapter : UIElement, IScrollInfo
    {
        private IScrollInfo _child;
        private double _computedVerticalOffset = 0;
        private double _computedHorizontalOffset = 0;
        internal const double _scrollLineDelta = 16.0;
        internal const double _mouseWheelDelta = 48.0;

        public ScrollInfoAdapter(IScrollInfo child)
        {
            _child = child;
        }

        public bool CanVerticallyScroll
        {
            get => _child.CanVerticallyScroll;
            set => _child.CanVerticallyScroll = value;
        }
        public bool CanHorizontallyScroll
        {
            get => _child.CanHorizontallyScroll;
            set => _child.CanHorizontallyScroll = value;
        }

        public double ExtentWidth => _child.ExtentWidth;

        public double ExtentHeight => _child.ExtentHeight;

        public double ViewportWidth => _child.ViewportWidth;

        public double ViewportHeight => _child.ViewportHeight;

        public double HorizontalOffset => _child.HorizontalOffset;
        public double VerticalOffset => _child.VerticalOffset;

        public System.Windows.Controls.ScrollViewer ScrollOwner
        {
            get => _child.ScrollOwner;
            set => _child.ScrollOwner = value;
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return _child.MakeVisible(visual, rectangle);
        }

        public void LineUp()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.LineUp();
            else
                VerticalScroll(_computedVerticalOffset - _scrollLineDelta);
        }

        public void LineDown()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.LineDown();
            else
                VerticalScroll(_computedVerticalOffset + _scrollLineDelta);
        }

        public void LineLeft()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.LineLeft();
            else
                HorizontalScroll(_computedHorizontalOffset - _scrollLineDelta);
        }

        public void LineRight()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.LineRight();
            else
                HorizontalScroll(_computedHorizontalOffset + _scrollLineDelta);
        }

        public void MouseWheelUp()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.MouseWheelUp();
            else
                VerticalScroll(_computedVerticalOffset - _mouseWheelDelta);
        }

        public void MouseWheelDown()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.MouseWheelDown();
            else
                VerticalScroll(_computedVerticalOffset + _mouseWheelDelta);
        }

        public void MouseWheelLeft()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.MouseWheelLeft();
            else
                HorizontalScroll(_computedHorizontalOffset - _mouseWheelDelta);
        }

        public void MouseWheelRight()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.MouseWheelRight();
            else
                HorizontalScroll(_computedHorizontalOffset + _mouseWheelDelta);
        }

        public void PageUp()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.PageUp();
            else
                VerticalScroll(_computedVerticalOffset - ViewportHeight);
        }

        public void PageDown()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.PageDown();
            else
                VerticalScroll(_computedVerticalOffset + ViewportHeight);
        }

        public void PageLeft()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.PageLeft();
            else
                HorizontalScroll(_computedHorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.PageRight();
            else
                HorizontalScroll(_computedHorizontalOffset + ViewportWidth);
        }

        public void SetHorizontalOffset(double offset)
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.SetHorizontalOffset(offset);
            else
            {
                _computedHorizontalOffset = offset;
                Animate(HorizontalScrollOffsetProperty, offset);
            }
        }

        public void SetVerticalOffset(double offset)
        {
            if (_child.ScrollOwner.CanContentScroll == true)
                _child.SetVerticalOffset(offset);
            else
            {
                _computedVerticalOffset = offset;
                Animate(VerticalScrollOffsetProperty, offset);
            }
        }

        #region not exposed methods
        private void Animate(DependencyProperty property, double targetValue, int duration = 200)
        {
            //make a smooth animation that starts and ends slowly

            var keyFramesAnimation = new DoubleAnimationUsingKeyFrames()
            {
                IsCumulative=true,
                //IsAdditive=true,
                Duration = TimeSpan.FromMilliseconds(duration),
                KeyFrames = new DoubleKeyFrameCollection()
                {
                    new SplineDoubleKeyFrame(
                        targetValue,
                        KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration)),
                        new KeySpline(0.1, 0.0, 0.1, 1.0)
                    )
                }
            };

            BeginAnimation(property, keyFramesAnimation);
        }

        private void VerticalScroll(double val)
        {
            if (Math.Abs(_computedVerticalOffset - ValidateVerticalOffset(val)) > 0.1)//prevent restart of animation in case of frequent event fire
            {
                _computedVerticalOffset = ValidateVerticalOffset(val);
                Animate(VerticalScrollOffsetProperty, _computedVerticalOffset, 0);
            }
        }

        private void HorizontalScroll(double val)
        {
            if (Math.Abs(_computedHorizontalOffset - ValidateHorizontalOffset(val)) > 0.1)//prevent restart of animation in case of frequent event fire
            {
                _computedHorizontalOffset = ValidateHorizontalOffset(val);
                Animate(HorizontalScrollOffsetProperty, _computedHorizontalOffset);
            }
        }

        private double ValidateVerticalOffset(double verticalOffset)
        {
            if (verticalOffset < 0)
                return 0;
            if (verticalOffset > _child.ScrollOwner.ScrollableHeight)
                return _child.ScrollOwner.ScrollableHeight;
            return verticalOffset;
        }

        private double ValidateHorizontalOffset(double horizontalOffset)
        {
            if (horizontalOffset < 0)
                return 0;
            if (horizontalOffset > _child.ScrollOwner.ScrollableWidth)
                return _child.ScrollOwner.ScrollableWidth;
            return horizontalOffset;
        }
        #endregion

        #region helper dependency properties as scrollbars are not animatable by default
        internal double VerticalScrollOffset
        {
            get { return (double)GetValue(VerticalScrollOffsetProperty); }
            set { SetValue(VerticalScrollOffsetProperty, value); }
        }
        internal static readonly DependencyProperty VerticalScrollOffsetProperty =
            DependencyProperty.Register("VerticalScrollOffset", typeof(double), typeof(ScrollInfoAdapter),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnVerticalScrollOffsetChanged)));
        private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var smoothScrollViewer = (ScrollInfoAdapter)d;
            smoothScrollViewer._child.SetVerticalOffset((double)e.NewValue);
        }

        internal double HorizontalScrollOffset
        {
            get { return (double)GetValue(HorizontalScrollOffsetProperty); }
            set { SetValue(HorizontalScrollOffsetProperty, value); }
        }
        internal static readonly DependencyProperty HorizontalScrollOffsetProperty =
            DependencyProperty.Register("HorizontalScrollOffset", typeof(double), typeof(ScrollInfoAdapter),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnHorizontalScrollOffsetChanged)));
        private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var smoothScrollViewer = (ScrollInfoAdapter)d;
            smoothScrollViewer._child.SetHorizontalOffset((double)e.NewValue);
        }
        #endregion
    }


    public class SmoothScrollViewer : System.Windows.Controls.ScrollViewer
    {
        public SmoothScrollViewer()
        {
            Loaded += ScrollViewer_Loaded;
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollInfo = new ScrollInfoAdapter(ScrollInfo);
        }
    }

    public class SmoothScrollViewerBehavior : Behavior<System.Windows.Controls.ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += ScrollViewerLoaded;
        }

        private void ScrollViewerLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var property = AssociatedObject.GetType().GetProperty("ScrollInfo", BindingFlags.NonPublic | BindingFlags.Instance);
            property.SetValue(AssociatedObject, new ScrollInfoAdapter((IScrollInfo)property.GetValue(AssociatedObject)));
        }
    }
}