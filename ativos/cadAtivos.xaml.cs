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





    }
}
