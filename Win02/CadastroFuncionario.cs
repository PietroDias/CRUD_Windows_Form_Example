using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win02.Banco;
using Win02.Modelo;

namespace Win02
{
    public partial class CadastroFuncionario : Form
    {
        private TelaPrincipal telaPrincipal;
        private Funcionario func;

        public CadastroFuncionario(TelaPrincipal tela)
        {
            telaPrincipal = tela;
            InitializeComponent();
        }

        public CadastroFuncionario(TelaPrincipal tela , int id)
        {
            telaPrincipal = tela;
            InitializeComponent();

            func = FuncionarioDataAccess.CarregarFuncionario(id);

            FuncionarioParaTela(func);
        }

        private void FuncionarioParaTela(Funcionario funcionario)
        {
            txtNome.Text = funcionario.Nome.Trim();
            txtEmail.Text = funcionario.Email.Trim();
            txtSalario.Text = funcionario.Salario.ToString();

            if (funcionario.Sexo == "M") { rbMasculino.Checked = true; } else { rbFeminino.Checked = true; }
            if(funcionario.TipoContrato == "CLT") { rbClt.Checked = true; } else if(funcionario.TipoContrato == "PJ") { rbPj.Checked = true; } else { rbAutonomo.Checked = true; }
        }

        private void SalvarAction(object sender, EventArgs e)
        {
            Funcionario funcionario;

            if (func != null)
            {
                funcionario = func;
                funcionario.DataAtualizacao = DateTime.Now;
            }
            else
            {
                funcionario = new Funcionario();
                funcionario.DataCadastro = DateTime.Now;
            }

            // 1º Mover os dados para a classe funcionario
            funcionario.Nome = txtNome.Text;
            funcionario.Email = txtEmail.Text;
            funcionario.Salario = decimal.Parse(txtSalario.Text);
            funcionario.Sexo = (rbMasculino.Checked) ? "M" : "F";
            funcionario.TipoContrato = (rbClt.Checked) ? "CLT" : (rbPj.Checked) ? "PJ" : "Aut";

            // 2º Validação dos campos
            // 2.1 Deve-se criar uma lista de erros, já que o tratamente esta sendo realizado por DataAnnotation
            List<ValidationResult> listErros = new List<ValidationResult>();
            //2.2 Cria-se um contexto para validar as annotation criadas na Model, passando o objeto em questão
            ValidationContext contexto = new ValidationContext(funcionario);
            // 2.3 Atrav´s do Validator realiza as validações e verifica-se se há algum erro ou não
                // True = Não há erros
                // False = Há erros inseridos no listErros
            bool validado = Validator.TryValidateObject(funcionario, contexto, listErros, true);
            bool resultado;

            if (validado)
            {
                // 3º Salvar os dados
                if (func != null)
                {
                    resultado = FuncionarioDataAccess.AtualizaFuncionario(funcionario);
                }
                else
                {
                    resultado = FuncionarioDataAccess.SalvarFuncionario(funcionario);
                }

                if (resultado)
                {
                    telaPrincipal.AtualizaTabela();
                    this.Close();
                }
                else
                {
                    lblErros.Text = "Erro na inserção de funcionário ao banco!";
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (ValidationResult erro in listErros)
                {
                    sb.Append(erro.ErrorMessage + '\n');
                }

                lblErros.Text = sb.ToString();
            }

        }
    }
}
