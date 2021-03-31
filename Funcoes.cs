using Biblioteca_Daniel;
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
        
        public static void GetComboxAtivos(List<DB.Celula> Celulas,Control Controle)
        {
            foreach(Control tc in Controle.Controls)
            {
                if(tc is ComboBox)
                {
                    if(tc.Enabled)
                    {
                        Celulas.Add(new DB.Celula(tc.Name, tc.Text));
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
        

            try
            {
                System.Diagnostics.Process.Start(DuplicanoTemp(Arquivo));
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        public static string DuplicanoTemp(string Arquivo)
        {
            string ArqTemp = Vars.PastaTmp + "dlm" + RandomString(5)  + Arquivo_Pasta.info(Arquivo).Extension;
            int c = 0;
            while (File.Exists(ArqTemp))
            {
                try
                {
                    File.Delete(ArqTemp);
                }
                catch (Exception)
                {
                    ArqTemp = Vars.PastaTmp + "dlm" + RandomString(5)  + Arquivo_Pasta.info(Arquivo).Extension;
                    c++;
                    //throw;
                }
            }
            File.Copy(Arquivo, ArqTemp);
            return ArqTemp;
        }

        public static void Preview(string Arquivo)
        {
            string ArquivoFim = Arquivo.Replace("|", @"\");
            if(File.Exists(ArquivoFim))
            {
                string Ext = Arquivo_Pasta.info(ArquivoFim).Extension.Replace(".", "").ToLower();
                 if (Vars.ExtensoesAbrir().Find(x => x == Ext) != null)
                {
                    Abrir(Arquivo);
                }
                else if (Vars.ExtensoesOffice().Find(x => x == Ext) != null)
                {
                    //Visualizador.MainWindow.Abrir(ArquivoFim);
                }
                else if (Vars.ExtensoesDXF().Find(x => x == Ext) != null)
                {
                    if(Convert.ToBoolean(Funcoes_ini.ler_ini(Vars.cfguser,"Geral","CAD","False")))
                    {
                        Abrir(Arquivo);

                    }
                    else
                    {
                    AbrirDXF(Arquivo);

                    }
                }
            
                else
                {
                    System.Windows.Forms.MessageBox.Show("Não é possível pré-visualizar o arquivo " + Arquivo + " formato não suportado.");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("O arquivo " + Arquivo + " não existe.");
            }
        }
        public static void AbrirDXF(string Arquivo)
        {
            if(File.Exists(Arquivo))
            {
                try
                {
                    //Matar(Vars.ExecDXF);
                    Process nDxf = new Process();
                    nDxf.StartInfo.WorkingDirectory = System.Windows.Forms.Application.StartupPath;
                    nDxf.StartInfo.FileName = Vars.ExecDXF + ".exe";
                    nDxf.StartInfo.Arguments = "\"" + DuplicanoTemp(Arquivo) + "\"";
                    nDxf.Start();
                }
                catch (Exception)
                {

                    throw;
                }
              
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("O arquivo " + Arquivo + " não existe.");
            }
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
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefgijklmnopqrstuvxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static double Porcentagem(double Valor, double Total, int Decimais = 2)
        {
            return Math.Round(Convert.ToDouble((Valor / Total) * 100), Decimais);
        }
    }
}
