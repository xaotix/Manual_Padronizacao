using Biblioteca_Daniel;
using Conexoes;
using DLM.db;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Manual_Padronizacao
{
    public class Forms
    {
        
        public static void AlimentaCombo(Control Controle,bool preselecionar=false)
        {
            foreach (Control cx in Controle.Controls)
            {
                if (cx is ComboBox)
                {
                    ComboBox com = (ComboBox)cx;
                    com.GotFocus += Program.Menu.PegaFocus;
                    com.LostFocus += Program.Menu.PerdeFocus;
                    Forms.AlimentaCombo(com, Arquivo_Pasta.ler(Vars.DirCfgs + cx.Name + ".db"),preselecionar);
                }
                else
                {
                    AlimentaCombo(cx,preselecionar);
                }
            }

        }
        public static void Status(String Nome, Control Raiz,bool St)
        {
            foreach(Control cx in Raiz.Controls)
            {
                if(cx.Name.ToLower() ==Nome.ToLower())
                {
                    cx.Enabled = St;
                }
                else
                {
                    Status(Nome, cx, St);
                }
            }
        }
        
        public static void GetComboxAtivos(List<Celula> Celulas,Control Controle)
        {
            foreach(Control tc in Controle.Controls)
            {
                if(tc is ComboBox)
                {
                    if(tc.Enabled)
                    {
                        Celulas.Add(new Celula(tc.Name, tc.Text));
                    }                 
                }
                else
                {
                    GetComboxAtivos(Celulas, tc);
                }
            }
        }

        public static void AlimentaCombo(ToolStripComboBox combo, List<string> Chaves, bool selecionar = false)
        {
            combo.Text = "";
            combo.Items.Clear();
            foreach (string t in Chaves)
            {
                combo.Items.Add(t);
            }
            if (combo.Items.Count > 0 && selecionar)
            {
                combo.SelectedIndex = 0;
            }
        }
        public static void AlimentaCombo(ComboBox combo, List<string> Chaves,bool selecionar = false,bool organizar=false)
        {
            Chaves = Chaves.Distinct().ToList();
            if(Chaves.Count>0)
            {
                if (organizar)
                {
                    Chaves.Sort();
                }

                combo.Text = "";
                combo.Items.Clear();
                foreach (string t in Chaves)
                {
                    combo.Items.Add(t);
                }
                if (combo.Items.Count > 0 && selecionar)
                {
                    combo.SelectedIndex = 0;
                }
            }
            
        }
    }
    public class Funcoes
    {
        public static void ExportaCSV(DataGridView Grid)
        {
            string arqv = Arquivo_Pasta.salvar("csv", "Selecione o destino");
            if (arqv != "")
            {
                List<List<string>> lista = new List<List<string>>();
                List<string> Cab = new List<string>();
                foreach (DataGridViewColumn cx in Grid.Columns)
                {
                    Cab.Add(cx.HeaderText);
                }
                lista.Add(Cab);

                lista.AddRange(Funcoes_Form.datagrid_para_lista(Grid));
                Buffer_Texto.gravar_arquivo_csv(arqv, lista);

                if (Funcoes_Form.mensagem("Arquivo Salvo. Deseja abri-lo?"))
                {
                    Abrir(arqv);
                }
            }
        }
        public static void Abrir(string Arquivo)
        {
            DuplicanoTemp(Arquivo).Abrir();
        }

        public static string DuplicanoTemp(string Arquivo)
        {
            var arquivo = new Conexoes.Arquivo(Arquivo);
            var prefix = DLM.vars.Cfg.Init.TMP_Manual() + arquivo.Nome;
            string ArqTemp = prefix + "." + arquivo.Extensao;
            int c = 0;
            while (!Conexoes.Utilz.Copiar(Arquivo, ArqTemp, false))
            {
                ArqTemp = prefix + "_" + c + "." + arquivo.Extensao;
                c++;
            }
            return ArqTemp;
        }


        public static void Matar(string Executavel)
        {
            foreach(Process P in Process.GetProcesses())
            {
                if(P.ProcessName.Contains(Executavel))
                {
                    try
                    {
                        P.Kill();

                    }
                    catch (Exception ex)
                    {

                        System.Windows.MessageBox.Show("Algo de errado aconteceu " + ex.Message);
                    }
                }
            }
        }

        public static void SetPNG(Arquivo Arq, PictureBox Preview)
        {
            var arqpng = Arq.Nome + ".png";
            var png = Arq.Pasta + arqpng;
            var png2 = DLM.vars.Cfg.Init.png_dir + arqpng;
            var png_tmp = DLM.vars.Cfg.Init.PNG_Manual() + Arq.Nome + ".png";



            if (png2.Existe())
            {
                int c = 1;
                while (!Conexoes.Utilz.Copiar(png2, png_tmp, false))
                {
                    png_tmp = DLM.vars.Cfg.Init.PNG_Manual() + Arq.Nome + "_" + c + ".png";
                    c++;
                }
            }


            if (png_tmp.Existe())
            {

                Preview.Image = System.Drawing.Image.FromFile(png_tmp);
                Preview.Image.Tag = Arq.Nome;
            }
            else
            {
                Preview.Image = Properties.Resources.semimagem;
            }
        }

        public static double Porcentagem(double Valor, double Total, int Decimais = 2)
        {
            return Math.Round(Convert.ToDouble((Valor / Total) * 100), Decimais);
        }
    }
}
