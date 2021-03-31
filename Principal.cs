using Biblioteca_Daniel;
using DB;
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
            Directory.CreateDirectory(Vars.PastaTmp);
            this.Text = Application.ProductName + " v" + Application.ProductVersion;
            LabelCab.Text = "";
            Program.Cadastro = new Cadastro();
            SplitPesquisa.Panel1Collapsed = true;
            //SplitResultados.Panel2Collapsed = true;
            //ArvoreLista = new NativeTreeView();
            splitContainer1.Panel2Collapsed = true;

            this.ActiveControl = PesquisaS;
            this.Update();

            Vars.Conexao = new Banco("10.54.0.90", "3306", "root", "root", "plm");
      
            if(Vars.Conexao.GetEstaOnline()==false)
            {
                MessageBox.Show("Servidor Offline. Contacte suporte.");
                Application.Exit();
            }

            Vars.Buffer.tree.XMLParaArvore(Vars.Arvore, ArvoreLista);


            if (ArvoreLista.Nodes.Count > 0)
            {


                ArvoreLista.Nodes[0].Expand();
            }


            Vars.Buffer.Banco = Vars.Conexao.Consulta(new Celula("Ativo", "Sim"), false, "plm", "padronizacao");
            Resultado.Items.Clear();





            Forms.AlimentaCombo(this, true);
            MostrarNews();

            //if (Vars.Repositorio().Count == 2)
            //{
            //    if (Directory.Exists(Vars.Repositorio()[0]) == false)
            //    {
            //        Vars.SubstituirRepositorio = true;
            //    }
            //    else
            //    {
            //        Vars.SubstituirRepositorio = false;
            //    }
            //}
            //else
            //{
                Vars.SubstituirRepositorio = false;
            //}
            lvwColumnSorter = new ListViewColumnSorter();
            this.Resultado.ListViewItemSorter = lvwColumnSorter;
            if(File.Exists(Vars.ArvoreUser))
            {
                ArvoreXML x = new ArvoreXML();
                x.XMLParaArvore(Vars.ArvoreUser, ArvoreUser);
            }

            //Preview.Controls.Add(pictureBox3);
            //pictureBox3.Location = new Point(0, 0);
            //pictureBox3.Visible = true;
            //pictureBox3.BackColor = Color.Transparent;

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

        private void testeToolStripMenuItem_Click(object sender, EventArgs e)
        {
         

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Funcoes_Form.pesquisar_datagrid(PropriedadesGrid, textBox1.Text, 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SplitResultado.Panel1Collapsed = true;
        }

        private void PropriedadesGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                //button1.PerformClick();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SplitPesquisa.Panel1Collapsed = true;
        }

        private void pesquisaAvançadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(SplitPesquisa.Panel1Collapsed)

            {
                SplitPesquisa.Panel1Collapsed = false;
                SplitPesquisa.SplitterDistance = 250;


            }
            else
            {
                SplitPesquisa.Panel1Collapsed = true;

            }
        }

        private void PreviewArquivos(object sender, EventArgs e)
        {
            if(Resultado.SelectedItems.Count>3)
            {
                if (!Funcoes_Form.mensagem("Tem certeza que deseja abrir os " + Resultado.SelectedItems.Count + " itens selecionados?")) return;                
            }
            foreach (ListViewItem Selecao in Resultado.SelectedItems)
            {
                Funcoes.Preview(Selecao.Tag.ToString());
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

                List<DB.Linha> ResultadoNode = new List<Linha>();
                ResultadoNode.AddRange(Vars.Buffer.Banco.Linhas);
                foreach (List<string> Filtro in PesquisaNodes)
                {
                    ResultadoNode = ResultadoNode.FindAll(x => x.Celulas.FindAll(y => y.Coluna == Filtro[0] && y.Valor == Filtro[1]).Count > 0);
                }

                Vars.Buffer.PesquisaUser = ResultadoNode;

                Pesquisa.Add(Vars.Buffer.PesquisaUser, Resultado, IconesLista,Status,Progresso,SplitResultados);
                SplitPesquisa.Visible = true;

                PictureCab.Image = IconesTree.Images[ArvoreLista.SelectedNode.ImageKey];
                LabelCab.Text = ArvoreLista.SelectedNode.Text;


                string Png = Vars.PNGDir + ArvoreLista.SelectedNode.ImageKey;

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
            CheckBox tx = (CheckBox)sender;
            Forms.Status(tx.Tag.ToString(), SplitPesquisa.Panel1, tx.Checked);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<DB.Celula> Celulas = new List<Celula>();
            Forms.GetComboxAtivos(Celulas, SplitPesquisa.Panel1);

            if(SelecaoAvancado.Checked)
            {
                Pesquisa.Add(Pesquisa.Filtrar(Vars.Buffer.PesquisaUser, Celulas, Exato.Checked), Resultado, IconesLista, Status, Progresso, SplitResultados);
            }
            else
            {
                Pesquisa.Add(Pesquisa.Filtrar(Vars.Buffer.Banco.Linhas, Celulas, Exato.Checked), Resultado, IconesLista, Status, Progresso, SplitResultados);
            }


        }

        private void PesquisaAvancadaclicar(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                PesquisaAvancada.PerformClick();
            }
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
            List<DB.Celula> Celulas = new List<Celula>();
            Celulas.Add(new Celula(FiltroPesquisa.Text, PesquisaS.Text.Replace(",",Vars.SeparadorPesquisa)));
            if (Selecao.Checked)
            {
                Pesquisa.Add(Pesquisa.Filtrar(Vars.Buffer.PesquisaUser, Celulas, Exato2.Checked), Resultado, IconesLista, Status, Progresso, SplitResultados);

            }
            else if (FiltroPesquisa.Text == "*")
            {
                Pesquisa.Add(Pesquisa.Filtrar(Vars.Buffer.Banco.Linhas, PesquisaS.Text, Exato2.Checked), Resultado, IconesLista, Status, Progresso, SplitResultados);

            }
            else
            {
                Pesquisa.Add(Pesquisa.Filtrar(Vars.Buffer.Banco.Linhas, Celulas, Exato2.Checked), Resultado, IconesLista, Status, Progresso, SplitResultados);

            }
        }

        private void PesquisaS_TextChanged(object sender, EventArgs e)
        {

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
                    DB.Linha Lt = new Linha("padronizacao_log");

                    DB.Celula ARQS = new Celula("ARQUIVOS", string.Join(" | ", Arquivos));
                    Lt.Celulas.Add(ARQS);
                    string MA = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString().Replace(@"\", "_");
                    Lt.Celulas.Add(new Celula("MA", MA));
                    Lt.Celulas.Add(new Celula("DATA", DateTime.Now.ToString()));
                    Lt.Celulas.Add(new Celula("USER", (string)Biblioteca_Daniel.Outras.nome()));
                    Vars.Conexao.Cadastro(new List<Linha> { Lt },Vars.nome_db,Vars.nome_tb);

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
        public static Propriedades p;
        private void propriedadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (p == null)
            {
                p = new Propriedades();
                p.Show();
            }
            else if (p.IsDisposed)
            {
                p = new Propriedades();
                p.Show();
            }
            foreach (ListViewItem it in Resultado.SelectedItems)
            {
              
                    p.Add(it.Text);
                    p.Show();
              
               
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string arquivo = Arquivo_Pasta.salvar(Vars.Ext.Pesquisa, "Selecione o destino do arquivo.");
            if(arquivo!="")
            {
                Funcoes_ini.gravarcfg(TabPesquisa, arquivo);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string arquivo = Arquivo_Pasta.abrir_string(Vars.Ext.Pesquisa, "Selecione o arquivo.", "");
            if (arquivo != "")
            {
                Funcoes_ini.lercfg(TabPesquisa, arquivo);
            }

            PesquisaAvancada.PerformClick();
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Resultado_Click(object sender, EventArgs e)
        {
          
        }

        private void Resultado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Resultado.SelectedItems.Count == 1)
            {
                string ArquivoFim = Resultado.SelectedItems[0].Tag.ToString().Replace("|", @"\");

                DB.Linha Linha = Vars.Buffer.Banco.Linhas.Find(x => x.Celulas.FindAll(y => y.Coluna == "NOME" && y.Valor == Resultado.SelectedItems[0].Text).Count > 0);

                if(Linha!=null)
                {
                   DB.Celula Padrao = Linha.Celulas.Find(x => x.Coluna.ToLower() == "padronizado");
                    if(Padrao!=null)
                    {
                        if(Padrao.Valor.ToLower() !="sim")
                        {
                            RequerAnalise.Visible = true;
                        }
                        else
                        {
                            RequerAnalise.Visible = false;
                        }
                    }
                }

                if (File.Exists(ArquivoFim))
                {
                    string ext = Arquivo_Pasta.info(ArquivoFim).Extension;

                    string png = ArquivoFim.Replace(ext, ".png");
                    string png2 = Vars.PNGDir + Arquivo_Pasta.info_nome(ArquivoFim) + ".png";
                    if (File.Exists(png))
                    {
                        Preview.Image = Image.FromFile(png);
                        Preview.Image.Tag = Resultado.SelectedItems[0].Text;                       
                       
                    }
                    else if(File.Exists(png2))
                    {
                        Preview.Image = Image.FromFile(png2);
                        Preview.Image.Tag = Resultado.SelectedItems[0].Text;                      
                    }
                    
                    else
                    {
                        Preview.Image = Properties.Resources.semimagem;
                        Preview.Image.Tag = null;
                    }
                }



            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SplitResultados.Panel2Collapsed = true;
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

        private void button5_Click(object sender, EventArgs e)
        {
            PreviewPNG();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
        }

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode xt = new TreeNode(Texto.Text);
            xt.ImageKey = "Pasta";
            xt.SelectedImageKey = "Pasta";
            xt.Tag = Texto.Text;

            if (ArvoreUser.SelectedNode != null)
            {
                ArvoreUser.SelectedNode.Nodes.Add(xt);
            }
            else
            {
                ArvoreUser.Nodes.Add(xt);
            }
        }

        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void sToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ArvoreXML x = new ArvoreXML();
            x.ArvoreParaXML(ArvoreUser, Vars.ArvoreUser);
            MessageBox.Show("Alterações Salvas.");
        }

        private void Resultado_DragDrop(object sender, DragEventArgs e)
        {

        }
        List<string> Chaves = new List<string>();
        private void Resultado_ItemDrag(object sender, ItemDragEventArgs e)
        {
            //foreach(ListViewItem Xt in Resultado.SelectedItems)
            //{
                Resultado.DoDragDrop(e.Item, DragDropEffects.Move);

            //}

        }

        private void ArvoreUser_DragDrop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode dn = ((TreeView)sender).GetNodeAt(pt);
                if(dn==null)
                {
                    MessageBox.Show("Nenhuma pasta selecionada. Crie uma pasta e tente novamente.");
                    return;
                }
                ListViewItem lvt = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                List<string> Chaves = new List<string>();
                if (Funcoes_Form.mensagem("Você tem certeza que deseja adicionar os " + Resultado.SelectedItems.Count + " itens selecionados na pasta [" + dn.Text + "]?"))
                {
                    
                    foreach (ListViewItem Xt in Resultado.SelectedItems)
                    {
                        Chaves.Add(Xt.Text);
                    }
                    List<string> cx = dn.Tag.ToString().Split(Vars.SeparadorChaves.ToCharArray()).ToList();
                    if (cx.Count > 2)
                    {
                        Chaves.AddRange(cx[2].Split(Vars.SeparadorPesquisa.ToArray()).ToList());
                    }
                    Chaves = Chaves.Distinct().ToList();
                    dn.Tag = cx[0] + Vars.SeparadorChaves + "NOME" + Vars.SeparadorChaves + string.Join(Vars.SeparadorPesquisa, Chaves);

                    ArvoreXML x = new ArvoreXML();
                    x.ArvoreParaXML(ArvoreUser, Vars.ArvoreUser);
                    MessageBox.Show("Alterações Salvas.\n " + Resultado.SelectedItems.Count + " itens adicionados na pasta " + dn.Text);

                }

            }
        }

        private void Resultado_DragEnter(object sender, DragEventArgs e)
        {
          
        }

        private void ArvoreUser_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
                e.Effect = DragDropEffects.Move;
        }

        private void ArvoreUser_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelecionaNodeUser();
        }

        private void SelecionaNodeUser()
        {
            if (ArvoreUser.SelectedNode != null)
            {
                TreeNode Selecao = ArvoreUser.SelectedNode;
                string Chave = Selecao.Tag.ToString();

                List<string> Chaves = Chave.Split(Vars.SeparadorChaves.ToCharArray()).ToList();
                if (Chaves.Count > 2)
                {
                    List<DB.Celula> Celulas = new List<Celula>();
                    Celulas.Add(new Celula(Chaves[1], Chaves[2]));
                    Pesquisa.Add(Pesquisa.Filtrar(Vars.Buffer.Banco.Linhas, Celulas, Exato2.Checked), Resultado, IconesLista, Status, Progresso, SplitResultados);

                }
                else
                {
                    Resultado.Items.Clear();
                }

                PictureCab.Image = IconesTree.Images[ArvoreUser.SelectedNode.ImageKey];
                LabelCab.Text = "Pasta Usuário - [" + ArvoreUser.SelectedNode.Text + "]";
            }
        }

        private void xToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = true;
        }

        private void pastasUsuárioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel2Collapsed = false;
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;
            }
        }

        private void Texto_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode== Keys.Enter)
            {
                xToolStripMenuItem.PerformClick();
            }
        }

        private void imagemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewPNG();
        }

        private void ArvoreUser_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && ArvoreUser.SelectedNode!=null)
            {
                contextMenuStrip2.Show(ArvoreUser, e.Location);
            }
        }

        private void limparItensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> Chaves = ArvoreUser.SelectedNode.Tag.ToString().Split('|').ToList();
            if(Chaves.Count>2)
            {
                if(Funcoes_Form.mensagem("Você tem certeza que deseja remover os " + Chaves[2].Split(',').Count() + " itens da pasta [" + ArvoreUser.SelectedNode.Text + "]?"))
                {
                    ArvoreUser.SelectedNode.Tag = ArvoreUser.SelectedNode.Text;
                    ArvoreXML x = new ArvoreXML();
                    x.ArvoreParaXML(ArvoreUser, Vars.ArvoreUser);
                    MessageBox.Show("Alterações Salvas.");
                }
            }
        }

        private void novaPastaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ArvoreUser.SelectedNode != null)
            {
                if (Funcoes_Form.mensagem("Você tem certeza que deseja remover a pasta [" + ArvoreUser.SelectedNode.Text + "]?"))
                {


                    if (ArvoreUser.SelectedNode != null)
                    {
                        ArvoreUser.Nodes.Remove(ArvoreUser.SelectedNode);
                    }

                }
            }
        }

        private void Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            Funcoes.Matar(Vars.ExecDXF);
            Funcoes.Matar("excel");
            Funcoes.Matar("word");

            if (Directory.Exists(Vars.PastaTmp))
            {
                foreach(string arq in Arquivo_Pasta.lista(Vars.PastaTmp,"*"))
                {
                    try
                    {
                        File.Delete(arq);
                    }
                    catch (Exception)
                    {

                        //throw;
                    }
                }
            }
        }
        ImageZoom niz = new ImageZoom();
        private void Preview_DoubleClick(object sender, EventArgs e)
        {
            if (Preview.Image.Tag!=null)

            {
                //if (niz.IsDisposed)
                //{
                    niz = new ImageZoom();
            //}               
              
                niz.pictureBox1.Image = Preview.Image;            
                niz.Text = Preview.Image.Tag.ToString();
                niz.Show();
                niz.Update();
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

        private void ArvoreUser_KeyDown(object sender, KeyEventArgs e)
        {
            ChamaAtalhos(e);
        }

        private void ArvoreLista_KeyDown(object sender, KeyEventArgs e)
        {
            ChamaAtalhos(e);
        }

        private void PesquisaS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ESPESSURA_KeyPress(object sender, KeyPressEventArgs e)
        {
            Biblioteca_Daniel.Texto.verifica(e, this, "0123456789.");
        }

        private void ArvoreUser_AfterSelect(object sender, EventArgs e)
        {
            SelecionaNodeUser();

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
                    DB.Linha Lt = new Linha("padronizacao_log");

                    DB.Celula ARQS = new Celula("ARQUIVOS", string.Join(" | ", Arquivos));
                    Lt.Celulas.Add(ARQS);
                    string MA = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString().Replace(@"\", "_");
                    Lt.Celulas.Add(new Celula("MA", MA));
                    Lt.Celulas.Add(new Celula("DATA", DateTime.Now.ToString()));
                    Lt.Celulas.Add(new Celula("USER", (string)Biblioteca_Daniel.Outras.nome()));
                    Vars.Conexao.Cadastro(new List<Linha> { Lt }, Vars.nome_db,Vars.nome_tb);
                }

            }
        }
    }
}
