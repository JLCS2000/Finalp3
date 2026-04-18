using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P1SC08
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            this.Text = "Menu General"; // cambia el texto en momento de ejecucion

            this.KeyPreview = true;  // activa la primera linea del teclado (funciones)
        }

        private void frmMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode  == Keys.Escape) // pregunta si presionaste la tecla de escape
            {
                Application.Exit();  // cierra la aplicacion completa
            }
        }

        private void productoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmProductos frm = new frmProductos();
            frm.Show();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();  // cierra la aplicacion completa
        }
    }
}
