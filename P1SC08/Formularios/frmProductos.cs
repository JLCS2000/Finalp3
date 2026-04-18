using System;
using System.Data;
using System.Drawing;
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
            txtProducto.Text = Busco.BuscaUltimoNumero("1");
            _dataExiste = false;
        }

        private void frmProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        // ──────────────────────────────────────────────────────────────
        // TEXTBOX — navegación con Enter
        // ──────────────────────────────────────────────────────────────
        private void txtProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtProducto.Text.Trim() != string.Empty)
                    txtDescripcion.Focus();
            }
        }

        private void txtProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
                btnProducto.PerformClick();
        }

        private void txtProducto_Leave(object sender, EventArgs e)
        {
            if (txtProducto.Text.Trim() != string.Empty)
                BuscarProducto(txtProducto.Text.Trim());
        }

        private void txtDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtDescripcion.Text.Trim() != string.Empty)
                    txtCantidad.Focus();
            }
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCantidad.Text.Trim() != string.Empty)
                    txtCosto.Focus();
            }
        }

        private void txtCosto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCosto.Text.Trim() != string.Empty)
                    txtPrecio.Focus();
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecio.Text.Trim() != string.Empty)
                    txtPorciento.Focus();
            }
        }

        private void txtPorciento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPorciento.Text.Trim() != string.Empty)
                    txtBarCode.Focus();
            }
        }

        private void txtBarCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtBarCode.Text.Trim() != string.Empty)
                    btnGuardar.Focus();
            }
        }

        // ──────────────────────────────────────────────────────────────
        // BOTONES
        // ──────────────────────────────────────────────────────────────
        private void btnProducto_Click(object sender, EventArgs e)
        {
            frmVENPROD frm = new frmVENPROD();
            frm.Show();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // BUG CORREGIDO: validaciones con mensajes al usuario
            if (txtProducto.Text.Trim() == string.Empty)
            {
                MessageBox.Show("El código del producto es requerido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProducto.Focus(); return;
            }
            if (txtDescripcion.Text.Trim() == string.Empty)
            {
                MessageBox.Show("La descripción es requerida.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus(); return;
            }
            if (txtCantidad.Text.Trim() == string.Empty)
            {
                MessageBox.Show("La cantidad en existencia es requerida.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Focus(); return;
            }
            if (txtCosto.Text.Trim() == string.Empty)
            {
                MessageBox.Show("El costo es requerido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCosto.Focus(); return;
            }
            if (txtPrecio.Text.Trim() == string.Empty)
            {
                MessageBox.Show("El precio de venta es requerido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus(); return;
            }
            if (txtPorciento.Text.Trim() == string.Empty)
            {
                MessageBox.Show("El porciento de impuesto es requerido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPorciento.Focus(); return;
            }

            if (_dataExiste == false)
                InsertarData();
            else
                ActualizarData();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            txtProducto.Focus();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (_dataExiste == false)
            {
                MessageBox.Show("No hay ningún producto cargado para borrar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // BUG CORREGIDO: antes comparaba DialogResult (clase) en lugar de
            // dialogResult (variable local) — el borrado nunca se ejecutaba.
            DialogResult dialogResult = MessageBox.Show(
                "¿Está seguro de borrar el producto " + txtProducto.Text + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)   // ← variable, no clase
            {
                BorrarData(txtProducto.Text.Trim());
                LimpiarFormulario();
                MessageBox.Show("Producto eliminado correctamente.", "Sistema Contable",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ──────────────────────────────────────────────────────────────
        // MÉTODOS PRIVADOS
        // ──────────────────────────────────────────────────────────────
        private void LimpiarFormulario()
        {
            txtBarCode.Clear();
            txtCantidad.Clear();
            txtCosto.Clear();
            txtDescripcion.Clear();
            txtPorciento.Clear();
            txtPrecio.Clear();
            txtProducto.Clear();

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            _dataExiste = false;
            txtProducto.Text = Busco.BuscaUltimoNumero("1");
        }

        /// <summary>
        /// Busca un producto por su código (ITEM).
        /// BUG CORREGIDO #1: Los alias del SELECT ahora coinciden con los nombres
        /// usados en rsd[]. Antes:
        ///   - SELECT usaba PRECIODEVENTA pero código leía rsd["PRECIO"]   → excepción
        ///   - SELECT usaba BARCODE       pero código leía rsd["BARRA"]    → excepción
        /// </summary>
        private void BuscarProducto(string numProducto)
        {
            _dataExiste = false;

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            string query = "SELECT ITEM, " +
                           "       DESCRIPCION, " +
                           "       CANTIDADENEXISTENCIA, " +
                           "       COSTO, " +
                           "       PRECIODEVENTA, " +   // ← alias ahora coincide
                           "       IMPUESTO, " +
                           "       BARCODE " +          // ← alias ahora coincide
                           "  FROM PRODUCTOS " +
                           " WHERE ITEM = @ITEM " +
                           "   AND EstatusProducto = 1";

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

                            txtProducto.Text    = Convert.ToString(rsd["ITEM"]);
                            txtDescripcion.Text = Convert.ToString(rsd["DESCRIPCION"]);
                            txtCantidad.Text    = Convert.ToString(rsd["CANTIDADENEXISTENCIA"]);
                            txtCosto.Text       = Convert.ToString(rsd["COSTO"]);
                            txtPrecio.Text      = Convert.ToString(rsd["PRECIODEVENTA"]);  // ✅
                            txtPorciento.Text   = Convert.ToString(rsd["IMPUESTO"]);
                            txtBarCode.Text     = Convert.ToString(rsd["BARCODE"]);         // ✅
                        }
                        else
                        {
                            MessageBox.Show("Producto no encontrado.", "Sistema Contable",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }

            // La imagen se carga en consulta separada para evitar el cast fallido
            if (_dataExiste)
                CargarImagenProducto(numProducto);
        }

        private void CargarImagenProducto(string numProducto)
        {
            string query = "SELECT IMAGEN FROM PRODUCTOS WHERE ITEM = @ITEM";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnx))
                {
                    cmd.Parameters.AddWithValue("@ITEM", numProducto);

                    using (SqlDataReader rsd = cmd.ExecuteReader())
                    {
                        if (rsd.Read() && rsd["IMAGEN"] != DBNull.Value)
                        {
                            try
                            {
                                pictureBox1.Image = ConvertImage.ByteArrayToImage(
                                    (byte[])rsd["IMAGEN"]);
                            }
                            catch { /* sin imagen */ }
                        }
                    }
                }
            }
        }

        private void BorrarData(string numProducto)
        {
            string query = "DELETE FROM PRODUCTOS WHERE ITEM = @ITEM";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnx))
                {
                    cmd.Parameters.AddWithValue("@ITEM", numProducto);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertarData()
        {
            string query = "INSERT INTO PRODUCTOS " +
                           "    (ITEM, DESCRIPCION, CANTIDADENEXISTENCIA, " +
                           "     COSTO, PRECIODEVENTA, BARCODE, " +
                           "     ESTATUSPRODUCTO, IMPUESTO) " +
                           "VALUES (@A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8)";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnx))
                {
                    cmd.Parameters.AddWithValue("@A1", txtProducto.Text.Trim());
                    cmd.Parameters.AddWithValue("@A2", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@A3", txtCantidad.Text.Trim());
                    cmd.Parameters.AddWithValue("@A4", txtCosto.Text.Trim());
                    cmd.Parameters.AddWithValue("@A5", txtPrecio.Text.Trim());
                    cmd.Parameters.AddWithValue("@A6", txtBarCode.Text.Trim());
                    cmd.Parameters.AddWithValue("@A7", 1);
                    cmd.Parameters.AddWithValue("@A8", txtPorciento.Text.Trim());

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Producto guardado correctamente.", "Sistema Contable",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            _dataExiste = true;
        }

        private void ActualizarData()
        {
            string query = "UPDATE PRODUCTOS " +
                           "   SET DESCRIPCION          = @A2, " +
                           "       CANTIDADENEXISTENCIA = @A3, " +
                           "       COSTO                = @A4, " +
                           "       PRECIODEVENTA        = @A5, " +
                           "       IMPUESTO             = @A8, " +
                           "       BARCODE              = @A6 " +
                           " WHERE ITEM = @A1";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnx))
                {
                    cmd.Parameters.AddWithValue("@A1", txtProducto.Text.Trim());
                    cmd.Parameters.AddWithValue("@A2", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@A3", txtCantidad.Text.Trim());
                    cmd.Parameters.AddWithValue("@A4", txtCosto.Text.Trim());
                    cmd.Parameters.AddWithValue("@A5", txtPrecio.Text.Trim());
                    cmd.Parameters.AddWithValue("@A6", txtBarCode.Text.Trim());
                    cmd.Parameters.AddWithValue("@A8", txtPorciento.Text.Trim());

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Producto actualizado correctamente.", "Sistema Contable",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}
