using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SkipAndZip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Func<EventPattern<MouseButtonEventArgs>, Point> getPosition = args =>
                args.EventArgs.GetPosition(_box);

            var points = Observable
                .FromEventPattern<MouseButtonEventArgs>(this, "MouseUp")
                .Select(getPosition);

            points
                .Zip(points.Skip(1), Line.Create)
                .Subscribe(UpdateUi);
        }

        private void UpdateUi(Line line)
        {
            _label.Content = line;
            _box.Children.Add(new System.Windows.Shapes.Line
            {
                Stroke = System.Windows.Media.Brushes.Black,
                X1 = line.PointA.X,
                Y1 = line.PointA.Y,
                X2 = line.PointB.X,
                Y2 = line.PointB.Y,
                StrokeThickness = 2
            });

            var label = new Label
            {
                Content = line.Length.ToString("N2"),
                Background = System.Windows.Media.Brushes.LightGray,
            };

            _box.Children.Add(label);

            Canvas.SetLeft(label, line.PointB.X);
            Canvas.SetTop(label, line.PointB.Y);
        }
    }

    public class Line
    {
        public Point PointA { get; private set; }
        public Point PointB { get; private set; }

        public double Length
        {
            get
            {
                var d = Point.Subtract(PointA, PointB);
                return Math.Sqrt(Math.Abs(d.X * d.X - d.Y * d.Y));
            }
        }

        private Line(Point a, Point b) { PointA = a; PointB = b; }

        public static Line Create(Point a, Point b)
        {
            return new Line(a, b);
        }

        public override string ToString()
        {
            return String.Format("distance P1 ({1:N0}; {2:N0}) - P2 ({3:N0}; {4:N0}) = {0:N2}", Length, PointA.X, PointA.Y, PointB.X, PointB.Y);
        }
    }
}
