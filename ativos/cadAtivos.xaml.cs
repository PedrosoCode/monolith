using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using monolith.parceiroNegocio;
using Npgsql;

namespace monolith.ativos
{
    public partial class cadAtivos : UserControl
    {

        private int? codigoAtivoAtual;
        clsCadAtivo FuncsCadAtivo = new clsCadAtivo();

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
            int? codigo = codigoAtivoAtual; 
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
                codigoAtivoAtual = (int)button.Tag;
                MostrarDetalhesAtivo(codigoAtivoAtual);
            }
        }

        private void MostrarDetalhesAtivo(int? idAtivo)
        {
            // Exibe a aba de detalhes e carrega os dados do ativo
            TabListagem.Visibility = Visibility.Collapsed;
            TabDetalhes.Visibility = Visibility.Visible;
            MainTabControl.SelectedItem = TabDetalhes;

            // Carregue os detalhes do ativo para exibição
            MessageBox.Show($"Detalhes do ativo {idAtivo} carregados.");
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Retorna à aba de listagem
            TabDetalhes.Visibility = Visibility.Collapsed;
            TabListagem.Visibility = Visibility.Visible;
            MainTabControl.SelectedItem = TabListagem;
        }
    }
}
