using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manual_Padronizacao
{
    static class Program
    {
        public static Principal Menu;
        public static Cadastro Cadastro;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            Menu = new Principal();


            //DateTime T = new DateTime(2019, 08, 30);
            //DateTime agora = DateTime.Now;
            //if (agora > T)
            //{
            //    MessageBox.Show("Aplicativo desatualizado. Contacte suporte\nDaniel Lins Maciel");
            //    Application.Exit();
            //}
            //else
            //{
            //    Application.Run(Menu);
            //}
            Application.Run(Menu);

        }
    }
}
