﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace monolith.parceiroNegocio
{
    class clsCadParceiroNegocio
    {


        public void updateParceiroNegocio(int? codigoParceiroAtual,
                                   string sDocumento,
                                   string sNomeFantasia,
                                   string sRazaoSocial,
                                   string sEmail,
                                   string sContato,
                                   string sTelefone,
                                   string sTipo,
                                   int? lCodigoPais,
                                   int? iCodigoEstado,
                                   int? lCodigocidade,
                                   string sCep,
                                   string sLogradouro,
                                   string sNumero,
                                   string sComplemento,
                                   string sBairro)
        {
            try
            {
                // Validação de CPF/CNPJ
                bool is_cnpj = (sDocumento.Length == 14);
                if (sDocumento.Length != 11 && sDocumento.Length != 14)
                {
                    throw new ArgumentException("O número fornecido não é um CPF ou CNPJ válido.");
                }

                // Validação de CEP
                if (sCep.Length != 8)
                {
                    throw new ArgumentException("CEP inválido.");
                }

                // Prepare os parâmetros para o comando
                var parameters = new Dictionary<string, object>
                    {
                        { "@p_codigo"           ,  codigoParceiroAtual          },
                        { "@p_codigo_empresa"   ,  Globals.GlobalCodigoEmpresa  },
                        { "@p_documento"        ,  sDocumento                   },
                        { "@p_nome_fantasia"    ,  sNomeFantasia                },
                        { "@p_razao_social"     ,  sRazaoSocial                 },
                        { "@p_email"            ,  sEmail                       },
                        { "@p_contato"          ,  sContato                     },
                        { "@p_telefone"         ,  sTelefone                    },
                        { "@p_tipo"             ,  sTipo                        },
                        { "@p_codigo_pais"      ,  lCodigoPais                  },
                        { "@p_codigo_estado"    ,  iCodigoEstado                },
                        { "@p_codigo_cidade"    ,  lCodigocidade                },
                        { "@p_tipo_documento"   ,  is_cnpj                      },
                        { "@p_cep"              ,  sCep                         },
                        { "@p_logradouro"       ,  sLogradouro                  },
                        { "@p_numero"           ,  sNumero                      },
                        { "@p_complemento"      ,  sComplemento                 },
                        { "@p_bairro"           ,  sBairro                      }  
                    };

                // Chama o DatabaseHelper para executar o comando
                var dbHelper = new DatabaseHelper();
                dbHelper.ExecuteCommand(
                    "CALL sp_atualizar_parceiro_negocio(@p_codigo, "            +
                                                       "@p_codigo_empresa, "    +
                                                       "@p_documento, "         +
                                                       "@p_nome_fantasia, "     +
                                                       "@p_razao_social, "      +
                                                       "@p_email, "             +
                                                       "@p_contato, "           +
                                                       "@p_telefone, "          +
                                                       "@p_tipo, "              +
                                                       "@p_codigo_pais, "       +
                                                       "@p_codigo_estado, "     +
                                                       "@p_codigo_cidade, "     +
                                                       "@p_tipo_documento, "    +
                                                       "@p_cep, "               +
                                                       "@p_logradouro, "        +
                                                       "@p_numero, "            +
                                                       "@p_complemento, "       +
                                                       "@p_bairro"              +
                                                       ")",
                    parameters
                );

                MessageBox.Show("Dados atualizados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar os dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        public void excluirParceiroNegocio(int? codigoParceiroAtual)


        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using (var command = new NpgsqlCommand("CALL sp_delete_cadastro_parceiro_negocio(@p_codigo_empresa,"    +
                                                                                                     "@p_codigo_parceiro" +
                                                                                                     ")"
                                                                                                     , conn))
                    {

                        command.Parameters.AddWithValue("@p_codigo_empresa", Globals.GlobalCodigoEmpresa);
                        command.Parameters.AddWithValue("@p_codigo_parceiro", codigoParceiroAtual);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Parceiro deletado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar os dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void insertParceiroNegocio(string sDocumento,
                                          string sCep,
                                          string sTelefone,
                                          string sEmail,
                                          string sTipo,
                                          string sNomeFantasia,
                                          string sRazaoSocial,
                                          string sLogradouro,
                                          string sNumero,
                                          string sComplemento,
                                          string sBairro,
                                          string sContato,
                                          int? lCodigoPais,
                                          int? lCodigoCidade,
                                          int? iCodigoEstado
                                          )


        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    using (var command = new NpgsqlCommand("CALL sp_insert_cadastro_basico_parceiro_negocio(@p_codigo_empresa,"    +
                                                                                                            "@p_documento,"        +
                                                                                                            "@p_cep,"              +
                                                                                                            "@p_telefone,"         +
                                                                                                            "@p_email,"            +
                                                                                                            "@p_data_cadastro,"    +
                                                                                                            "@p_tipo,"             +
                                                                                                            "@p_nome_fantasia,"    +
                                                                                                            "@p_razao_social,"     +
                                                                                                            "@p_logradouro,"       +
                                                                                                            "@p_numero,"           +
                                                                                                            "@p_complemento,"      +
                                                                                                            "@p_bairro,"           +
                                                                                                            "@p_contato,"          +
                                                                                                            "@p_codigo_pais,"      +
                                                                                                            "@p_codigo_cidade,"    +
                                                                                                            "@p_codigo_estado,"    +
                                                                                                            "@p_tipo_documento" +
                                                                                                            ")"
                                                                                                            , conn))
                    {

                        bool is_cnpj = false;
                        if (sDocumento.Length == 14)
                        {
                            is_cnpj = true;
                        }
                        else if (sDocumento.Length == 11)
                        {
                            is_cnpj = false;
                        }
                        else
                        {
                            throw new ArgumentException("O número fornecido não é um CPF ou CNPJ válido.");
                        }


                        if (sCep.Length != 8)
                        {
                            throw new ArgumentException("CEP inválido.");
                        }

                        command.Parameters.AddWithValue("@p_codigo_empresa", Globals.GlobalCodigoEmpresa);
                        command.Parameters.AddWithValue("@p_documento", sDocumento);
                        command.Parameters.AddWithValue("@p_cep", sCep);
                        command.Parameters.AddWithValue("@p_telefone", sTelefone);
                        command.Parameters.AddWithValue("@p_email", sEmail);
                        command.Parameters.AddWithValue("@p_data_cadastro", DateTime.Now);
                        command.Parameters.AddWithValue("@p_tipo", sTipo);
                        command.Parameters.AddWithValue("@p_nome_fantasia", sNomeFantasia);
                        command.Parameters.AddWithValue("@p_razao_social", sRazaoSocial);
                        command.Parameters.AddWithValue("@p_logradouro", sLogradouro);
                        command.Parameters.AddWithValue("@p_numero", sNumero);
                        command.Parameters.AddWithValue("@p_complemento", sComplemento);
                        command.Parameters.AddWithValue("@p_bairro", sBairro);
                        command.Parameters.AddWithValue("@p_contato", sContato);
                        command.Parameters.AddWithValue("@p_codigo_pais", lCodigoPais ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_codigo_cidade", lCodigoCidade ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_codigo_estado", iCodigoEstado ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_tipo_documento", is_cnpj);



                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dados inseridos com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inserir os dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


            private readonly string connectionString;

            public clsCadParceiroNegocio()
            {
                connectionString = ConfigurationManager.ConnectionStrings["ConnPostgres"].ConnectionString;
            }

            public Dictionary<string, object> ObterDadosParceiro(int? codigoParceiroAtual, int? codigoEmpresa)
            {
                var dados = new Dictionary<string, object>();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM fn_cad_parceiro_negocio_dados(@p_codigo, @p_codigo_empresa)", conn))
                    {
                        cmd.Parameters.AddWithValue("@p_codigo", codigoParceiroAtual ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_codigo_empresa", codigoEmpresa ?? (object)DBNull.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    dados[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                            }
                        }
                    }
                }

                return dados;
            }



    }
}
