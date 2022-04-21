using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Threading;
using DLM.db;
using Biblioteca_Daniel;

namespace Manual_Padronizacao
{
   public  class Pesquisa
    {
        private static ToolStripStatusLabel status;
        private static ProgressBar Progresso;
        private static SplitContainer SplitPesquisa;
        private static List<Linha> Linhas = new List<Linha>();
        public static void Add(List<Linha> Linhas, System.Windows.Forms.ListView Lista, System.Windows.Forms.ImageList Imagens, ToolStripStatusLabel status,ProgressBar Progresso, SplitContainer SplitPesquisa, bool Limpar=true)
        {
            Pesquisa.Progresso = Progresso;
            Pesquisa.status = status;
            Pesquisa.SplitPesquisa = SplitPesquisa;
            Progresso.Value = 0;
            Progresso.Maximum = Linhas.Count();


            if (Linhas.Count > Vars.AlertaMax)
            {
                if (Funcoes_Form.mensagem("A pesquisa selecionada resultou em mais de " + Vars.AlertaMax + " itens. Isto pode demorar um pouco. Deseja continuar?"))
                {
                    SplitPesquisa.Visible = false;
                }
                else
                {
                    SplitPesquisa.Visible = true;

                    return;
                }
            }

            if (Linhas.Count > 100)
            {
                SplitPesquisa.Visible = false;
            }
            SplitPesquisa.Visible = false;

            status.Text = "Adicionando " + Linhas.Count + " itens, aguarde...";
            Program.Menu.Update();
            if (Limpar==true)
            {
                Lista.Items.Clear();
            }

            List<Task> Tarefas = new List<Task>();

            Pesquisa.Linhas = Linhas;
            foreach (DLM.db.Linha L in Linhas)
            {
                status.Text = "Adicionando " + Linhas.Count + " itens, aguarde... " + Funcoes.Porcentagem(Progresso.Value, Linhas.Count(), 0) + "%";
                status.GetCurrentParent().Update();
                Progresso.Value = Progresso.Value + 1;

                AddNaLista(L, Lista, Imagens);        

            }

            status.Text = "Mostrando " + Linhas.Count + " itens.";

            SplitPesquisa.Visible = true;

        }
        public static void AddNaLista(DLM.db.Linha Linha, System.Windows.Forms.ListView Lista, System.Windows.Forms.ImageList Imagens)
        {
            string Arq = Linha.Celulas.Find(x => x.Coluna == "DIR").Valor.Replace("|",@"\");
            if (Vars.SubstituirRepositorio)
            {                
                Arq = Arq.Replace(Vars.Repositorio()[0], Vars.Repositorio()[1]);
            }
            if (File.Exists(Arq))
            {
                string Nome = Linha.Get("NOME").Valor;
                Icon icone = SystemIcons.WinLogo;
                DirectoryInfo info = Arquivo_Pasta.info(Arq);
                if (Imagens.Images.ContainsKey(info.Extension.Replace(".","")) == false)
                {
                  if (Imagens.Images.ContainsKey(info.Extension.Replace(".", "") + ".png") == false)
                    {
                        try
                        {
                            icone = System.Drawing.Icon.ExtractAssociatedIcon(Arq);
                            Imagens.Images.Add(info.Extension.Replace(".", ""), icone);
                        }
                        catch (Exception)
                        {


                        }
                    }
                   
                  
                }

                List<string> Valores = new List<string>();
                foreach (ColumnHeader Col in Lista.Columns)
                {
                    //nItem.SubItems.Add(Linha.Celulas.Find(x => x.Coluna == Col.Text.ToUpper()).Valor);
                    Valores.Add(Linha[Col.Text].Valor);
                }
                ListViewItem nItem = new ListViewItem(Valores.ToArray());

                nItem.Tag = Arq;
                if ( Imagens.Images.ContainsKey(info.Extension.Replace(".", "") + ".png"))
                {
                    nItem.ImageKey = info.Extension.Replace(".", "") + ".png";           

                }
                else
                {
                    nItem.ImageKey = info.Extension.Replace(".", "");

                }

                Lista.Items.Add(nItem);
            }

        }
        public static List<DLM.db.Linha> Filtrar(List<DLM.db.Linha> Total, List<DLM.db.Celula> Filtro, bool Exato)
        {
            List<DLM.db.Linha> Retorno = new List<DLM.db.Linha>();
            Retorno.AddRange(Total);
            foreach (DLM.db.Celula Cl in Filtro)
            {
                List<string> Valores = Cl.Valor.Split(Vars.SeparadorPesquisa.ToCharArray()).ToList();
                if (Exato)
                {
                    if (Valores.Count == 1)
                    {
                        Retorno = Retorno.FindAll(x => x.Celulas.FindAll(y => y.Coluna.ToLower() == Cl.Coluna.ToLower() && y.Valor.ToLower() == Cl.Valor.ToLower()).Count > 0);

                    }
                    else
                    {
                        Retorno = Retorno.FindAll(x => x.Celulas.FindAll(y => y.Coluna.ToLower() == Cl.Coluna.ToLower()).Count > 0);
                        List<DLM.db.Linha> FiltroSP = new List<DLM.db.Linha>();
                        FiltroSP.AddRange(Retorno);
                        Retorno.Clear();
                        foreach (DLM.db.Linha L in FiltroSP)
                        {
                            foreach (string Valor in Valores)
                            {

                                if (L.Celulas.FindAll(x => x.Coluna.ToLower() == Cl.Coluna.ToLower() && x.Valor.ToLower() == Valor.ToLower()).Count > 0)
                                {
                                    if (Retorno.Find(x => x.Valores().SequenceEqual(L.Valores())) == null)
                                    {
                                        Retorno.Add(L);
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    if (Valores.Count == 1)
                    {
                        Retorno = Retorno.FindAll(x => x.Celulas.FindAll(y => y.Coluna.ToLower() == Cl.Coluna.ToLower() && y.Valor.ToLower().Contains(Cl.Valor.ToLower())).Count > 0);

                    }
                    else
                    {
                        Retorno = Retorno.FindAll(x => x.Celulas.FindAll(y => y.Coluna.ToLower() == Cl.Coluna.ToLower()).Count > 0);
                        List<DLM.db.Linha> FiltroSP = new List<DLM.db.Linha>();
                        FiltroSP.AddRange(Retorno);
                        Retorno.Clear();
                        foreach (DLM.db.Linha L in FiltroSP)
                        {
                            foreach (string Valor in Valores)
                            {

                                if (L.Celulas.FindAll(x => x.Coluna.ToLower() == Cl.Coluna.ToLower() && x.Valor.ToLower().Contains(Valor.ToLower())).Count > 0)
                                {
                                    if (Retorno.Find(x => x.Valores().SequenceEqual(L.Valores())) == null)
                                    {
                                        Retorno.Add(L);
                                    }
                                }
                            }

                        }
                    }
                }
            }

            return Retorno;
        }



        public static List<DLM.db.Linha> Filtrar(List<DLM.db.Linha> Total, string Chave, bool Exato)
        {
   

           return Total.FindAll(x => Contem(string.Join(" ", x.Celulas.Select(y => y.Valor).ToArray()), Chave, Exato ? 100 : 70));

        }



        public static bool Contem(object item, string valor, double porcentagem = 70)
        {
            if (valor == "") { return true; }
            else
            {
                if (String.IsNullOrEmpty(valor))
                    return true;

                var pirate = item;

                if (pirate.ToString().ToUpper().Contains(valor.ToUpper()))
                {
                    return true;
                }
                else
                {
                    string[] chaves = valor.Replace("  ", " ").Split(' ').ToList().FindAll(y => y.Replace(" ", "").Count() > 2).ToArray();
                    int cc = 0;
                    foreach (string chave in chaves)
                    {
                        if (pirate.ToString().ToUpper().Contains(chave.ToUpper()))
                        {
                            cc++;
                        }

                    }

                    if (cc > 0)
                    {
                        double x = 100 * cc / chaves.Count();
                        return (x >= porcentagem);
                    }
                    else
                    {
                        return false;
                    }

                }
            }
        }
    }
}
