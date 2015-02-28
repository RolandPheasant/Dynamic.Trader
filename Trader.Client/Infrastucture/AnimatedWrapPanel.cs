using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Trader.Client.Infrastucture
{
    /// <summary>
    /// Lifted from http://tech.pro/tutorial/736/wpf-tutorial-creating-a-custom-panel-control
    /// </summary>
    public class AnimatedWrapPanel : Panel
    {
        private readonly TimeSpan _animationLength = TimeSpan.FromMilliseconds(250);

        protected override Size MeasureOverride(Size availableSize)
        {
            var infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            double curX = 0, curY = 0, curLineHeight = 0;
            foreach (UIElement child in Children)
            {
                child.Measure(infiniteSize);

                if (curX + child.DesiredSize.Width > availableSize.Width)
                { //Wrap to next line
                    curY += curLineHeight;
                    curX = 0;
                    curLineHeight = 0;
                }

                curX += child.DesiredSize.Width;
                if(child.DesiredSize.Height > curLineHeight)
                    curLineHeight = child.DesiredSize.Height;
            }

            curY += curLineHeight;

            var resultSize = new Size
            {
                Width = double.IsPositiveInfinity(availableSize.Width)
                    ? curX
                    : availableSize.Width,
                Height = double.IsPositiveInfinity(availableSize.Height)
                    ? curY
                    : availableSize.Height
            };

            return resultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children == null || this.Children.Count == 0)
                return finalSize;

            double curX = 0, curY = 0, curLineHeight = 0;

            foreach (UIElement child in Children)
            {
                var trans = child.RenderTransform as TranslateTransform;


                if (trans == null)
                {
                    child.RenderTransformOrigin = new Point(0, 0);
                    trans = new TranslateTransform();
                    child.RenderTransform = trans;
                }

                if (curX + child.DesiredSize.Width > finalSize.Width)
                { //Wrap to next line
                    curY += curLineHeight;
                    curX = 0;
                    curLineHeight = 0;
                }

                child.Arrange(new Rect(0, 0, child.DesiredSize.Width, 
                    child.DesiredSize.Height));


                trans.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(curX, _animationLength), HandoffBehavior.Compose);
                trans.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(curY, _animationLength), HandoffBehavior.Compose);



                curX += child.DesiredSize.Width;
                if (child.DesiredSize.Height > curLineHeight)
                    curLineHeight = child.DesiredSize.Height;       
            }

            return finalSize;
        } 
    }
}