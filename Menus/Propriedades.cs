using Biblioteca_Daniel;
using Conexoes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manual_Padronizacao
{
    public partial class Propriedades : Form
    {
        public Propriedades(/*string Chave*/)
        {
            InitializeComponent();
        }
        public void Add(string Chave)
        {


            var consulta = Vars.Buffer.GetTabela().Filtrar("NOME",Chave);

            if(consulta.Count>0)
            {
                var Linha = consulta.Linhas.First();
                string Nome = Linha["NOME"].Valor;
                MenuProps novo_menu = new MenuProps();

                novo_menu.RequerAnalise.BackColor = Color.Transparent;
                novo_menu.RequerAnalise.Parent = novo_menu.Preview;

                foreach (DLM.db.Celula C in Linha.Celulas)
                {
                    if (C.Coluna != "DIR")
                    {
                        novo_menu.dataGridView1.Rows.Add(C.Coluna, C.Valor);

                    }

                }


                if (Linha != null)
                {
                    novo_menu.RequerAnalise.Visible = !Linha["padronizado"].Boolean();
                }



                string ArquivoFim = Linha["DIR"].Valor.Replace(" | ", @"\");

                var arq = new Arquivo(ArquivoFim);
                Funcoes.SetPNG(arq, novo_menu.Preview);

                TabPage t = new TabPage(Nome);

                t.Controls.Add(novo_menu);

                tabControl1.TabPages.Add(t);

                tabControl1.SelectedTab = t;

                if (tabControl1.TabPages.Count > 1)
                {
                    if (tabControl1.TabPages[0].Text == "TABPAGE1")
                        tabControl1.TabPages.Remove(tabControl1.TabPages[0]);

                }
                SetTitulo();
            }



        }

        private void SetTitulo()
        {
            if (tabControl1.TabPages.Count > 1)
            {
                this.Text = "Propriedades [" + tabControl1.TabPages.Count + " itens]";

            }
            else
            {
                this.Text = "Propriedades [" + tabControl1.TabPages[0].Text + "]";

            }
        }

        private void Propriedades_Load(object sender, EventArgs e)
        {
            //comboBox1.Items.Clear();
            //foreach(DataGridViewColumn x in dataGridView1.Columns)
            //{
            //    comboBox1.Items.Add(x.HeaderText);
            //}
            //if(comboBox1.Items.Count>0)
            //{
            //    comboBox1.SelectedIndex = 0;
            //}

            //foreach (DLM.db.Celula C in Linha.Celulas)
            //{

            //    dataGridView1.Rows.Add(C.Coluna, C.Valor);

            //}
            //this.Text = "Propriedades";
            //tabPage1.Text = "[" + Linha.Celulas.Find(x => x.Coluna == "NOME").Valor + "]";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Funcoes_Form.pesquisar_datagrid(dataGridView1, textBox1.Text, comboBox1.SelectedIndex);
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if(tabControl1.TabPages.Count>1)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                    SetTitulo();
                }
            }
           
        }
    }
}
