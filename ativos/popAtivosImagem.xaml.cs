using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace monolith.ativos
{
    /// <summary>
    /// Lógica interna para popAtivosImagem.xaml
    /// </summary>

    public partial class popAtivosImagem : Window
    {
        private string caminhoImagem;
        private Point lastMousePosition;
        private bool isDragging = false;

        public popAtivosImagem(string caminhoCompleto)
        {
            InitializeComponent();
            caminhoImagem = caminhoCompleto;
            FullScreenImage.Source = new BitmapImage(new Uri(caminhoImagem));

            // Inicializa o ScaleTransform e TranslateTransform
            ImageScaleTransform.ScaleX = 1;
            ImageScaleTransform.ScaleY = 1;
            ImageTranslateTransform.X = 0;
            ImageTranslateTransform.Y = 0;
        }

        // Inicia o arraste da imagem ao pressionar o botão esquerdo do mouse
        private void FullScreenImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Marca o início do arraste e registra a posição do mouse
            isDragging = true;
            lastMousePosition = e.GetPosition(this);
            FullScreenImage.CaptureMouse();  // Captura o mouse para o controle da imagem
        }

        // Realiza o movimento da imagem enquanto o mouse se move
        private void FullScreenImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentMousePosition = e.GetPosition(this);
                Vector delta = currentMousePosition - lastMousePosition;

                // Atualiza a posição da imagem com TranslateTransform
                ImageTranslateTransform.X += delta.X;
                ImageTranslateTransform.Y += delta.Y;

                // Atualiza a última posição do mouse
                lastMousePosition = currentMousePosition;
            }
        }

        // Libera a captura do mouse quando o botão esquerdo é solto
        private void FullScreenImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            FullScreenImage.ReleaseMouseCapture();
        }

        // Manipula o evento MouseWheel para fazer zoom na imagem
        private void OnZoomImage(object sender, MouseWheelEventArgs e)
        {
            double scaleFactor = e.Delta > 0 ? 1.1 : 0.9; // Aumenta ou diminui o zoom baseado no movimento do mouse

            // Aplica o zoom na imagem de forma proporcional
            ImageScaleTransform.ScaleX *= scaleFactor;
            ImageScaleTransform.ScaleY *= scaleFactor;

            // Impede que a imagem fique com escala muito pequena ou grande
            if (ImageScaleTransform.ScaleX < 0.1)
            {
                ImageScaleTransform.ScaleX = 0.1;
            }
            if (ImageScaleTransform.ScaleY < 0.1)
            {
                ImageScaleTransform.ScaleY = 0.1;
            }
            if (ImageScaleTransform.ScaleX > 10)
            {
                ImageScaleTransform.ScaleX = 10;
            }
            if (ImageScaleTransform.ScaleY > 10)
            {
                ImageScaleTransform.ScaleY = 10;
            }
        }
    }









}
