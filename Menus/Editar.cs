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
    public partial class Editar : Form
    {
        public Editar()
        {
            InitializeComponent();
        }
        List<DataGridViewCell> Celulas = new List<DataGridViewCell>();
        private void Editar_Load(object sender, EventArgs e)
        {
            //AlimentaCombo(Nivel1, Vars.Niveis.Nivel1);
            //AlimentaCombo(Nivel2, Vars.Niveis.Nivel2);
            //AlimentaCombo(Nivel3, Vars.Niveis.Nivel3);
            //AlimentaCombo(Nivel4, Vars.Niveis.Nivel4);
            //AlimentaCombo(Nivel5, Vars.Niveis.Nivel5);
            //AlimentaCombo(Ativo, Vars.Niveis.Ativo);
            //AlimentaCombo(Tipo, Vars.Niveis.Tipos);
            Forms.AlimentaCombo(this);


            Funcoes_ini.lercfg(Atributos, Vars.CfgUser);
            Funcoes_ini.lercfg(Tags, Vars.CfgUser);
            Funcoes_ini.lercfg(Geometria, Vars.CfgUser);


            if (Ativo.Items.Count>0)
            {
                Ativo.SelectedIndex = 0;
            }

            Celulas.Clear();

            foreach (DataGridViewRow L in Dados.Rows)
            {
                foreach (DataGridViewCell C in L.Cells)
                {
                    Celulas.Add(C);
                }
            }
            foreach(DataGridViewColumn c in Dados.Columns)
            {
                c.ReadOnly = true;
                c.DefaultCellStyle.BackColor = Color.LightGray;
            }
        }

        private void AlimentaCombo(ComboBox combo, List<string> Chaves)
        {
            combo.Items.Clear();
            foreach (string t in Chaves)
            {
                combo.Items.Add(t);
            }
            //if (combo.Items.Count > 0)
            //{
            //    combo.SelectedIndex = 0;
            //}
        }

        private void AtualizaCelulas(ComboBox Valor)
        {
            if(Valor.Enabled)
            {
                foreach (DataGridViewCell Cel in Celulas.FindAll(x => x.OwningColumn.HeaderText.ToLower() == Valor.Name.ToLower()))
                {
                    Cel.Value = Valor.Text;
                    Cel.Style.BackColor = Color.Yellow;
                 
                }
                this.Update();
            }

        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            if(Funcoes_Form.mensagem("Tem certeza que deseja atualizar " + Dados.Rows.Count + " itens?"))
            {
                progressBar1.Maximum = Dados.Rows.Count;
                progressBar1.Value = 0;
                List<ComboBox> boxes = new List<ComboBox>();
                foreach(var t in this.Controls)
                {
                    if(t is ComboBox)
                    {
                        boxes.Add(t as ComboBox);
                    }
                    else if(t is GroupBox)
                    {
                        foreach( var t1 in (t as GroupBox).Controls)
                        {
                            if (t1 is ComboBox)
                            {
                                boxes.Add(t1 as ComboBox);
                            }
                        }
                    }
                }
                foreach(DataGridViewRow L in Dados.Rows)
                {
                    List<DLM.db.Celula> Edicoes = new List<DLM.db.Celula>();
                    for (int i = 1; i < L.Cells.Count; i++)
                    {
                        string Nome = L.Cells[i].OwningColumn.HeaderText;
                        string valor="";
                        try
                        {
                         valor=  L.Cells[i].Value.ToString();
                        }
                        catch (Exception)
                        {

                        
                        }
                        if(valor == null)
                        {
                            valor = "";
                        }

                        var bb = boxes.Find(x => x.Name.ToUpper() == Nome.ToUpper());
                        if(bb!=null)
                        {
                            if(bb.Enabled)
                            {
                                Edicoes.Add(new DLM.db.Celula(Nome, valor));

                            }
                        }
                       
                    }
                    Vars.Dbase.Update("NOME", L.Cells[0].Value.ToString(), Edicoes, Vars.nome_db, Vars.nome_tb);
                    progressBar1.Value = progressBar1.Value + 1;

                }
                MessageBox.Show("Itens atualizados.");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Nivel1.Enabled = checkBox1.Checked;
            SetColuna(Nivel1.Name, checkBox1.Checked);
        }

        private void SetColuna(string nome_coluna, bool ativo)
        {
            DataGridViewColumn c = Dados.Columns.Cast<DataGridViewColumn>().ToList().Find(x => x.HeaderText.ToUpper() == nome_coluna.ToUpper());
            if (c != null)
            {
                c.ReadOnly = (ativo!=true);
                if(ativo)
                {
                    c.DefaultCellStyle.BackColor = Color.Yellow;

                }
                else
                {
                    c.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Nivel2.Enabled = checkBox2.Checked;
            SetColuna(Nivel2.Name, checkBox2.Checked);

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Nivel3.Enabled = checkBox3.Checked;
            SetColuna(Nivel3.Name, checkBox3.Checked);

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Nivel4.Enabled = checkBox4.Checked;
            SetColuna(Nivel4.Name, checkBox4.Checked);

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Nivel5.Enabled = checkBox5.Checked;
            SetColuna(Nivel5.Name, checkBox5.Checked);

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            Ativo.Enabled = checkBox6.Checked;
            SetColuna(Ativo.Name, checkBox6.Checked);

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            Codigo.Enabled = checkBox7.Checked;
            SetColuna(Codigo.Name, checkBox7.Checked);

        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            DESCRICAO.Enabled = checkBox8.Checked;
            SetColuna(DESCRICAO.Name, checkBox8.Checked);

        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            SECAO.Enabled = checkBox9.Checked;
            SetColuna(SECAO.Name, checkBox9.Checked);

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            TAG1.Enabled = checkBox10.Checked;
            SetColuna(TAG1.Name, checkBox10.Checked);

        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            TAG2.Enabled = checkBox11.Checked;
            SetColuna(TAG2.Name, checkBox11.Checked);

        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            TAG3.Enabled = checkBox12.Checked;
            SetColuna(TAG3.Name, checkBox12.Checked);

        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            TAG4.Enabled = checkBox13.Checked;
            SetColuna(TAG4.Name, checkBox13.Checked);

        }

        private void AtualizarValores(object sender, EventArgs e)
        {
            ComboBox x = (ComboBox)sender;
            AtualizaCelulas(x);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            Comprimento.Enabled = checkBox14.Checked;
            SetColuna(Comprimento.Name, checkBox14.Checked);

        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            Largura.Enabled = checkBox15.Checked;
            SetColuna(Largura.Name, checkBox15.Checked);

        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            Espessura.Enabled = checkBox16.Checked;
            SetColuna(Espessura.Name, checkBox16.Checked);

        }

        private void Editar_FormClosing(object sender, FormClosingEventArgs e)
        {
            Funcoes_ini.gravarcfg(Atributos, Vars.CfgUser);
            Funcoes_ini.gravarcfg(Tags, Vars.CfgUser);
            Funcoes_ini.gravarcfg(Geometria, Vars.CfgUser);
        }

        private void Tipo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            Tipo.Enabled = checkBox17.Checked;
            SetColuna(Tipo.Name, checkBox17.Checked);

        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            MOSTRAR_NEWS.Enabled = checkBox18.Checked;
            SetColuna(MOSTRAR_NEWS.Name, checkBox18.Checked);

        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            DESCRICAO_REVISAO.Enabled = checkBox19.Checked;
            SetColuna(DESCRICAO_REVISAO.Name, checkBox19.Checked);

        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {
            ACABAMENTO.Enabled = checkBox20.Checked;
            SetColuna(ACABAMENTO.Name, checkBox20.Checked);

        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            MATERIAL.Enabled = checkBox21.Checked;
            SetColuna(MATERIAL.Name, checkBox21.Checked);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string arqv = Arquivo_Pasta.salvar("csv", "Selecione o destino");
            if(arqv!="")
            {
                List<List<string>> lista = new List<List<string>>();
                List<string> Cab = new List<string>();
                foreach (DataGridViewColumn cx in Dados.Columns)
                {
                    Cab.Add(cx.HeaderText);
                }
                lista.Add(Cab);

             lista.AddRange(Funcoes_Form.datagrid_para_lista(Dados));
                Buffer_Texto.gravar_arquivo_csv(arqv, lista);

                MessageBox.Show("Arquivo salvo.");
            }



        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            PADRONIZADO.Enabled = checkBox22.Checked;
            SetColuna(PADRONIZADO.Name, checkBox22.Checked);

        }

        private void Largura_KeyDown(object sender, KeyEventArgs e)
        {
     
        }

        private void Largura_KeyPress(object sender, KeyPressEventArgs e)
        {
            Biblioteca_Daniel.Texto.verifica(e, this, "0123456789.");
        }

        private void checkBox23_CheckedChanged(object sender, EventArgs e)
        {
            Peso.Enabled = checkBox23.Checked;
            SetColuna(Peso.Name, checkBox23.Checked);



        }

        private void ferramentasToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mapearNiveisToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
