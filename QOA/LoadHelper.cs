using System.Windows.Controls;
using System.Windows.Documents;

public static class LoadHelper
{
    /// <summary>
    /// Preenche um controle com o valor correspondente do dicionário de dados.
    /// </summary>
    /// <param name="controle">O controle a ser preenchido (TextBox, ComboBox, etc.).</param>
    /// <param name="coluna">O nome da chave no dicionário de dados.</param>
    /// <param name="dados">Dicionário contendo os dados do banco.</param>
    public static void PreencherControle(dynamic controle, 
                                         string coluna, 
                                         Dictionary<string, object> dados)
    {
        if (dados.ContainsKey(coluna))
        {
            var valor = dados[coluna];

            switch (controle)
            {
                case TextBox textBox:
                    textBox.Text = valor?.ToString() ?? string.Empty;
                    break;

                case ComboBox comboBox:
                    comboBox.SelectedValue = valor;
                    break;

                case CheckBox checkBox:
                    checkBox.IsChecked = valor != null && bool.TryParse(valor.ToString(), out var isChecked) && isChecked;
                    break;

                case Label label:
                    label.Content = valor?.ToString() ?? string.Empty;
                    break;

                case RichTextBox richTextBox:
                    if (valor != null)
                    {
                        var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                        textRange.Text = valor.ToString();
                    }
                    else
                    {
                        var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                        textRange.Text = string.Empty; // Limpa o conteúdo se o valor for nulo
                    }
                    break;


                // Adicione outros controles conforme necessário.
                default:
                    throw new ArgumentException($"Tipo de controle {controle.GetType().Name} não suportado.");
            }
        }
    }
}
