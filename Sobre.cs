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
    public partial class Sobre : Form
    {
        public Sobre()
        {
            InitializeComponent();
        }

        private void Sobre_Load(object sender, EventArgs e)
        {
            label1.Text = Application.ProductName + "\n v." + Application.ProductVersion;
        }
    }
}
