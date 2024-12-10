using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace monolith.controls
{
    public partial class GenPopVisualizaImagem : UserControl
    {
        private Point origin;
        private Point start;

        public GenPopVisualizaImagem()
        {
            InitializeComponent();
        }

        public void LoadImage(string imagePath)
        {
            if (!System.IO.File.Exists(imagePath))
                throw new ArgumentException("O caminho da imagem não existe.");

            FullScreenImage.Source = new BitmapImage(new Uri(imagePath));
            ResetTransform();
        }

        private void ResetTransform()
        {
            ImageScaleTransform.ScaleX = 1;
            ImageScaleTransform.ScaleY = 1;
            ImageTranslateTransform.X = 0;
            ImageTranslateTransform.Y = 0;
        }

        private void OnZoomImage(object sender, MouseWheelEventArgs e)
        {
            double scaleFactor = e.Delta > 0 ? 1.1 : 0.9;
            var position = e.GetPosition(FullScreenImage);

            ImageScaleTransform.CenterX = position.X;
            ImageScaleTransform.CenterY = position.Y;

            ImageScaleTransform.ScaleX *= scaleFactor;
            ImageScaleTransform.ScaleY *= scaleFactor;
        }

        private void FullScreenImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            if (image != null && !image.IsMouseCaptured)
            {
                start = e.GetPosition(this);
                origin = new Point(ImageTranslateTransform.X, ImageTranslateTransform.Y);
                Mouse.Capture(image);
            }
        }

        private void FullScreenImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            image?.ReleaseMouseCapture();
        }

        private void FullScreenImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!((sender as Image)?.IsMouseCaptured ?? false)) return;

            var delta = Point.Subtract(e.GetPosition(this), start);
            ImageTranslateTransform.X = origin.X + delta.X;
            ImageTranslateTransform.Y = origin.Y + delta.Y;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            ResetTransform();
        }



    }
}
