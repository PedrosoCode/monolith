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
using System.Windows.Input;
using System.Windows.Media.Imaging;
using monolith.controls;
using Microsoft.Win32;
using System.Windows.Documents;
using System.Windows.Threading;

namespace monolith.ativos
{
    public partial class cadAtivos : UserControl
    {

        #region +++===VARIAVÉIS===+++
        
        private int? iCodigoAtivoAtual;
        clsCadAtivo FuncsCadAtivo = new clsCadAtivo();
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
           try
           {
               var button = sender as Button;
               if (button != null && button.Tag != null)
               {
                   iCodigoAtivoAtual = (int)button.Tag;
       
                   MostrarDetalhesAtivo(iCodigoAtivoAtual);
                   CarregarDadosAtivo(iCodigoAtivoAtual);
               }
           }
           catch (Exception ex)
           {
               MessageBox.Show($"Erro ao carregar os detalhes do Ativo: {ex.Message}");
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
                MessageBox.Show($"Erro ao carregar os detalhes do ativo: {ex.Message}");
            }
        }

        #region +++===FOTOS DO ATIVO===+++



        private async void MostrarDetalhesAtivo(int? idAtivo)
        {
            try
            {
                TabImagem.Visibility = Visibility.Visible;
                MainTabControl.SelectedItem = TabDetalhes;

                // Obter os caminhos das imagens do banco de dados
                var caminhos = await FuncsCadAtivo.ObterCaminhosImagensAsync(idAtivo);

                // Limpar e carregar as imagens no carrossel
                FotosCarrossel.Items.Clear();

                foreach (var caminho in caminhos)
                {
                    if (File.Exists(caminho))
                    {
                        var image = new Image
                        {
                            Source = new BitmapImage(new Uri(caminho)),
                            Width = 100,
                            Height = 100,
                            Tag = caminho // Armazena o caminho diretamente no controle
                        };

                        FotosCarrossel.Items.Add(image);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar detalhes do ativo: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Função para carregar a imagem a partir de um Stream
        private BitmapImage LoadBitmapImageFromStream(string caminho)
        {
            var bitmapImage = new BitmapImage();
            using (var stream = new FileStream(caminho, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) // Abrir com FileShare
            {
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream; // Usando o StreamSource diretamente
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Congelar a imagem
            }
            return bitmapImage;
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

            try
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
            catch (Exception)
            {

                throw;
            }


        }

        private void btnInserirImagem_Click(object sender, RoutedEventArgs e)
        {
            try { 
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
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inserir imagens: {ex.Message}");
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
            try { 

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
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar dados: {ex.Message}");
            }
        }

        private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var caminhoCompleto = (string)((Button)sender).Tag;

                if (!string.IsNullOrEmpty(caminhoCompleto))
                {
                    // Remover a imagem do carrossel
                    var itemParaRemover = FotosCarrossel.Items.Cast<Image>()
                        .FirstOrDefault(img => ((BitmapImage)img.Source).UriSource.LocalPath == caminhoCompleto);

                    if (itemParaRemover != null)
                    {
                        FotosCarrossel.Items.Remove(itemParaRemover);
                    }

                    // Excluir o arquivo fisicamente
                    if (File.Exists(caminhoCompleto))
                    {
                        File.Delete(caminhoCompleto);
                    }

                    // Atualizar banco de dados (se necessário)
                    await FuncsCadAtivo.ExcluirImagemAsync(iCodigoAtivoAtual, null, caminhoCompleto);

                    MessageBox.Show("Imagem excluída com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir a imagem: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExcluirListagem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                int? iCodigoAtivo = button.Tag as int?;

                FuncsCadAtivo.ExcluirAtivo(iCodigoAtivo);
            }

            filtrar();
        }









    }
}
