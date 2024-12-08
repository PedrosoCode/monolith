using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monolith.Ativos
{
    public class FotoAtivo
    {
        public string CaminhoCompleto { get; set; }
        public string NomeImagem { get; set; }

        public FotoAtivo(string caminhoCompleto, string nomeImagem)
        {
            CaminhoCompleto = caminhoCompleto;
            NomeImagem = nomeImagem;
        }
    }
}

