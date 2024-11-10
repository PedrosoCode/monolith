using System;
using System.Windows;
using System.Windows.Controls;
using monolith.ativos;
using monolith.parceiroNegocio;

namespace monolith
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
        }

        // Evento de clique para abrir o UserControl cadAtivos em uma nova aba
        private void MenuItem_AbrirCadAtivos_Click(object sender, RoutedEventArgs e)
        {
            // Identifica o MenuItem que foi clicado
            MenuItem menuItem = sender as MenuItem;
            string abaSolicitada = menuItem?.Name;

            // Verifica se a aba já está aberta
            foreach (TabItem tab in MainTabControl.Items)
            {
                if ((abaSolicitada == "MenuCadAtivos" && tab.Header.ToString() == "cadAtivos") ||
                    (abaSolicitada == "MenuCadParceiros" && tab.Header.ToString() == "cadParceiroNegocio"))
                {
                    MainTabControl.SelectedItem = tab;
                    return;
                }
            }

            // Cria uma nova aba com o UserControl apropriado
            TabItem newTab = new TabItem();

            if (abaSolicitada == "MenuCadAtivos")
            {
                newTab.Header = "cadAtivos";
                newTab.Content = new cadAtivos();
            }
            else if (abaSolicitada == "MenuCadParceiros")
            {
                newTab.Header = "cadParceiroNegocio";
                newTab.Content = new cadParceiroNegocio();
            }

            // Adiciona a nova aba e a seleciona
            MainTabControl.Items.Add(newTab);
            MainTabControl.SelectedItem = newTab;
        }

    }
}
