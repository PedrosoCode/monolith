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

namespace monolith.ativos
{
    public partial class popInputboxTituloFoto : Window
    {
        public string TituloFoto { get; private set; } 

        public popInputboxTituloFoto(string prompt)
        {
            InitializeComponent();
            this.Title = prompt;  // Define o título da janela, se necessário
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            TituloFoto = txtInput.Text.Trim();  // Armazena a entrada do usuário
            this.DialogResult = true;  // Fecha a janela e indica sucesso
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;  // Fecha a janela e indica cancelamento
            this.Close();
        }
    }
}

