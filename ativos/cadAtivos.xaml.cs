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
using monolith.controls;
using Microsoft.Win32;
using System.Windows.Documents;

namespace monolith.ativos
{
    public partial class cadAtivos : UserControl
    {

        #region +++===VARIAVÉIS===+++
        
        private int? iCodigoAtivoAtual;
        clsCadAtivo FuncsCadAtivo = new clsCadAtivo();
        public ObservableCollection<FotoAtivo> cFotosAtivo { get; set; }
        private GenPopVisualizaImagem genImageViewer;

        #endregion

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
            int? codigoCliente = cboParceiroListagem.SelectedValue as int?;
            string numeroSerie = string.IsNullOrWhiteSpace(txtNumeroSerie.Text) ? null : txtNumeroSerie.Text.Trim();
            int? codigoFabricante = cboFabricanteListagem.SelectedValue as int?;
            string modelo = string.IsNullOrWhiteSpace(txtModelo.Text) ? null : txtModelo.Text.Trim();
            string observacao = string.IsNullOrWhiteSpace(txtObservacao.Text) ? null : txtObservacao.Text.Trim();

            FuncsCadAtivo.CarregarAtivos(dgAtivos,
                                         codigoCliente, 
                                         numeroSerie, 
                                         codigoFabricante, 
                                         modelo, 
                                         observacao
                                         );
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                iCodigoAtivoAtual = (int)button.Tag;

                MostrarDetalhesAtivo(iCodigoAtivoAtual);
                CarregarDadosAtivo(iCodigoAtivoAtual);

            }
        }

        private void CarregarDadosAtivo(int? iCodigoAtivoAtual
                                        )
        {
            try
            {
                var dados = FuncsCadAtivo.LoadDadosAtivo(iCodigoAtivoAtual
                                                         );

                if (dados.Count > 0)
                {
                    LoadHelper.PreencherControle(txtNserieDados         , "numero_serie"    , dados);
                    LoadHelper.PreencherControle(txtModeloDados         , "modelo"          , dados);
                    LoadHelper.PreencherControle(txtAliasDados          , "alias"           , dados);
                    LoadHelper.PreencherControle(txtNserieDados         , "numero_serie"    , dados);
                    LoadHelper.PreencherControle(txtObservacaoDados     , "observacao"      , dados);
                    LoadHelper.PreencherControle(ltxtInformacaoDados    , "descricao"       , dados);

                    var parceirosNegocio = FuncsCadAtivo.CarregarComboParceiroListagem();
                    cboParceiroNegocioDados.ItemsSource = parceirosNegocio;
                    cboParceiroNegocioDados.DisplayMemberPath = "Value";
                    cboParceiroNegocioDados.SelectedValuePath = "Key";
                    LoadHelper.PreencherControle(cboParceiroNegocioDados, "codigo_parceiro_negocio", dados);

                    var fabricantes = FuncsCadAtivo.CarregarComboFabricanteListagem();
                    cbofabricanteDados.ItemsSource = fabricantes;
                    cbofabricanteDados.DisplayMemberPath = "Value";
                    cbofabricanteDados.SelectedValuePath = "Key";
                    LoadHelper.PreencherControle(cbofabricanteDados, "codigo_fabricante", dados);

                    MainTabControl.SelectedItem = TabDetalhes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os detalhes do parceiro: {ex.Message}");
            }
        }

        #region +++===FOTOS DO ATIVO===+++



        private void MostrarDetalhesAtivo(int? idAtivo)
        {
            //TabListagem.Visibility = Visibility.Collapsed;
            //TabDetalhes.Visibility = Visibility.Visible;
            TabImagem.Visibility = Visibility.Visible;

            MainTabControl.SelectedItem = TabDetalhes;

            var fotos = FuncsCadAtivo.ObterCaminhosImagens(idAtivo);

            cFotosAtivo = new ObservableCollection<FotoAtivo>(fotos);

            FotosCarrossel.ItemsSource = cFotosAtivo;
            FotosCarrossel.Items.Refresh();
        }

        private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Erro ao carregar imagem: {e.ErrorException?.Message}");
        }

        private void OnImageClick(object sender, MouseButtonEventArgs e)
        {
            var imageControl = sender as Image;
            if (imageControl != null)
            {
                string caminhoCompleto = imageControl.Source.ToString().Replace("file:///", "");
                if (File.Exists(caminhoCompleto))
                {
                    if (genImageViewer == null)
                    {
                        genImageViewer = new GenPopVisualizaImagem();
                        genImageViewer.HorizontalAlignment = HorizontalAlignment.Stretch;
                        genImageViewer.VerticalAlignment = VerticalAlignment.Stretch;
                        genImageViewer.Margin = new Thickness(10);

                        GridFotos.Children.Add(genImageViewer);
                    }

                    genImageViewer.LoadImage(caminhoCompleto);

                    genImageViewer.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Arquivo não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        #endregion

        //private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        //{
        //    // Retorna à aba de listagem
        //    TabDetalhes.Visibility = Visibility.Collapsed;
        //    TabListagem.Visibility = Visibility.Visible;
        //    TabImagem.Visibility = Visibility.Visible;  
        //    MainTabControl.SelectedItem = TabListagem;
        //}

        private void BtnNovo_Click(object sender, RoutedEventArgs e)
        {
            
            TabImagem.Visibility = Visibility.Visible;
            MainTabControl.SelectedItem = TabDetalhes;

            iCodigoAtivoAtual = null;
            txtNserieDados.Text = "";
            txtModeloDados.Text = "";
            txtAliasDados.Text = "";
            txtObservacaoDados.Text = "";
            cboParceiroNegocioDados.SelectedValue = null;
            cbofabricanteDados.SelectedValue = null;
            ltxtInformacaoDados.Document = new FlowDocument();



        }

        private void btnInserirImagem_Click(object sender, RoutedEventArgs e)
        {
            int? iCodigoAtivo = iCodigoAtivoAtual;
            string sArquivo = "";
            string sCaminhoUpload = "";
            string sNomeFoto = "";

            if (iCodigoAtivo != null) {

                popInputboxTituloFoto inputBox = new popInputboxTituloFoto("Digite o título da foto");
                bool? result = inputBox.ShowDialog();

                if (result == true)  
                {
                    sNomeFoto = inputBox.TituloFoto; 
                }
                else
                {
                    MessageBox.Show("É necessário um título para a imagem.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; 
                }

                sArquivo = SelecionarImagem();
                sCaminhoUpload = FuncsCadAtivo.ObterCaminhoUpload();
                FuncsCadAtivo.UploadImagem(iCodigoAtivo,
                                           sArquivo,
                                           sCaminhoUpload,
                                           sNomeFoto
                                           );

                MostrarDetalhesAtivo(iCodigoAtivoAtual);

            }
            else
            {
                MessageBox.Show("Primeiro crie ou selecione um ativo antes de definir a imagem.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }




        }

        


        private string SelecionarImagem()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos de Imagem (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Selecione uma imagem"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        private void Btnsalvar_Click(object sender, RoutedEventArgs e)
        {

            string sNumeroSerie = txtNserieDados.Text.Trim();
            string sModelo = txtModeloDados.Text.Trim();
            string sAlias = txtAliasDados.Text.Trim();
            string sObservacao = txtObservacaoDados.Text.Trim();
            int? iCodigoParceiroNegocio = cboParceiroNegocioDados.SelectedValue != null ? Convert.ToInt32(cboParceiroNegocioDados.SelectedValue) : (int?)null;
            int? iCodigoFabricante = cbofabricanteDados.SelectedValue != null ? Convert.ToInt32(cbofabricanteDados.SelectedValue) : (int?)null;
            string sTexto = new TextRange(ltxtInformacaoDados.Document.ContentStart, ltxtInformacaoDados.Document.ContentEnd).Text.Trim();


            if (iCodigoAtivoAtual == null)
            {

                FuncsCadAtivo.inserirAtivo(iCodigoAtivoAtual,
                                           sNumeroSerie, 
                                           sModelo,  
                                           sAlias,  
                                           sObservacao,
                                           iCodigoParceiroNegocio,
                                           iCodigoFabricante,
                                           sTexto
                                           );

                filtrar();

            }
            else
            {

                FuncsCadAtivo.atualizarAtivo(iCodigoAtivoAtual,
                                             sNumeroSerie,
                                             sModelo,
                                             sAlias,
                                             sObservacao,
                                             iCodigoParceiroNegocio,
                                             iCodigoFabricante,
                                             sTexto
                                             );

                filtrar();

            }
        }

    }
}
