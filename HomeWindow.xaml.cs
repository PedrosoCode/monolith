using System;
using System.Windows;
using System.Windows.Controls;
using monolith.ativos; // Namespace do UserControl

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
            // Verifica se a aba já está aberta
            foreach (TabItem tab in MainTabControl.Items)
            {
                if (tab.Header.ToString() == "cadAtivos")
                {
                    MainTabControl.SelectedItem = tab;
                    return;
                }
            }

            // Cria uma nova aba com o UserControl
            TabItem newTab = new TabItem
            {
                Header = "cadAtivos",
                Content = new cadAtivos()
            };

            MainTabControl.Items.Add(newTab);
            MainTabControl.SelectedItem = newTab;
        }
    }
}
