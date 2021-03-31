using Biblioteca_Daniel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manual_Padronizacao
{
    public partial class Acesso : Form
    {
        public Acesso()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Funcoes_ini.gravarcfg(this, Vars.CfgUser);
            if (Senha.Text=="@medainoC")
            {
                if(Program.Cadastro.IsDisposed)
                {
                    Program.Cadastro = new Cadastro();
                }
                Program.Cadastro.Show();
                List<string> Selecao = new List<string>();
                if(Program.Menu.IsDisposed==false)
                {
                    if(Program.Menu.Resultado.SelectedItems.Count>0)
                    {
                        foreach (ListViewItem tx in Program.Menu.Resultado.SelectedItems)
                        {
                            Selecao.Add(tx.Text);
                        }
                        Program.Cadastro.Combo2.Text = string.Join(Vars.SeparadorPesquisa, Selecao);
                        Program.Cadastro.Nome.Text = "NOME";
                        Program.Cadastro.Pesquisar(true);
                    }
                   
                }
                
                this.Close();
            }
            else
            {
                MessageBox.Show("Senha Incorrreta.");
            }

          

        }

        private void Senha_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Ok.PerformClick();
            }
            else if(e.KeyCode == Keys.Escape)
            {
                Cancelar.PerformClick();
            }
        }

        private void Acesso_Load(object sender, EventArgs e)
        {
            Funcoes_ini.lercfg(this, Vars.CfgUser);
            this.ActiveControl = Senha;

        }
    }
}
