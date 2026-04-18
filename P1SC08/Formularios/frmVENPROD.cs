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
    public partial class frmVENPROD : Form
    {
        public frmVENPROD()
        {
            InitializeComponent();
        }

        private void frmVENPROD_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        private void frmVENPROD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)  // estoy preguntando si la tecla que presionaste es igual a 27 (es igual tecla ESC)
            {
                this.Close();  // Cierra el formulario 
            }
        }
    }
}
