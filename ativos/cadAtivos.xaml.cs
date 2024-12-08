using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using monolith.parceiroNegocio;
using Npgsql;
using Monolith.Ativos;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace monolith.ativos
{
    public partial class cadAtivos : UserControl
    {

        private int? iCodigoAtivoAtual;
        clsCadAtivo FuncsCadAtivo = new clsCadAtivo();
        public ObservableCollection<FotoAtivo> cFotosAtivo { get; set; }
        private int iIndexImagemAtivoAtual = 0;


        public cadAtivos()
        {
            InitializeComponent();
            CarregarComboParceiroListagem();
            CarregarComboFabricanteListagem();

        }

        private void CarregarComboFabricanteListagem()
        {
            var fabricantes = FuncsCadAtivo.CarregarComboFabricanteListagem();
            cboFabricanteListagem.ItemsSource = fabricantes;
            cboFabricanteListagem.DisplayMemberPath = "Value";
            cboFabricanteListagem.SelectedValuePath = "Key";
        }

        private void CarregarComboParceiroListagem()
        {
            var parceiros = FuncsCadAtivo.CarregarComboParceiroListagem();
            cboParceiroListagem.ItemsSource = parceiros;
            cboParceiroListagem.DisplayMemberPath = "Value";
            cboParceiroListagem.SelectedValuePath = "Key";
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {

            filtrar();

        }

        private void filtrar()
        {
            int codigoEmpresa = Globals.GlobalCodigoEmpresa;
            int? codigo = iCodigoAtivoAtual; 
            int? codigoCliente = cboParceiroListagem.SelectedValue as int?;
            string numeroSerie = string.IsNullOrWhiteSpace(txtNumeroSerie.Text) ? null : txtNumeroSerie.Text.Trim();
            int? codigoFabricante = cboFabricanteListagem.SelectedValue as int?;
            string modelo = string.IsNullOrWhiteSpace(txtModelo.Text) ? null : txtModelo.Text.Trim();
            string observacao = string.IsNullOrWhiteSpace(txtObservacao.Text) ? null : txtObservacao.Text.Trim();

            FuncsCadAtivo.CarregarAtivos(dgAtivos, 
                           codigo, 
                           codigoCliente, 
                           numeroSerie, 
                           codigoFabricante, 
                           modelo, 
                           observacao);
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                iCodigoAtivoAtual = (int)button.Tag;
                MostrarDetalhesAtivo(iCodigoAtivoAtual);
            }
        }

        private void MostrarDetalhesAtivo(int? idAtivo)
        {
            TabListagem.Visibility = Visibility.Collapsed;
            TabDetalhes.Visibility = Visibility.Visible;
            MainTabControl.SelectedItem = TabDetalhes;

            var fotos = FuncsCadAtivo.ObterCaminhosImagens(idAtivo); // Agora retornando FotoAtivo

            cFotosAtivo = new ObservableCollection<FotoAtivo>(fotos);

            FotosCarrossel.ItemsSource = cFotosAtivo;
            FotosCarrossel.Items.Refresh();

            MessageBox.Show($"Detalhes do ativo {idAtivo} carregados.");
        }

        private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Erro ao carregar imagem: {e.ErrorException?.Message}");
        }


        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Retorna à aba de listagem
            TabDetalhes.Visibility = Visibility.Collapsed;
            TabListagem.Visibility = Visibility.Visible;
            TabImagem.Visibility = Visibility.Visible;  
            MainTabControl.SelectedItem = TabListagem;
        }


        // Evento para abrir a imagem em alta resolução
        // Evento para abrir a imagem em alta resolução
        // Declara a variável 'popUpImagem' no escopo da classe
        private popAtivosImagem popUpImagem;

        private void OnImageClick(object sender, MouseButtonEventArgs e)
        {
            var imageControl = sender as Image;
            if (imageControl != null)
            {
                string caminhoCompleto = imageControl.Source.ToString().Replace("file:///", "");
                if (File.Exists(caminhoCompleto))
                {
                    // Cria e exibe a janela de imagem em alta resolução
                    popUpImagem = new popAtivosImagem(caminhoCompleto);
                    popUpImagem.ShowDialog(); // Isso vai abrir a janela como modal
                }
                else
                {
                    MessageBox.Show("Arquivo não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }





    }
}
