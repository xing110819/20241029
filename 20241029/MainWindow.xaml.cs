using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _20241029
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Color strokeColor = Colors.Black;
        Color fillColor = Colors.Aqua;
        Brush strokeBrush = Brushes.Black;
        Brush fillBrush = Brushes.Aqua;
        string shapeType = "line";
        int strokeThickness = 1;
        Point start, dest;
        string actionType = "draw";
        public MainWindow()
        {
            InitializeComponent();
            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;
        }

        private void MyCanvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            myCanvas.Cursor = Cursors.Pen;
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Brush strokeBrush = new SolidColorBrush(strokeColor);
            Brush fillBrush = new SolidColorBrush(fillColor);

            switch (shapeType)
            {
                case "line":
                    var line = myCanvas.Children.OfType<Line>().LastOrDefault();
                    line.Stroke = strokeBrush;
                    line.StrokeThickness = strokeThickness;
                    break;
                case "rectangle":
                    var rectangle = myCanvas.Children.OfType<Rectangle>().LastOrDefault();
                    rectangle.Stroke = strokeBrush;
                    rectangle.Fill = fillBrush;
                    rectangle.StrokeThickness = strokeThickness;
                    break;
                case "ellipse":
                    var ellipse = myCanvas.Children.OfType<Ellipse>().LastOrDefault();
                    ellipse.Stroke = strokeBrush;
                    ellipse.Fill = fillBrush;
                    ellipse.StrokeThickness = strokeThickness;
                    break;
                case "polyline":
                    var polyline = myCanvas.Children.OfType<Polyline>().LastOrDefault();
                    polyline.Stroke = strokeBrush;
                    polyline.Fill = fillBrush;
                    polyline.StrokeThickness = strokeThickness;
                    break;
            }
            DisplayStatus();
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(myCanvas);
            myCanvas.Cursor = Cursors.Cross;

            if (actionType == "draw")
            {
                switch (shapeType)
                {
                    case "line":
                        Line line = new Line
                        {
                            X1 = start.X,
                            Y1 = start.Y,
                            X2 = dest.X,
                            Y2 = dest.Y,
                            Stroke = Brushes.Gray,
                            StrokeThickness = 1
                        };
                        myCanvas.Children.Add(line);
                        break;
                    case "rectangle":
                        Rectangle rectangle = new Rectangle
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        myCanvas.Children.Add(rectangle);
                        rectangle.SetValue(Canvas.LeftProperty, start.X);
                        rectangle.SetValue(Canvas.TopProperty, start.Y);
                        break;
                    case "ellipse":
                        Ellipse ellipse = new Ellipse
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        myCanvas.Children.Add(ellipse);
                        ellipse.SetValue(Canvas.LeftProperty, start.X);
                        ellipse.SetValue(Canvas.TopProperty, start.Y);
                        break;
                    case "polyline":
                        Polyline polyline = new Polyline
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        myCanvas.Children.Add(polyline);
                        break;
                }
            }
            DisplayStatus();
        }
        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            dest = e.GetPosition(myCanvas);
            DisplayStatus();

            switch (actionType)
            {
                case "draw": // 繪圖模式
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point origin;
                        origin.X = Math.Min(start.X, dest.X);
                        origin.Y = Math.Min(start.Y, dest.Y);
                        double width = Math.Abs(start.X - dest.X);
                        double height = Math.Abs(start.Y - dest.Y);

                        switch (shapeType)
                        {
                            case "line":
                                var line = myCanvas.Children.OfType<Line>().LastOrDefault();
                                line.X2 = dest.X;
                                line.Y2 = dest.Y;
                                break;
                            case "rectangle":
                                var rectangle = myCanvas.Children.OfType<Rectangle>().LastOrDefault();
                                rectangle.Width = width;
                                rectangle.Height = height;
                                rectangle.SetValue(Canvas.LeftProperty, origin.X);
                                rectangle.SetValue(Canvas.TopProperty, origin.Y);
                                break;
                            case "ellipse":
                                var ellipse = myCanvas.Children.OfType<Ellipse>().LastOrDefault();
                                ellipse.Width = width;
                                ellipse.Height = height;
                                ellipse.SetValue(Canvas.LeftProperty, origin.X);
                                ellipse.SetValue(Canvas.TopProperty, origin.Y);
                                break;
                            case "polyline":
                                var polyline = myCanvas.Children.OfType<Polyline>().LastOrDefault();
                                polyline.Points.Add(dest);
                                break;
                        }
                    }
                    break;
                case "erase": // 橡皮擦模式
                    var shape = e.OriginalSource as Shape;
                    myCanvas.Cursor = Cursors.Hand;
                    myCanvas.Children.Remove(shape);
                    if (myCanvas.Children.Count == 0) myCanvas.Cursor = Cursors.Arrow;
                    break;
            }
        }

        private void DisplayStatus()
        {
            if (actionType != "draw") statusAction.Content = $"{actionType}";
            else statusAction.Content = $"繪圖模式: {shapeType}";
            statusPoint.Content = $"({Convert.ToInt32(start.X)}, {Convert.ToInt32(start.Y)}) - ({Convert.ToInt32(dest.X)}, {Convert.ToInt32(dest.Y)})";
            int lineCount = myCanvas.Children.OfType<Line>().Count();
            int rectangleCount = myCanvas.Children.OfType<Rectangle>().Count();
            int ellipseCount = myCanvas.Children.OfType<Ellipse>().Count();
            int polylineCount = myCanvas.Children.OfType<Polyline>().Count();
            statusShape.Content = $"Lines: {lineCount}, Rectangles: {rectangleCount}, Ellipses: {ellipseCount}, Polylines: {polylineCount}";
        }

        private void StrokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            strokeColor = strokeColorPicker.SelectedColor.Value;
        }

        private void FillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fillColor = fillColorPicker.SelectedColor.Value;
        }

        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            var targetRadioButton = sender as RadioButton;
            shapeType = targetRadioButton.Tag.ToString();
            actionType = "draw";
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            actionType = "erase";
            DisplayStatus();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            actionType = "clear";
            myCanvas.Children.Clear();
            DisplayStatus();
        }

        private void SaveCanvas_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "儲存畫布",
                Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|All Files (*.*)|*.*",
                DefaultExt = ".png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                int w = Convert.ToInt32(myCanvas.RenderSize.Width);
                int h = Convert.ToInt32(myCanvas.RenderSize.Height);

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(w, h, 96d, 96d, PixelFormats.Pbgra32);

                renderBitmap.Render(myCanvas);

                BitmapEncoder encoder;
                string extension = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                switch (extension)
                {
                    case ".jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    default:
                        encoder = new PngBitmapEncoder();
                        break;
                }

                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (FileStream outStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    encoder.Save(outStream);
                }
            }
        }

        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = (int)strokeThicknessSlider.Value;
        }
    }
}