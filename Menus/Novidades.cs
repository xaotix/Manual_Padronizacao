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
    public partial class Novidades : Form
    {
        public Novidades()
        {
            InitializeComponent();
        }

        private void ShowTelaNews_CheckedChanged(object sender, EventArgs e)
        {
            DLM.ini.INI.Set(Vars.CfgUser, "Geral", ShowTelaNews.Name, ShowTelaNews.Checked.ToString());
        }

        private void Novidades_Load(object sender, EventArgs e)
        {
            this.ActiveControl = ShowTelaNews;
            ShowTelaNews.Checked = Convert.ToBoolean(Biblioteca_Daniel.Funcoes_ini.ler_ini(Vars.CfgUser, "Geral", ShowTelaNews.Name, true.ToString()));


            DLM.db.Celula XP = new DLM.db.Celula("MOSTRAR_NEWS","SIM");
            List<DLM.db.Linha> Linhas = Pesquisa.Filtrar(Vars.Buffer.GetTabela().Linhas,new List<DLM.db.Celula> { XP }, true);

            foreach(DLM.db.Linha L in Linhas)
            {
                dataGridView1.Rows.Add(L.Celulas.Find(x => x.Coluna == "NOME").Valor, L.Celulas.Find(x => x.Coluna == "DESCRICAO_REVISAO").Valor);
            }

            webBrowser1.Navigate(Vars.PaginaNews);
            comboBox1.SelectedIndex = 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.Menu.FiltroPesquisa.Text = "NOME";
            List<string> Valores = new List<string>();
            foreach(DataGridViewRow L in dataGridView1.SelectedRows)
            {
                Valores.Add(L.Cells[0].Value.ToString());
            }
            Program.Menu.PesquisaS.Text = string.Join(",", Valores);
            Program.Menu.PesquisarDefault();
            this.Close();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button1.PerformClick();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Biblioteca_Daniel.Funcoes_Form.pesquisar_datagrid(dataGridView1, textBox1.Text, comboBox1.SelectedIndex);
        }
    }
}
