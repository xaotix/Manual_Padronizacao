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
    public partial class ImageZoom : Form
    {
        //ZoomImagem Visualizar;
        public ImageZoom()
        {
            InitializeComponent();        
        }

        public void Abrir(string Arquivo)
        {
            try
            {
                pictureBox1.Image = new Bitmap(Arquivo);
                //Visualizar.Reset();

            }
            catch (Exception)
            {

                //throw;
            }
          
        }

        private void ImageZoom_Load(object sender, EventArgs e)
        {
            //Visualizar = new ZoomImagem(pictureBox1);            
        }

    }
}
