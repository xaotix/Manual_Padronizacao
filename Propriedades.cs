using Biblioteca_Daniel;
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
            //dataGridView1.Rows.Clear();
            //Linha = Vars.Buffer.Banco.Linhas.Find(x => x.Celulas.FindAll(y => y.Coluna == "NOME" && y.Valor == Chave).Count > 0);
            //this.Text = "Propriedades [" + Linha.Celulas.Find(x => x.Coluna == "NOME").Valor + "]";


        }
        public void Add(string Chave)
        {


            DB.Linha Linha = Vars.Buffer.Banco.Linhas.Find(x => x.Celulas.FindAll(y => y.Coluna == "NOME" && y.Valor == Chave).Count > 0);
            string Nome = Linha.Celulas.Find(x => x.Coluna == "NOME").Valor;
            MenuProps xt = new MenuProps();

            xt.RequerAnalise.BackColor = Color.Transparent;
            xt.RequerAnalise.Parent = xt.pictureBox1;

            foreach (DB.Celula C in Linha.Celulas)
            {
                if(C.Coluna!="DIR")
                {
                    xt.dataGridView1.Rows.Add(C.Coluna, C.Valor);

                }

            }


            if (Linha != null)
            {
                DB.Celula Padrao = Linha.Celulas.Find(x => x.Coluna.ToLower() == "padronizado");
                if (Padrao != null)
                {
                    if (Padrao.Valor.ToLower() != "sim")
                    {
                        xt.RequerAnalise.Visible = true;
                    }
                    else
                    {
                        xt.RequerAnalise.Visible = false;
                    }
                }
            }



            string ArquivoFim = Linha.Celulas.Find(x => x.Coluna == "DIR").Valor.Replace("|", @"\");
            string png = ArquivoFim.Replace(Arquivo_Pasta.info(ArquivoFim).Extension, ".png");
            string png2 = Vars.PNGDir + Arquivo_Pasta.info_nome(ArquivoFim) + ".png";

            if (File.Exists(png))
            {
                xt.pictureBox1.Image = Image.FromFile(png);
            }
            else if (File.Exists(png2))
            {
                xt.pictureBox1.Image = Image.FromFile(png2);
            }
            else
            {
                xt.pictureBox1.Image = Properties.Resources.semimagem;
         
            }

            TabPage t = new TabPage(Nome);

            t.Controls.Add(xt);

            tabControl1.TabPages.Add(t);

            tabControl1.SelectedTab = t;

            if (tabControl1.TabPages.Count > 1)
            {
                if (tabControl1.TabPages[0].Text == "TABPAGE1")
                    tabControl1.TabPages.Remove(tabControl1.TabPages[0]);

            }
            SetTitulo();

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

            //foreach (DB.Celula C in Linha.Celulas)
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
