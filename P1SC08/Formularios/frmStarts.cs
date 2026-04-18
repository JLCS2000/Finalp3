using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace P1SC08
{
    public partial class frmStarts : Form
    {
        // ── campo que almacena el hash de contraseña traído de BD ──────
        private string _hashAlmacenado = string.Empty;

        // ── contador de intentos fallidos ──────────────────────────────
        private int _intentosFallidos = 0;
        private const int MAX_INTENTOS = 3;

        public frmStarts()
        {
            InitializeComponent();
        }

        private void frmStarts_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Login";
        }

        private void frmStarts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }

        // ──────────────────────────────────────────────────────────────
        // TEXTBOX
        // ──────────────────────────────────────────────────────────────
        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtUsuario.Text.Trim() != string.Empty)
                {
                    txtPassword.Focus();
                }
            }
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() != string.Empty)
            {
                BuscarUsuario(txtUsuario.Text.Trim());
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPassword.Text.Trim() != string.Empty)
                {
                    btnAceptar.PerformClick();
                }
            }
        }

        // ──────────────────────────────────────────────────────────────
        // BOTONES
        // ──────────────────────────────────────────────────────────────
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Por favor ingrese su usuario.", "Sistema Contable",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            if (txtPassword.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Por favor ingrese su contraseña.", "Sistema Contable",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (_hashAlmacenado == string.Empty)
            {
                MessageBox.Show("Usuario no encontrado en el sistema.", "Sistema Contable",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsuario.Focus();
                return;
            }

            // BUG CORREGIDO: antes comparaba en texto plano.
            // Ahora valida con SHA-256 (RNF-03).
            bool credencialesValidas = Seguridad.VerificarPassword(
                txtPassword.Text.Trim(), _hashAlmacenado);

            if (credencialesValidas)
            {
                _intentosFallidos = 0;
                frmMenu frm = new frmMenu();
                frm.Show();
                this.Hide();
            }
            else
            {
                _intentosFallidos++;

                if (_intentosFallidos >= MAX_INTENTOS)
                {
                    MessageBox.Show(
                        "Ha superado el número máximo de intentos.\nLa aplicación se cerrará.",
                        "Acceso bloqueado",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                    return;
                }

                int restantes = MAX_INTENTOS - _intentosFallidos;
                MessageBox.Show(
                    $"Contraseña incorrecta. Le quedan {restantes} intento(s).",
                    "Acceso denegado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // ──────────────────────────────────────────────────────────────
        // MÉTODOS PRIVADOS
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Busca el usuario en la BD y guarda su hash de contraseña.
        /// BUG CORREGIDO: antes guardaba la contraseña en texto plano.
        /// Ahora recupera el campo CLAVE (que debe contener hash SHA-256).
        /// </summary>
        private void BuscarUsuario(string usuario)
        {
            _hashAlmacenado = string.Empty;

            string query = "SELECT CLAVE FROM USUARIO WHERE NOMBRECORTO = @USUARIO";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnx))
                {
                    cmd.Parameters.AddWithValue("@USUARIO", usuario);

                    using (SqlDataReader rcd = cmd.ExecuteReader())
                    {
                        if (rcd.Read())
                        {
                            _hashAlmacenado = rcd["CLAVE"].ToString();
                        }
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}
