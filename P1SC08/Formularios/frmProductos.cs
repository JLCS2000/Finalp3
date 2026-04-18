using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace P1SC08
{
    public partial class frmProductos : Form
    {
        private bool _dataExiste = false;

        public frmProductos()
        {
            InitializeComponent();
        }

        private void frmProductos_Load(object sender, EventArgs e)
        {
            this.Text = "Maestro de Productos";
            this.KeyPreview = true;
            textBox1.Text = Busco.BuscaUltimoNumero("1");
            _dataExiste = false;
        }

        private void frmProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void textBox9_TextChanged(object sender, EventArgs e) { }
        private void txtCosto_TextChanged(object sender, EventArgs e) { }

        private void button5_Click(object sender, EventArgs e)
        {
            frmVENPROD frm = new frmVENPROD();
            frm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == string.Empty)
            { MessageBox.Show("El código del producto es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox1.Focus(); return; }
            if (txtDescripcion.Text.Trim() == string.Empty)
            { MessageBox.Show("La descripción es requerida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDescripcion.Focus(); return; }
            if (txtCantidad.Text.Trim() == string.Empty)
            { MessageBox.Show("La cantidad es requerida.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtCantidad.Focus(); return; }
            if (txtCosto.Text.Trim() == string.Empty)
            { MessageBox.Show("El costo es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtCosto.Focus(); return; }
            if (txtVenta.Text.Trim() == string.Empty)
            { MessageBox.Show("El precio de venta es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtVenta.Focus(); return; }
            if (textBox4.Text.Trim() == string.Empty)
            { MessageBox.Show("El impuesto es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox4.Focus(); return; }

            if (_dataExiste == false) InsertarData();
            else ActualizarData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            textBox1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_dataExiste == false)
            { MessageBox.Show("No hay producto cargado para borrar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            DialogResult dialogResult = MessageBox.Show("¿Borrar el producto " + textBox1.Text + "?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                BorrarData(textBox1.Text.Trim());
                LimpiarFormulario();
                MessageBox.Show("Producto eliminado.", "Sistema Contable", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button4_Click(object sender, EventArgs e) { this.Close(); }

        private void LimpiarFormulario()
        {
            textBox1.Clear(); txtDescripcion.Clear(); txtCantidad.Clear();
            txtCosto.Clear(); txtVenta.Clear(); textBox4.Clear(); textBox9.Clear();
            _dataExiste = false;
            textBox1.Text = Busco.BuscaUltimoNumero("1");
        }

        private void BuscarProducto(string numProducto)
        {
            _dataExiste = false;
            string query = "SELECT ITEM,DESCRIPCION,CANTIDADENEXISTENCIA,COSTO,PRECIODEVENTA,IMPUESTO,BARCODE FROM PRODUCTOS WHERE ITEM=@ITEM AND EstatusProducto=1";
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnx))
                {
                    cmd.Parameters.AddWithValue("@ITEM", numProducto);
                    using (SqlDataReader rsd = cmd.ExecuteReader())
                    {
                        if (rsd.Read())
                        {
                            _dataExiste = true;
                            textBox1.Text = Convert.ToString(rsd["ITEM"]);
                            txtDescripcion.Text = Convert.ToString(rsd["DESCRIPCION"]);
                            txtCantidad.Text = Convert.ToString(rsd["CANTIDADENEXISTENCIA"]);
                            txtCosto.Text = Convert.ToString(rsd["COSTO"]);
                            txtVenta.Text = Convert.ToString(rsd["PRECIODEVENTA"]);
                            textBox4.Text = Convert.ToString(rsd["IMPUESTO"]);
                            textBox9.Text = Convert.ToString(rsd["BARCODE"]);
                        }
                        else { MessageBox.Show("Producto no encontrado.", "Sistema Contable", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                    }
                }
            }
        }

        private void BorrarData(string numProducto)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM PRODUCTOS WHERE ITEM=@ITEM", cnx))
                { cmd.Parameters.AddWithValue("@ITEM", numProducto); cmd.ExecuteNonQuery(); }
            }
        }

        private void InsertarData()
        {
            string query = "INSERT INTO PRODUCTOS(ITEM,DESCRIPCION,CANTIDADENEXISTENCIA,COSTO,PRECIODEVENTA,BARCODE,ESTATUSPRODUCTO,IMPUESTO) VALUES(@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8)";
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnx))
                {
                    cmd.Parameters.AddWithValue("@A1", textBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@A2", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@A3", txtCantidad.Text.Trim());
                    cmd.Parameters.AddWithValue("@A4", txtCosto.Text.Trim());
                    cmd.Parameters.AddWithValue("@A5", txtVenta.Text.Trim());
                    cmd.Parameters.AddWithValue("@A6", textBox9.Text.Trim());
                    cmd.Parameters.AddWithValue("@A7", 1);
                    cmd.Parameters.AddWithValue("@A8", textBox4.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Producto guardado.", "Sistema Contable", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _dataExiste = true;
        }

        private void ActualizarData()
        {
            string query = "UPDATE PRODUCTOS SET DESCRIPCION=@A2,CANTIDADENEXISTENCIA=@A3,COSTO=@A4,PRECIODEVENTA=@A5,IMPUESTO=@A8,BARCODE=@A6 WHERE ITEM=@A1";
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnx))
                {
                    cmd.Parameters.AddWithValue("@A1", textBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@A2", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@A3", txtCantidad.Text.Trim());
                    cmd.Parameters.AddWithValue("@A4", txtCosto.Text.Trim());
                    cmd.Parameters.AddWithValue("@A5", txtVenta.Text.Trim());
                    cmd.Parameters.AddWithValue("@A6", textBox9.Text.Trim());
                    cmd.Parameters.AddWithValue("@A8", textBox4.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Producto actualizado.", "Sistema Contable", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void frmProductos_Load_1(object sender, EventArgs e)
        {

        }
    }

}