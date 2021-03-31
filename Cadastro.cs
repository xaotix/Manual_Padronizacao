using Biblioteca_Daniel;
using DB;
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
    public partial class Cadastro : Form
    {
        public Cadastro()
        {
            InitializeComponent();
        }
        private static Editar Editar = new Editar();
        private void Cadastro_Load(object sender, EventArgs e)
        {
            Consulta_Log_Combo.SelectedIndex = 0;
            IconesTree = Program.Menu.IconesTree;

            treeView1.ImageList = Program.Menu.IconesTree;
           listView1.SmallImageList = Program.Menu.IconesTree;
           listView1.StateImageList = Program.Menu.IconesTree;
           listView1.LargeImageList = Program.Menu.IconesTree;
            this.Update();
            listView1.Items.Clear();
            for (int i = 0; i <Program.Menu.IconesTree.Images.Count; i++)
            {
                ListViewItem x = new ListViewItem();
                x.Text = Program.Menu.IconesTree.Images.Keys[i];
                x.ImageKey = x.Text;

                listView1.Items.Add(x);
            }


            PesquisaLocal.SelectedIndex = 0;
            Nome.Items.Clear();
            List<string> Headers = Vars.Conexao.GetColunas("plm", "padronizacao");


            Forms.AlimentaCombo(this, true);

            TreeNode Raiz = new TreeNode();
            Raiz.Text = "Raiz";
            Raiz.Tag = "0";

            treeView1.Nodes.Add(Raiz);

            foreach (string N in Headers)
            {
                Nome.Items.Add(N);
            }

            if (Nome.Items.Count > 0)
            {
                Nome.SelectedIndex = 0;
            }
            Pesquisar(false);


            ArvoreXML tree = new ArvoreXML();
            tree.XMLParaArvore(Vars.Arvore, treeView1);

            treeView1.ExpandAll();
        }

        public void Pesquisar(bool exato)
        {
            List<string> Chaves = Combo2.Text.Split(Vars.SeparadorPesquisa.ToCharArray()).ToList();
            
            if (Chaves.Count==1)
            {

               Vars.Conexao.Consulta(new Celula(Nome.Text, Combo2.Text), exato, "plm", "padronizacao").AlimentaDataGrid(CadastroGrid);
         
            }
            else
            {
                DB.Tabela Resultado = Vars.Conexao.Consulta(new Celula("Ativo", ""),false, "plm", "padronizacao");
                List<DB.Linha> Filtro = new List<Linha>();
                foreach(string Chave in Chaves)
                {
                    List<DB.Linha> nLinhas = Resultado.Filtrar(Nome.Text, Chave,true);
                    foreach(DB.Linha l in nLinhas)
                    {
                        if (Filtro.Find(x => x.Valores().SequenceEqual(l.Valores()))==null)
                        {
                            Filtro.Add(l);
                        }
                    }
                }
                Resultado.Linhas = Filtro;
                Resultado.AlimentaDataGrid(CadastroGrid);
            }
        }

        private void Valor_Click(object sender, EventArgs e)
        {

        }

        private void Valor_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Arquivo_Pasta.pasta("Selecione a pasta", Diretorio);

            AdicionarPastas();

        }

        private void AdicionarPastas()
        {
            if (Importar.Rows.Count > 0)
            {
                if (Funcoes_Form.mensagem("Ao continuar em sim, a lista atual será limpa. Deseja continuar?"))
                {
                    Importar.Rows.Clear();
                    Importar.Columns.Clear();
                    Importar.Columns.Add("NOME", "NOME");
                    Importar.Columns.Add("DIR", "DIR");
                    Importar.Columns.Add("EXTENSAO", "EXTENSAO");
                }
                else
                {
                    return;
                }
            }
            if (Directory.Exists(Diretorio.Text))
            {

                if (Funcoes_Form.mensagem("Deseja ler os arquivos da pasta " + Diretorio.Text + "?"))
                {

                    Tela_Load w = new Tela_Load();
                    w.Show();
                    w.Update();
                    Importar.Rows.Clear();
                    List<string> Arquivos = Arquivo_Pasta.lista(Diretorio.Text, "*", SearchOption.AllDirectories);
                    w.Progresso.Maximum = Arquivos.Count();

                    foreach (string Arq in Arquivos)
                    {
                        w.Progresso.Value = w.Progresso.Value + 1;
                        string nome = Arquivo_Pasta.info_nome(Arq);
                        if (Vars.Extensoes().Find(x => x.ToLower() == Arquivo_Pasta.info(Arq).Extension.ToLower().Replace(".", "")) != null)
                        {
                            Importar.Rows.Add(nome, Arq, Arquivo_Pasta.info(Arq).Extension.ToUpper().Replace(".", ""));
                        }
                    }
                    w.Close();
                }
            }
        }

        private void TextoPesquisaLocal_TextChanged(object sender, EventArgs e)
        {
        }

        private void apagarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Funcoes_Form.apagar_linhas(Importar);
        }

        private void importarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void atualizarDiretóriosToolStripMenuItem_Click(object sender, EventArgs e)
        {

            
            List<List<string>> Lista = Funcoes_Form.datagrid_para_lista(Importar);
            
            if(Funcoes_Form.mensagem("Você tem certeza que deseja atualizar os diretórios dos " + Lista.Count()  + " itens?"))
            {
                Tela_Load w = new Tela_Load();
                w.Show();
                w.Progresso.Maximum = Lista.Count();
                foreach (List<string> L in Lista)
                {
                    w.Progresso.Value = w.Progresso.Value + 1;

                    List<DB.Celula> Edicoes = new List<DB.Celula>();
                    Celula x = new Celula("DIR", L[1]);
                    Vars.Conexao.Update("NOME", L[0], Edicoes,Vars.nome_db, Vars.nome_tb);
                }
                w.Close();
                MessageBox.Show("Finalizado!");
            }
            
        }

        private void editarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(Editar.IsDisposed)
            {
                Editar = new Editar();
            }
            if(CadastroGrid.SelectedRows.Count>0)
            {
                Editar.Dados.Rows.Clear();
                Editar.Dados.Columns.Clear();
                foreach (DataGridViewColumn c in CadastroGrid.Columns)
                {
                    Editar.Dados.Columns.Add(c.Name, c.HeaderText);
                }

                foreach (DataGridViewRow d in CadastroGrid.SelectedRows)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row = (DataGridViewRow)d.Clone();
                    for (int i = 0; i < d.Cells.Count; i++)
                    {
                        row.Cells[i].Value = d.Cells[i].Value;
                    }
                    Editar.Dados.Rows.Add(row);
                }
                Editar.Show();
            }
            
            
        }

        private void atualizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pesquisar(false);
        }

        private void Valor_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Pesquisar(false);
            }
        }

        private void TextoPesquisaLocal_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Funcoes_Form.pesquisar_datagrid(Importar, TextoPesquisaLocal.Text, PesquisaLocal.SelectedIndex);

            }
        }
        private static TreeNode Selecao;
        private static string Nivel = "0";
        private static string NivelSup = "0";
        private static string ValorNode = "";
        private static int Imagem = 0;
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Selecao = e.Node;
            
            if (Selecao != null)
            {
                RemoveNode.Enabled = true;
                AddEmNivel.Text = "Adicionar item em " + e.Node.Text;
                if(e.Node.Tag.ToString()!="0")
                {
                    EditarNode.Enabled = true;
                    Forms.AlimentaCombo(ComboEditNodes, Arquivo_Pasta.ler(Vars.DirCfgs + e.Node.Tag.ToString() + ".db"));
                    ComboEditNodes.Text = e.Node.Text;
                }
                
                this.Update();
            }
            else
            {
                RemoveNode.Enabled = false;
                EditarNode.Enabled = false;

            }
            if (e.Node.Parent != null)
            {
                NivelSup = e.Node.Tag.ToString();

            }
            else
            {
                NivelSup = "0";
            }
            switch (NivelSup)
            {
                case "0":
                    Nivel = "NIVEL1";
                    NIVEL1.Enabled = true;
                    NIVEL2.Enabled = false;
                    NIVEL3.Enabled = false;
                    NIVEL4.Enabled = false;
                    NIVEL5.Enabled = false;
                    AddNode.Enabled = true;

                    break;
                case "NIVEL1":
                    Nivel = "NIVEL2";
                    NIVEL1.Enabled = false;
                    NIVEL2.Enabled = true;
                    NIVEL3.Enabled = false;
                    NIVEL4.Enabled = false;
                    NIVEL5.Enabled = false;
                    AddNode.Enabled = true;

                    break;
                case "NIVEL2":
                    Nivel = "NIVEL3";
                    NIVEL1.Enabled = false;
                    NIVEL2.Enabled = false;
                    NIVEL3.Enabled = true;
                    NIVEL4.Enabled = false;
                    NIVEL5.Enabled = false;
                    AddNode.Enabled = true;

                    break;
                case "NIVEL3":
                    Nivel = "NIVEL4";
                    NIVEL1.Enabled = false;
                    NIVEL2.Enabled = false;
                    NIVEL3.Enabled = false;
                    NIVEL4.Enabled = true;
                    NIVEL5.Enabled = false;
                    AddNode.Enabled = true;

                    break;
                case "NIVEL4":
                    Nivel = "NIVEL5";
                    NIVEL1.Enabled = false;
                    NIVEL2.Enabled = false;
                    NIVEL3.Enabled = false;
                    NIVEL4.Enabled = false;
                    NIVEL5.Enabled = true;
                    AddNode.Enabled = true;

                    break;
                case "NIVEL5":
                    Nivel = "NIVEL1";
                    NIVEL1.Enabled = false;
                    NIVEL2.Enabled = false;
                    NIVEL3.Enabled = false;
                    NIVEL4.Enabled = false;
                    NIVEL5.Enabled = false;
                    AddNode.Enabled = false;
                    break;

            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            TreeNode nNode = new TreeNode(ValorNode);
            nNode.ImageIndex = Imagem;
            nNode.SelectedImageIndex = Imagem;
            //nNode.ImageKey = Imagem;
            nNode.Tag = Nivel;
           
            if (Selecao != null)
            {
                
                Selecao.Nodes.Add(nNode);
                Selecao.Expand();
            }
            else
            {
                //treeView1.Nodes.Add(nNode);
                
            }
        }

        private void NIVEL5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void NIVEL5_TextChanged(object sender, EventArgs e)
        {
            ComboBox xt = (ComboBox)sender;
            if(xt.Enabled)
            {
                ValorNode = xt.Text;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if(Selecao!=null)
            {
                if(Selecao.Text !="Raiz")
                {
                    if (Funcoes_Form.mensagem("Tem certeza que deseja excluir o nível " + Selecao.Text + " e todos os subníveis?"))
                    {
                        treeView1.Nodes.Remove(Selecao);
                    }
                }
           
            }
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            //if (Imagem < IconesTree.Images.Count-1)
            //{
            //    Imagem = Imagem + 1;
            //}
            //else
            //{
            //    Imagem = 0;
            //}
            //pictureBox1.Image = IconesTree.Images[Imagem];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //if (Imagem >0)
            //{
            //    Imagem = Imagem - 1;
            //}
            //else
            //{
            //    Imagem = 0;
            //}
            //pictureBox1.Image = IconesTree.Images[Imagem];
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count>0)
            {
                Imagem = listView1.SelectedItems[0].Index;
                pictureBox1.Image = Program.Menu.IconesTree.Images[listView1.SelectedItems[0].Index];
            }
        }

        private void button2_Click_3(object sender, EventArgs e)
        {
            if(Funcoes_Form.mensagem("Você tem certeza?"))
            {
                ArvoreXML x = new ArvoreXML();
                x.ArvoreParaXML(treeView1, Vars.Arvore);
                MessageBox.Show("Alteração Salva.");
            }
        
        }

        private void Nome_Click(object sender, EventArgs e)
        {
           
        }

        private void Nome_TextChanged(object sender, EventArgs e)
        {
            Forms.AlimentaCombo(Combo2, Arquivo_Pasta.ler(Vars.DirCfgs + Nome.Text + ".db"),true);
        }

        private void EditarNode_Click(object sender, EventArgs e)
        {
            if(Funcoes_Form.mensagem("Você tem certeza que deseja alterar o nome de " + treeView1.SelectedNode.Text + "\nPara: " + ComboEditNodes.Text + "?"))
            {
                treeView1.SelectedNode.Text = ComboEditNodes.Text;
            }
        }

        private void adicionarArquivosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void Cadastro_FormClosing(object sender, FormClosingEventArgs e)
        {
            Vars.Buffer.Banco = Vars.Conexao.Consulta(new Celula("Ativo", "Sim"), false, "plm", "padronizacao");
            Program.Menu.Resultado.Items.Clear();
        }

        private void importarCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            
        }

        private void atualizarNaDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

          
        }

        private void abrirArquivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> arquivo = Arquivo_Pasta.abrir("csv", "Defina o arquivo");
            if (arquivo.Count > 0)
            {
                List<string> Cabs = Vars.Conexao.GetColunas("plm", "padronizacao");
                Importar.Rows.Clear();
                Importar.Columns.Clear();
                List<string> Cab = arquivo[0].Split(';').ToList();

                foreach(string C in Cab)
                {
                    if(Cabs.Find(x=>x.ToLower()==C.ToLower())==null)
                    {
                        MessageBox.Show("Coluna " + C + " não existe na tabela do banco. Operação abortada.");
                        return;
                    }
                }

                List<int> Tratar = new List<int>();

                for (int i = 0; i < Cab.Count(); i++)
                {
                    if(Vars.Numericos().Find(x=>x.ToLower() == Cab[i].ToLower())!=null)
                    {
                        Tratar.Add(i);
                    }
                }
                List<List<string>> Final = new List<List<string>>();
                for (int i = 1; i < arquivo.Count; i++)
                {
                    Final.Add(arquivo[i].Split(';').ToList());
                }

                foreach (int xt in Tratar)
                {
                    for (int i = 0; i < Final.Count(); i++)
                    {
                        Final[i][xt] = Final[i][xt].Replace(",", ".");
                    }
                }


      
                foreach (string s in Cab)
                {
                    Importar.Columns.Add(s, s);
                }

                for (int i = 1; i < Final.Count; i++)
                {
                    Importar.Rows.Add(Final[i].ToArray());
                }

            }
        }

        private void importarNoBancoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Funcoes_Form.mensagem("Tem certeza que deseja atualizar " + Importar.Rows.Count + " itens?"))
            {
                Tela_Load w = new Tela_Load();
                w.Progresso.Maximum = Importar.Rows.Count;
                w.Progresso.Value = 0;
                w.Show();
                w.Update();
                int chave_nome = 0;
                bool achei = false;
                for (int i = 0; i < Importar.Columns.Count; i++)
                {
                    if (Importar.Columns[i].HeaderText.ToLower() == "nome")
                    {
                        chave_nome = i;
                        achei = true;
                        break;
                    }
                }
                if (achei == false)
                {
                    MessageBox.Show("Não foi encontrado o item 'NOME' no csv importado.\n Operação abortada.");
                    return;
                }
                foreach (DataGridViewRow L in Importar.Rows)
                {
                    w.Progresso.Value = w.Progresso.Value + 1;
                    w.Mensagem.Text = Funcoes.Porcentagem(w.Progresso.Value,w.Progresso.Maximum,2) + "%";
                    List<DB.Celula> Edicoes = new List<DB.Celula>();
                    for (int i = 1; i < L.Cells.Count; i++)
                    {
                        Edicoes.Add(new DB.Celula(L.Cells[i].OwningColumn.HeaderText, L.Cells[i].Value.ToString()));
                    }
                    Vars.Conexao.Update(new List<Celula> { new Celula("NOME", L.Cells[chave_nome].Value.ToString()) }, Edicoes,Vars.nome_db,Vars.nome_tb);
                  

                }
                w.Close();
                MessageBox.Show("Itens atualizados.");
            }
        }

        private void adicionarArquivosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(Importar.Rows.Count>0)
            {
                if(Funcoes_Form.mensagem("Ao continuar em sim, a lista atual será limpa. Deseja continuar?"))
                {
                    Importar.Rows.Clear();
                    Importar.Columns.Clear();
                    Importar.Columns.Add("NOME", "NOME");
                    Importar.Columns.Add("DIR", "DIR");
                    Importar.Columns.Add("EXTENSAO", "EXTENSAO");
                }
                else
                {
                    return;
                }
            }
 

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Selecione o(s) arquivo(s)";
            op.Multiselect = true;
            DialogResult dr = op.ShowDialog();
            if (dr == DialogResult.OK)
            {
                foreach (string Arq in op.FileNames)
                {
                    string nome = Arquivo_Pasta.info_nome(Arq);
                    if (Vars.Extensoes().Find(x => x.ToLower() == Arquivo_Pasta.info(Arq).Extension.ToLower().Replace(".", "")) != null)
                    {
                        Importar.Rows.Add(nome, Arq, Arquivo_Pasta.info(Arq).Extension.ToUpper().Replace(".", ""));
                    }
                }

            }
        }

        private void adicionarPastasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Funcoes_Form.mensagem("Tem certeza que deseja importar os " + Importar.Rows.Count + " itens?"))
            {
               

                List<List<string>> Lista = Funcoes_Form.datagrid_para_lista(Importar);
                DB.Tabela tb = new DB.Tabela();

                DB.Tabela Resultado = Vars.Conexao.Consulta(new Celula("NOME", ""), false, "plm", "padronizacao");
                List<string> Atuais = Resultado.Linhas.SelectMany(x => x.Celulas.FindAll(z => z.Coluna == "NOME").Select(y => y.Valor)).Distinct().ToList();


                Lista = Lista.FindAll(x => Atuais.FindAll(y => y == x[0]).Count == 0);
                if (Funcoes_Form.mensagem("Foram encontrados " + Lista.Count + " itens novos. Deseja continuar?"))
                {
                    string Ativo = "Nao";
                    if (Funcoes_Form.mensagem("Deseja ativar as peças importadas para acesso?"))
                    {
                        Ativo = "Sim";
                    }
                    foreach (List<string> L in Lista)
                    {
                        DB.Linha N = new DB.Linha("padronizacao");
                        N.Celulas.Add(new Celula("NOME", L[0]));
                        N.Celulas.Add(new Celula("DIR", L[1].Replace(@"\", "|")));
                        N.Celulas.Add(new Celula("EXTENSAO", L[2]));
                        N.Celulas.Add(new Celula("Ativo", Ativo));
                        tb.Linhas.Add(N);
                    }
                    Tela_Load w = new Tela_Load();
                    w.Show();
                    w.Update();
                    Vars.Conexao.Cadastro(tb.Linhas, Vars.nome_db, Vars.nome_tb);
                    w.Close();
                    Importar.Rows.Clear();
                    MessageBox.Show("Importação finalizada.");
                }

            }
        }

        private void Combo2_Click(object sender, EventArgs e)
        {

        }

        private void exportarEmCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string arqv = Arquivo_Pasta.salvar("csv", "Selecione o destino");
            if (arqv != "")
            {
                List<List<string>> lista = new List<List<string>>();
                List<string> Cab = new List<string>();
                foreach (DataGridViewColumn cx in Importar.Columns)
                {
                    Cab.Add(cx.HeaderText);
                }
                lista.Add(Cab);

                lista.AddRange(Funcoes_Form.datagrid_para_lista(Importar));
                Buffer_Texto.gravar_arquivo_csv(arqv, lista);

                MessageBox.Show("Arquivo salvo.");
            }
        }

        private void mostrarArquivosQuebradosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tela_Load w = new Tela_Load();
            w.Show();
            w.Update();

            DB.Tabela Resultado = Vars.Conexao.Consulta(new Celula("Ativo", ""), false, "plm", "padronizacao");
            List<DB.Linha> Filtro = new List<Linha>();

            Resultado.Linhas = Resultado.Linhas.FindAll(x => File.Exists(x.Celulas.Find(Y => Y.Coluna == "DIR").Valor.Replace("|", @"\"))==false);
            Resultado.AlimentaDataGrid(CadastroGrid);
            w.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
               Vars.Conexao.Consulta(new Celula(Consulta_Log_Combo.Text, Consulta_Log_Texto.Text), false, "plm", "padronizacao_log").AlimentaDataGrid(Log_Acesso);
               
            }
        }

        private void Consulta_Log_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            string arqv = Arquivo_Pasta.salvar("csv", "Selecione o destino");
            if (arqv != "")
            {
                List<List<string>> lista = new List<List<string>>();
                List<string> Cab = new List<string>();
                foreach (DataGridViewColumn cx in Log_Acesso.Columns)
                {
                    Cab.Add(cx.HeaderText);
                }
                lista.Add(Cab);

                lista.AddRange(Funcoes_Form.datagrid_para_lista(Log_Acesso));
                Buffer_Texto.gravar_arquivo_csv(arqv, lista);

                MessageBox.Show("Arquivo salvo.");
            }
        }

        private void Consulta_Log_Texto_TextChanged(object sender, EventArgs e)
        {

        }

        private void Diretorio_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                AdicionarPastas();
            }
        }
    }
}
