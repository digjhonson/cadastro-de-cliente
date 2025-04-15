using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace cadastrodeclientes
{
    public partial class frmCadastrodeClientes : Form
    {
        //Conexão com o banco de dados MySQL
        MySqlConnection Conexao;
        string data_source = "datasource=localhost; username=root; password=; database=db_cadastro";


        private int? codigo_client = null;

        public frmCadastrodeClientes()
        {
            InitializeComponent();

            //Configuração inicial do ListView para a exibição  dos dados dos clientes
            lstCliente.View = View.Details;             //Define a visualização como "Detalhes"
            lstCliente.LabelEdit = true;                //Permite editar os títulos das colunas
            lstCliente.AllowColumnReorder = true;       //Permite reordenar as colunas
            lstCliente.FullRowSelect = true;            //Seleciona a linha inteira ao clicar
            lstCliente.GridLines = true;                //Exibe as linhas de grade no ListView


            //Definindo as colunas do ListView
            lstCliente.Columns.Add("Codigo", 100, HorizontalAlignment.Left); //Coluna de código
            lstCliente.Columns.Add("Nome Completo", 200, HorizontalAlignment.Left); //Coluna Nome Completo
            lstCliente.Columns.Add("Nome Social", 200, HorizontalAlignment.Left); //Coluna Nome Social
            lstCliente.Columns.Add("E-mail", 200, HorizontalAlignment.Left); //Coluna de E-mail
            lstCliente.Columns.Add("CPF", 200, HorizontalAlignment.Left); //Coluna de CPF

            //Carrega os dados do cliente na interface
            carregar_clientes();

        }

        private void carregar_clientes_com_query(string query)
        {
            try
            {
                //Cria a conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                //Executa a consulta SQL fornecida
                MySqlCommand cmd = new MySqlCommand(query, Conexao);


                //Se a consulta contém o parâmetro @q, adiciona o valor da caixa de pesquisa
                if (query.Contains("@q"))
                {
                    cmd.Parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");
                }

                //Executa o comando e obtém os resultados 
                MySqlDataReader reader = cmd.ExecuteReader();

                //Limpa os itens existentes no ListView antes de adicionar novos
                lstCliente.Items.Clear();


                //Preenche o ListView com os dados dos clientes
                while (reader.Read())
                {
                    //Cria uma linha para cada cliente com os dados retornados da consulta
                    string[] row =
                    {
                        Convert.ToString(reader.GetInt32(0)), //Codigo
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4)
                    };


                    //Adiciona a linha ao ListView         
                    lstCliente.Items.Add(new ListViewItem(row));

                }
            }


            catch (MySqlException ex)
            {
                //Tratar erros relacionados ao MySQL
                MessageBox.Show("Erro " + ex.Number + "ocorreu: " + ex.Message,
                      "Erro",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);

            }

            catch (Exception ex)
            {

                //Trata outros tipos de erro
                MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);


            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();


                }


            }

        }

        //Método para carregar todos os clientes no ListView (usando uma consulta sem parâmetros)
        private void carregar_clientes()
        {
            string query = "SELECT * FROM dadosdecliente ORDER BY codigo DESC";
            carregar_clientes_com_query(query);
        }




            //Validação Regex
            private bool isValidEmail(string email)
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                Regex regex = new Regex(pattern);
                return regex.IsMatch(email);
            }


            //Função para validar se o CPF tem exatamente 11 dígitos numéricos
            private bool isValidCPFLength(string cpf)
            {
                //Remover quaisquer caracteres não numéricos (como pontos e traços)
                cpf = cpf.Replace(".", "").Replace("-", "");

                //Verificar se o CPF tem exatamente 11 caracteres numéricos
                if (cpf.Length != 11 || !cpf.All(char.IsDigit))
                {
                    return false;
                }
                return true;
            }




            private void btnSalvar_Click(object sender, EventArgs e)
            {
                try
                {
                    //Validação de campos obrigátórios
                    if (string.IsNullOrEmpty(txtNomeCompleto.Text.Trim()) ||
                        string.IsNullOrEmpty(txtEmail.Text.Trim()) ||
                        string.IsNullOrEmpty(txtCPF.Text.Trim()))

                    {
                        MessageBox.Show("Todos os campos devem ser preenchidos.",
                                        "Validação",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return; // Impede o prosseguimento se algum campo estiver vazio
                    }


                    //Validação do e-mail
                    string email = txtEmail.Text.Trim();
                    if (!isValidEmail(email))
                    {
                        MessageBox.Show("E-mail inválido. Certifique-se de que o email está no formato correto.",
                                        "Validação",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return; // Impede o prosseguimento se o e-mail for inválido

                    }



               


                    //Validação do CPF
                    string cpf = txtCPF.Text.Trim();
                    if (!isValidCPFLength(cpf))
                    {
                        MessageBox.Show("CPF inválido. Certifique-se de que o CPF tenha 11 dígitos numéricos",
                                        "Validação",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return; //Impede o prosseguimento se o CPF for inválido
                    }


                    //Cria a conexão com o banco de dados
                    Conexao = new MySqlConnection(data_source);
                    Conexao.Open();

                    //Teste de abertura de banco
                    //MessageBox.Show("Conexão aberta com sucesso");

                    //Comando SQL para inserir um novo cliente no banco de dados
                    MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = Conexao
                    };

                    cmd.Prepare();


                if (codigo_client== null)

                {

                    //insert CREATE

                    cmd.CommandText = "INSERT INTO dadosdecliente(nomecompleto, nomesocial, email, cpf) " +

                      "VALUES ( @nomecompleto, @nomesocial, @email, @cpf)";


                    //Adiciona os parâmetros com os dados do formulário

                    cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());

                    cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());

                    cmd.Parameters.AddWithValue("@email", email);

                    cmd.Parameters.AddWithValue("@cpf", cpf);

                    //Executa o comando de inserção no banco

                    cmd.ExecuteNonQuery();


                    //Mensagem de sucesso

                    MessageBox.Show("Contato inserido com Sucesso: ",

                                    "Sucesso",

                                    MessageBoxButtons.OK,

                                    MessageBoxIcon.Information);

                }

                else
                {
                    //update
                    cmd.CommandText = $"upadate `dadosdeclient` SET " +
                        $"nomecompleto = @nomecompleto," +
                        $"nomesocial = @nomesocial," +
                     $"cpf = @cpf," +
                        $"WHERE codigo = @codigo";

                    cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@cpf", cpf);
                }



                cmd.CommandText = "INSERT INTO dadosdecliente(nomecompleto, nomesocial, email, cpf) " +
                        "VALUES ( @nomecompleto, @nomesocial, @email, @cpf)";


                    //Adiciona os parâmetros com os dados do formulário
                    cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@cpf", cpf);

                    //Executa o comando de inserção no banco
                    cmd.ExecuteNonQuery();

                MessageBox.Show($"Os dados com o codigo{codigo_client} foram alterados com Sucesso!",
                   "Sucesso",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);



                    //Mensagem de sucesso
                    MessageBox.Show("Contato inserido com Sucesso: ",
                                    "Sucesso",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);


                //Limpa os campos após o sucesso
                txtNomeCompleto.Text = String.Empty;
                txtNomeSocial.Text = " ";
                txtEmail.Text = " ";
                txtCPF.Text = " ";

                //Regarregar os clintes na ListView 
                carregar_clientes();

                //Muda para a aba de pesquisa
                tabControl1.SelectedIndex = 1;

                }

                catch (MySqlException ex)
                {
                    //Tratar erros relacionados ao MySQL
                    MessageBox.Show("Erro " + ex.Number + "ocorreu: " + ex.Message,
                          "Erro",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error);

                }

                catch (Exception ex)
                {

                    //Trata outros tipos de erro
                    MessageBox.Show("Ocorreu: " + ex.Message,
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);


                }
                finally
                {
                    //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                    if (Conexao != null && Conexao.State == ConnectionState.Open)
                    {
                        Conexao.Close();

                        //Teste de fechamento de banco
                        //MessageBox.Show("Conexão fechada com sucesso");
                    }
                }
            }

            private void btnPesquisar_Click(object sender, EventArgs e)
            {
            string query = "SELECT * FROM dadosdecliente WHERE nomecompleto LIKE @q OR nomesocial LIKE @q ORDER BY codigo DESC";
            carregar_clientes_com_query(query);
            }

        private void lstCliente_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView.SelectedListViewItemCollection clientedaselecao = lstCliente.SelectedItems;
            foreach(ListViewItem item in clientedaselecao)

            {
                codigo_client = Convert.ToInt32(item.SubItems[0].Text);

                MessageBox.Show("Codigo do Cliente: " + codigo_client.ToString(),
                    "Codigo Selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);




                txtNomeCompleto.Text = item.SubItems[1].Text;
                txtNomeSocial.Text = item.SubItems[2].Text;
                txtEmail.Text = item.SubItems[3].Text;
                txtCPF.Text = item.SubItems[4].Text;
            }

            //muda para a aba de dados  do cliente
            tabControl1.SelectedIndex = 0;
        }

        private void btnNovoCliente_Click(object sender, EventArgs e)
        {
            codigo_client = null;
             
                txtNomeCompleto.Text = String.Empty;
            txtNomeSocial.Text = " ";
            txtEmail.Text = " ";
            txtCPF.Text = " ";

            txtNomeCompleto.Focus();
        }
    }
    }
