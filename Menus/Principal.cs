using Biblioteca_Daniel;
using Conexoes;
using DLM.db;
using DLM.vars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manual_Padronizacao
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }
        public static Novidades News = new Novidades();


        public void PegaFocus(object sender, EventArgs e)
        {
            ComboBox t = (ComboBox)sender;
            t.BackColor = Color.LightCyan;
        }
        public void PerdeFocus(object sender, EventArgs e)
        {
            ComboBox t = (ComboBox)sender;
            t.BackColor = Color.White;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PesquisaS.GotFocus += PegaFocus;
            PesquisaS.LostFocus += PerdeFocus;
            Preview.Image = Properties.Resources.semimagem;
            this.Text = Application.ProductName + " v" + Application.ProductVersion;
            LabelCab.Text = "";
            Program.Cadastro = new Cadastro();
            this.ActiveControl = PesquisaS;
            this.Update();

            Vars.Dbase = new Banco(DLM.vars.Cfg.Init.MySQL_Servidor, DLM.vars.Cfg.Init.MySQL_Porta, DLM.vars.Cfg.Init.MySQL_User, DLM.vars.Cfg.Init.MySQL_Senha,DLM.vars.Cfg.Init.db_plm);
      
            if(Vars.Dbase.GetEstaOnline()==false)
            {
                MessageBox.Show("Servidor Offline. Contacte suporte.");
                Application.Exit();
            }

            Vars.Buffer.tree.XMLParaArvore(Vars.Arvore, ArvoreLista);


            if (ArvoreLista.Nodes.Count > 0)
            {
                ArvoreLista.Nodes[0].Expand();
            }


            Resultado.Items.Clear();


            Forms.AlimentaCombo(this, true);
            MostrarNews();

                Vars.SubstituirRepositorio = false;
 
            lvwColumnSorter = new ListViewColumnSorter();
            this.Resultado.ListViewItemSorter = lvwColumnSorter;

            RequerAnalise.BackColor = Color.Transparent;
            RequerAnalise.Parent = Preview;
        }

        private static void MostrarNews()
        {
            if (News.IsDisposed)
            {
                News = new Novidades();
            }

            if (Convert.ToBoolean(Funcoes_ini.ler_ini(Vars.CfgUser, "Geral", News.ShowTelaNews.Name, true.ToString())) == true)
            {
                News.Show();
            }
        }

        private void cadastroToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            Acesso nA = new Acesso();
            
            nA.Show();
        }



        private void PreviewArquivos(object sender, EventArgs e)
        {
            if(Arquivos_Selecionados.Count>3)
            {
                if (!Funcoes_Form.mensagem("Tem certeza que deseja abrir os " + Resultado.SelectedItems.Count + " itens selecionados?")) return;                
            }
            foreach (var Selecao in Arquivos_Selecionados)
            {
                Funcoes.Abrir(Selecao.Endereco);
            }
        }

        private void SelecaoNode(object sender, EventArgs e)
        {
           


        }

        private void ArvoreLista_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (ArvoreLista.SelectedNode != null)
            {
                TreeNode xnode = ArvoreLista.SelectedNode;
                List<List<string>> PesquisaNodes = new List<List<string>>();

                if (xnode.Text.ToLower() != "raiz")
                {

                    PesquisaNodes.Add(new List<string> { xnode.Tag.ToString(), xnode.Text });

                    TreeNode tmp = xnode;
                    while (tmp != null)
                    {
                        if (tmp.Text.ToLower() != "raiz")
                        {
                            PesquisaNodes.Add(new List<string> { tmp.Tag.ToString(), tmp.Text });
                        }
                        tmp = tmp.Parent;

                    }
                }

                List<Linha> ResultadoNode = new List<Linha>();
                ResultadoNode.AddRange(Vars.Buffer.GetTabela().Linhas);
                foreach (List<string> Filtro in PesquisaNodes)
                {
                    ResultadoNode = ResultadoNode.FindAll(x => x.Celulas.FindAll(y => y.Coluna == Filtro[0] && y.Valor == Filtro[1]).Count > 0);
                }

                Vars.Buffer.PesquisaUser = ResultadoNode;

                Pesquisa.Add(Vars.Buffer.PesquisaUser, Resultado, IconesLista,Status,Progresso,SplitResultados);
 

                PictureCab.Image = IconesTree.Images[ArvoreLista.SelectedNode.ImageKey];
                LabelCab.Text = ArvoreLista.SelectedNode.Text;


                string Png = DLM.vars.Cfg.Init.PNG_Manual() + ArvoreLista.SelectedNode.ImageKey;

                if(File.Exists(Png))
                {
                    Preview.Image = Image.FromFile(Png);
                }
                else
                {
                    Preview.Image = Properties.Resources.semimagem;

                }
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            //CheckBox tx = (CheckBox)sender;
            //Forms.Status(tx.Tag.ToString(), SplitPesquisa.Panel1, tx.Checked);
        }





        private void PesquisaS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PesquisarDefault();
            }
        }

        public void PesquisarDefault()
        {
            List<DLM.db.Celula> Celulas = new List<Celula>();
            Celulas.Add(new Celula(FiltroPesquisa.Text, PesquisaS.Text.Replace(",",Vars.SeparadorPesquisa)));
            if (Selecao.Checked)
            {
                Pesquisa.Add(Pesquisa.Filtrar(Vars.Buffer.PesquisaUser, Celulas, Exato2.Checked), Resultado, IconesLista, Status, Progresso, SplitResultados);

            }
            else if (FiltroPesquisa.Text == "*")
            {
                Pesquisa.Add(Pesquisa.Filtrar(Vars.Buffer.GetTabela().Linhas, PesquisaS.Text, Exato2.Checked), Resultado, IconesLista, Status, Progresso, SplitResultados);

            }
            else
            {
                Pesquisa.Add(Pesquisa.Filtrar(Vars.Buffer.GetTabela().Linhas, Celulas, Exato2.Checked), Resultado, IconesLista, Status, Progresso, SplitResultados);

            }
        }


        private void Resultado_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                PreviewArquivos(sender, e);
            }
            ChamaAtalhos(e);

        }

        private void exportarSelecionadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportarSelecionados();
        }

        private void ExportarSelecionados()
        {
            if (Resultado.SelectedItems.Count > 0)
            {
                TextBox Pastax = new TextBox();                
                Arquivo_Pasta.pasta("Selecione o destino dos arquivos.", Pastax);
                if (Directory.Exists(Pastax.Text))
                {
                    bool subs = false;
                    if (Funcoes_Form.mensagem("Substituir arquivos (se já existirem)?"))
                    {
                        subs = true;
                    }
                        Tela_Load w = new Tela_Load();
                    w.Mensagem.Text = "Copiando arquivos...";
                    w.Progresso.Maximum = Resultado.SelectedItems.Count;
                    w.Progresso.Value = 0;
                    w.Show();
                    w.Update();
                    List<string> Arquivos = new List<string>();
                    foreach (ListViewItem Selecao in Resultado.SelectedItems)
                    {

                        w.Progresso.Value = w.Progresso.Value + 1;

                        try
                        {
                            string Arq = Selecao.Tag.ToString();
                            string ArqDest = Pastax.Text + @"\" + Arquivo_Pasta.info_nome(Arq) + Arquivo_Pasta.info(Arq).Extension;
                            if (File.Exists(ArqDest))
                            {
                                if (subs)
                                {
                                    File.Delete(ArqDest);
                                    File.Copy(Arq, ArqDest);
                                    Arquivos.Add(Arquivo_Pasta.info_nome(Arq));
                                }
                            }
                            else
                            {
                                File.Copy(Arq, ArqDest);
                                Arquivos.Add(Arquivo_Pasta.info_nome(Arq));
                            }


                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("Erro " + ex.Message);
                        }

                    }
                    DLM.db.Linha Lt = new Linha("padronizacao_log");

                    DLM.db.Celula ARQS = new Celula("ARQUIVOS", string.Join(" | ", Arquivos));
                    Lt.Celulas.Add(ARQS);
                    string MA = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString().Replace(@"\", "_");
                    Lt.Celulas.Add(new Celula("MA", Conexoes.DBases.GetUserAtual().ma));
                    Lt.Celulas.Add(new Celula("DATA", DateTime.Now.ToString()));
                    Lt.Celulas.Add(new Celula("USER", Conexoes.DBases.GetUserAtual().nome));
                    Vars.Dbase.Cadastro(new List<Linha> { Lt },Vars.nome_db,Vars.nome_tb);

                    w.Close();

                    if (Funcoes_Form.mensagem("Cópia finalizada. Deseja abrir a pasta?"))
                    {
                        //Funcoes.Abrir(Pastax.Text);
                        System.Diagnostics.Process.Start(Pastax.Text);
                    }
                }

            }
        }

        private void FiltroPesquisa_TextChanged(object sender, EventArgs e)
        {
            Forms.AlimentaCombo(PesquisaS, Arquivo_Pasta.ler(Vars.DirCfgs + FiltroPesquisa.Text + ".db"), true);
            this.Update();
        }

        private void visualizarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void íconesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Resultado.View = View.LargeIcon;
        }

        private void detalhesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Resultado.View = View.Details;

        }

        private void ladoALadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Resultado.View = View.List;

        }

        private void Resultado_MouseDown(object sender, MouseEventArgs e)
        {
            if(Resultado.SelectedItems.Count > 0)
            {
                propriedadesToolStripMenuItem.Enabled = true;
                abrirToolStripMenuItem.Enabled = true;
                exportarToolStripMenuItem.Enabled = true;
            }
            else
            {
                propriedadesToolStripMenuItem.Enabled = false;
                abrirToolStripMenuItem.Enabled = false;
                exportarToolStripMenuItem.Enabled = false;
            }
            if(e.Button==MouseButtons.Right)
            {
                contextMenuStrip1.Show(Resultado,e.Location);  
            }
        }

        private void exportarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportarSelecionados();
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewArquivos(sender, e);
        }

        private void íconesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Resultado.View = View.LargeIcon;

        }

        private void listaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Resultado.View = View.List;

        }

        private void detalhesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Resultado.View = View.Details;

        }
        public static Propriedades menu_props { get; set; }
        private void propriedadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menu_props == null)
            {
                menu_props = new Propriedades();
                menu_props.Show();
            }
            else if (menu_props.IsDisposed)
            {
                menu_props = new Propriedades();
                menu_props.Show();
            }
            foreach (ListViewItem it in Resultado.SelectedItems)
            {
              
                    menu_props.Add(it.Text);
                    menu_props.Show();
              
               
            }
        }

 



        private void novidadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MostrarNews();
            News.Show();
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sobre ns = new Sobre();
            ns.Show();
        }
        private ListViewColumnSorter lvwColumnSorter;
        private void Resultado_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.Resultado.Sort();
        }


        public List<Arquivo> Arquivos_Selecionados { get; set; } = new List<Arquivo>();
        public Arquivo Arquivo_Selecionado { get; set; }
        public DLM.db.Linha Arquivo_Selecionado_Dados { get; set; }
        private void Resultado_SelectedIndexChanged(object sender, EventArgs e)
        {
            Arquivos_Selecionados = new List<Arquivo>();
            Arquivo_Selecionado_Dados = null;
            Arquivo_Selecionado = null;

            foreach (var s in Resultado.SelectedItems)
            {
                string arq2 = Resultado.SelectedItems[0].Tag.ToString().Replace("|", @"\");
                Arquivos_Selecionados.Add(new Arquivo(arq2));
            }


            if (Arquivos_Selecionados.Count>0)
            {
                Arquivo_Selecionado = Arquivos_Selecionados.First();

                var Tabela = Vars.Buffer.GetTabela().Filtrar("NOME", Arquivo_Selecionado.Nome);

                if(Tabela.Count>0)
                {
                    Arquivo_Selecionado_Dados = Tabela.Linhas.First();
                }
                RequerAnalise.Visible = !Arquivo_Selecionado_Dados["padronizado"].Boolean();
                Funcoes.SetPNG(Arquivo_Selecionado, Preview);
            }
        }





        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewPNG();
        }

        private void PreviewPNG()
        {
            if (SplitResultados.Panel2Collapsed)
            {
                SplitResultados.Panel2Collapsed = false;
            }
            else
            {
                SplitResultados.Panel2Collapsed = true;
            }
        }










  






        private void imagemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewPNG();
        }





        private void Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            Conexoes.Utilz.Matar(Vars.ExecDXF);
            Conexoes.Utilz.Matar("excel");
            Conexoes.Utilz.Matar("word");

            //Conexoes.Utilz.LimparArquivosPasta(DLM.vars.Cfg.Init.TMP_Manual());
        }
        private ImageZoom zoom_imagem { get; set; } = new ImageZoom();
        private void Preview_DoubleClick(object sender, EventArgs e)
        {
            if (Preview.Image.Tag != null)
            {
                zoom_imagem = new ImageZoom();
                zoom_imagem.pictureBox1.Image = Preview.Image;
                zoom_imagem.Text = Preview.Image.Tag.ToString();
                zoom_imagem.Show();
                zoom_imagem.Update();
            }
        }

        private void Principal_KeyDown(object sender, KeyEventArgs e)
        {
            ChamaAtalhos(e);
        }

        private void ChamaAtalhos(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                this.ActiveControl = PesquisaS;
            }
        }


        private void ArvoreLista_KeyDown(object sender, KeyEventArgs e)
        {
            ChamaAtalhos(e);
        }

        private void PesquisaS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool abrir_sempre = Funcoes_Form.mensagem("Deseja sempre abrir com Auto-CAD?");
            if(abrir_sempre)
            {
                MessageBox.Show("A partir de agora o programa abrirá os desenhos sempre com AutoCAD (Se existir na máquina).\nDê 2 cliques para abri-los normalmente.\n Para voltar ao normal, clique novamente em 'Abrir com Cad e clique em não na mensagem de prompt.'");

            }
            Funcoes_ini.gravar_ini(Vars.cfguser, "Geral", "CAD", abrir_sempre.ToString());
            if (Resultado.SelectedItems.Count > 0)
            {
                TextBox Pastax = new TextBox();

                if (Funcoes_Form.mensagem("Abrir os " + Resultado.SelectedItems.Count + " itens selecionados?" ))
                {

                    List<string> Arquivos = new List<string>();
                    foreach (ListViewItem Selecao in Resultado.SelectedItems)
                    {
                        try
                        {
                            string Arq = Selecao.Tag.ToString();
                            Arquivos.Add(Arquivo_Pasta.info_nome(Arq));
                            Process.Start(Funcoes.DuplicanoTemp(Arq));

                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("Erro " + ex.Message);
                        }

                    }
                    DLM.db.Linha Lt = new Linha("padronizacao_log");

                    DLM.db.Celula ARQS = new Celula("ARQUIVOS", string.Join(" | ", Arquivos));
                    Lt.Celulas.Add(ARQS);
                    string MA = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString().Replace(@"\", "_");
                    Lt.Celulas.Add(new Celula("MA", Conexoes.DBases.GetUserAtual().nome));
                    Lt.Celulas.Add(new Celula("DATA", DateTime.Now.ToString()));
                    Lt.Celulas.Add(new Celula("USER", Conexoes.DBases.GetUserAtual().nome));
                    Vars.Dbase.Cadastro(new List<Linha> { Lt }, Vars.nome_db,Vars.nome_tb);
                }

            }
        }

        private void Resultado_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Abrir();
        }

        private void Abrir()
        {
            if (Arquivo_Selecionado != null)
            {
                Arquivo_Selecionado.Abrir();
            }
        }
    }
}
