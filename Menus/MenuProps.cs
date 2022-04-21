using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Biblioteca_Daniel;

namespace Manual_Padronizacao
{
    public partial class MenuProps : UserControl
    {
        public MenuProps()
        {
            InitializeComponent();
        }

        private void MenuProps_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            this.Dock = DockStyle.Fill;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Funcoes_Form.pesquisar_datagrid(dataGridView1, textBox1.Text,comboBox1.SelectedIndex);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Funcoes.ExportaCSV(dataGridView1);
        }
    }
}
