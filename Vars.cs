using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using Biblioteca_Daniel;
using System.Windows.Forms;

namespace Manual_Padronizacao
{
    public class Vars
    {
        public static int AlertaMax = 1000;
        public static bool SubstituirRepositorio = true;
        public class Ext
        {
            public static string Pesquisa = "Pdlm";
        }
        public class Buffer
        {
            public static ArvoreXML tree = new ArvoreXML();
            public static DB.Tabela Banco;
            public static List<DB.Linha> FiltroTree = new List<Linha>();
            public static List<DB.Linha> PesquisaUser = new List<Linha>();
        }
        public static string SeparadorPesquisa = "|";
        public static string SeparadorChaves = "*";


        public class Niveis
        {
            public static List<string> Nivel1 = Arquivo_Pasta.ler(DirCfgs + "nivel1.db");
            public static List<string> Nivel2 = Arquivo_Pasta.ler(DirCfgs + "nivel2.db");
            public static List<string> Nivel3 = Arquivo_Pasta.ler(DirCfgs + "nivel3.db");
            public static List<string> Nivel4 = Arquivo_Pasta.ler(DirCfgs + "nivel4.db");
            public static List<string> Nivel5 = Arquivo_Pasta.ler(DirCfgs + "nivel5.db");
            public static List<string> Ativo = Arquivo_Pasta.ler(DirCfgs + "Ativo.db");
            public static List<string> Tipos = Arquivo_Pasta.ler(DirCfgs + "tipos.db");
           
        }
        public static string DirCfgs = Application.StartupPath + @"\Cfg\";
        public static string Dir = Application.StartupPath + @"\";
        public static string ExtensoesArq = DirCfgs + "extensao.db";
        public static string Arvore = DirCfgs + "Arvore.dlm";
        public static string ArvoreUser = Dir + "ArvoreUser.dlm";
        public static string DXF = DirCfgs + "DXF.db";
        public static string Office = DirCfgs + "Office.db";
        public static string CfgUser = Dir + "Usuario.Cfg";
        public static string ExecDXF = "VisualizadorDXF";
        public static string cfguser = Vars.Dir + "cfguser.ini";

        public static List<string> Extensoes()
        {
            return Arquivo_Pasta.ler(ExtensoesArq);
        }
        public static List<string> Repositorio()
        {
            return Arquivo_Pasta.ler(DirCfgs + "repositorio.db");
        }
        public static List<string> ExtensoesOffice()
        {
            return Arquivo_Pasta.ler(Office);
        }
        public static List<string> ExtensoesDXF()
        {
            return Arquivo_Pasta.ler(DXF);
        }
        public static List<string> ExtensoesAbrir()
        {
            return Arquivo_Pasta.ler(DirCfgs + "ExtensoesAbrir.db");
        }


        public static List<string> Palavras_Genericas()
        {
            return Arquivo_Pasta.ler(DirCfgs + "Palavras_Genericas.db");
        }
        public static List<string> Numericos()
        {
            return Arquivo_Pasta.ler(DirCfgs + "numericos.db");
        }
        public static DB.Banco Conexao { get; set; }
        public static string PaginaNews { get; set; } = Vars.DirCfgs + "news.mht";
        public static string PastaTmp { get; set; } = Dir + @"\Temp\";
        public static string PNGDir { get; set; } = @"\\nbvmsfs04\08b2606cbd462954a1ded3d55c3e4023$\PNG\";

        public static string nome_db { get; set; } = "plm";
        public static string nome_tb { get; set; } = "padronizacao";
    }
}
