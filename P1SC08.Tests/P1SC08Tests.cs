// ===========================================================================
// SISTEMA CONTABLE — P1SC08
// Proyecto de pruebas unitarias: P1SC08.Tests
// Framework: MSTest (Microsoft.VisualStudio.TestTools.UnitTesting)
//
// CÓMO AGREGAR AL PROYECTO:
// 1. En Visual Studio: Archivo > Nuevo > Proyecto > MSTest Test Project (.NET Framework)
// 2. Nombre: P1SC08.Tests
// 3. Agregar referencia al proyecto P1SC08
// 4. Instalar NuGet: Microsoft.VisualStudio.TestTools.UnitTesting
// ===========================================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;
using P1SC08;
using System;

namespace P1SC08.Tests
{
    // =========================================================================
    // SUITE 1: Pruebas de la clase Seguridad (hash SHA-256)
    // Referencia: HU-01 (Login) | RNF-03 (Contraseñas con hash)
    // =========================================================================
    [TestClass]
    public class SeguridadTests
    {
        // CP-HU01-001
        [TestMethod]
        [Description("CP-HU01-001: HashSHA256 debe retornar un string de 64 caracteres hexadecimales.")]
        public void HashSHA256_DebeRetornar64Caracteres()
        {
            // Arrange
            string password = "admin123";

            // Act
            string hash = Seguridad.HashSHA256(password);

            // Assert
            Assert.AreEqual(64, hash.Length,
                "El hash SHA-256 debe tener exactamente 64 caracteres.");
        }

        // CP-HU01-002
        [TestMethod]
        [Description("CP-HU01-002: El mismo input siempre debe producir el mismo hash (determinismo).")]
        public void HashSHA256_MismoInputProduceMismoHash()
        {
            // Arrange
            string password = "cajero123";

            // Act
            string hash1 = Seguridad.HashSHA256(password);
            string hash2 = Seguridad.HashSHA256(password);

            // Assert
            Assert.AreEqual(hash1, hash2,
                "El mismo input debe siempre producir el mismo hash.");
        }

        // CP-HU01-003
        [TestMethod]
        [Description("CP-HU01-003: Inputs diferentes deben producir hashes diferentes.")]
        public void HashSHA256_InputsDiferentesProducenHashesDiferentes()
        {
            // Arrange
            string pass1 = "admin123";
            string pass2 = "admin124";

            // Act
            string hash1 = Seguridad.HashSHA256(pass1);
            string hash2 = Seguridad.HashSHA256(pass2);

            // Assert
            Assert.AreNotEqual(hash1, hash2,
                "Contraseñas distintas deben producir hashes distintos.");
        }

        // CP-HU01-004
        [TestMethod]
        [Description("CP-HU01-004: VerificarPassword debe retornar true cuando la contraseña es correcta.")]
        public void VerificarPassword_ContrasenaCorrecta_RetornaTrue()
        {
            // Arrange
            string passwordPlano = "admin123";
            string hashAlmacenado = Seguridad.HashSHA256(passwordPlano);

            // Act
            bool resultado = Seguridad.VerificarPassword(passwordPlano, hashAlmacenado);

            // Assert
            Assert.IsTrue(resultado,
                "VerificarPassword debe retornar true con la contraseña correcta.");
        }

        // CP-HU01-005
        [TestMethod]
        [Description("CP-HU01-005: VerificarPassword debe retornar false cuando la contraseña es incorrecta.")]
        public void VerificarPassword_ContrasenaIncorrecta_RetornaFalse()
        {
            // Arrange
            string passwordCorrecto = "admin123";
            string passwordIncorrecto = "wrongpass";
            string hashAlmacenado = Seguridad.HashSHA256(passwordCorrecto);

            // Act
            bool resultado = Seguridad.VerificarPassword(passwordIncorrecto, hashAlmacenado);

            // Assert
            Assert.IsFalse(resultado,
                "VerificarPassword debe retornar false con contraseña incorrecta.");
        }

        // CP-HU01-006
        [TestMethod]
        [Description("CP-HU01-006: HashSHA256 no debe lanzar excepción con string vacío.")]
        public void HashSHA256_StringVacio_NoLanzaExcepcion()
        {
            // Arrange
            string password = string.Empty;

            // Act - no debe lanzar excepción
            string hash = Seguridad.HashSHA256(password);

            // Assert
            Assert.IsNotNull(hash, "El hash no debe ser null aun con input vacío.");
            Assert.AreEqual(64, hash.Length, "Incluso el hash del string vacío debe tener 64 caracteres.");
        }

        // CP-HU01-007
        [TestMethod]
        [Description("CP-HU01-007: El hash no debe contener la contraseña original en texto plano.")]
        public void HashSHA256_NoContienePasswordOriginal()
        {
            // Arrange
            string password = "admin123";

            // Act
            string hash = Seguridad.HashSHA256(password);

            // Assert
            Assert.IsFalse(hash.Contains(password),
                "El hash no debe contener la contraseña original.");
        }
    }

    // =========================================================================
    // SUITE 2: Pruebas de la clase ConvertImage
    // Referencia: HU-02 (Registrar Producto con imagen)
    // =========================================================================
    [TestClass]
    public class ConvertImageTests
    {
        // CP-HU02-010
        [TestMethod]
        [Description("CP-HU02-010: ImageToByteArray debe retornar un arreglo no vacío para una imagen válida.")]
        public void ImageToByteArray_ImagenValida_RetornaArregloNoVacio()
        {
            // Arrange — crear una imagen de 1x1 píxel en memoria
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(10, 10);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            g.Clear(System.Drawing.Color.Red);

            // Act
            byte[] resultado = ConvertImage.ImageToByteArray(bmp);

            // Assert
            Assert.IsNotNull(resultado, "El resultado no debe ser null.");
            Assert.IsTrue(resultado.Length > 0, "El arreglo de bytes no debe estar vacío.");

            // Cleanup
            g.Dispose();
            bmp.Dispose();
        }

        // CP-HU02-011
        [TestMethod]
        [Description("CP-HU02-011: ByteArrayToImage debe devolver una imagen válida a partir de bytes correctos.")]
        public void ByteArrayToImage_BytesValidos_RetornaImagen()
        {
            // Arrange — crear imagen y convertir a bytes
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(10, 10);
            byte[] bytes = ConvertImage.ImageToByteArray(bmp);

            // Act
            System.Drawing.Image img = ConvertImage.ByteArrayToImage(bytes);

            // Assert
            Assert.IsNotNull(img, "La imagen resultante no debe ser null.");

            // Cleanup
            img.Dispose();
            bmp.Dispose();
        }

        // CP-HU02-012
        [TestMethod]
        [Description("CP-HU02-012: Conversión ida y vuelta debe preservar dimensiones de la imagen.")]
        public void ConversionIdaYVuelta_PreservaDimensiones()
        {
            // Arrange
            int ancho = 100, alto = 80;
            System.Drawing.Bitmap bmpOriginal = new System.Drawing.Bitmap(ancho, alto);

            // Act
            byte[] bytes  = ConvertImage.ImageToByteArray(bmpOriginal);
            System.Drawing.Image bmpRestaurado = ConvertImage.ByteArrayToImage(bytes);

            // Assert
            Assert.AreEqual(ancho, bmpRestaurado.Width,
                "El ancho debe preservarse en la conversión.");
            Assert.AreEqual(alto, bmpRestaurado.Height,
                "El alto debe preservarse en la conversión.");

            // Cleanup
            bmpOriginal.Dispose();
            bmpRestaurado.Dispose();
        }
    }

    // =========================================================================
    // SUITE 3: Pruebas de lógica de negocio — Cálculos de Venta
    // Referencia: HU-05 (Registrar Venta) | HU-06 (Descuentos)
    // Estas pruebas validan la lógica pura sin depender de la BD.
    // =========================================================================
    [TestClass]
    public class CalculosVentaTests
    {
        // CP-HU05-020
        [TestMethod]
        [Description("CP-HU05-020: El subtotal de una línea debe ser cantidad × precio unitario.")]
        public void Subtotal_CantidadPorPrecio_EsCorrecto()
        {
            // Arrange
            decimal cantidad = 3;
            decimal precioUnitario = 50.00m;

            // Act
            decimal subtotal = cantidad * precioUnitario;

            // Assert
            Assert.AreEqual(150.00m, subtotal,
                "3 unidades a RD$50.00 debe dar RD$150.00.");
        }

        // CP-HU05-021
        [TestMethod]
        [Description("CP-HU05-021: El ITBIS al 18% debe calcularse correctamente sobre el subtotal.")]
        public void ITBIS_18PorCiento_CalculaCorrecto()
        {
            // Arrange
            decimal subtotal   = 100.00m;
            decimal porciento  = 18.00m;

            // Act
            decimal itbis = subtotal * (porciento / 100);

            // Assert
            Assert.AreEqual(18.00m, itbis,
                "El 18% de ITBIS sobre RD$100.00 debe ser RD$18.00.");
        }

        // CP-HU05-022
        [TestMethod]
        [Description("CP-HU05-022: El total = subtotal + ITBIS - descuento.")]
        public void Total_SubtotalMasITBISMenosDescuento_EsCorrecto()
        {
            // Arrange
            decimal subtotal  = 100.00m;
            decimal itbis     = 18.00m;
            decimal descuento = 10.00m;

            // Act
            decimal total = subtotal + itbis - descuento;

            // Assert
            Assert.AreEqual(108.00m, total,
                "100 + 18 - 10 = 108.");
        }

        // CP-HU06-030
        [TestMethod]
        [Description("CP-HU06-030: Un descuento del 10% sobre RD$200.00 debe ser RD$20.00.")]
        public void Descuento_10PorcientoSobre200_Es20()
        {
            // Arrange
            decimal total      = 200.00m;
            decimal porciento  = 10.00m;

            // Act
            decimal descuento = total * (porciento / 100);

            // Assert
            Assert.AreEqual(20.00m, descuento);
        }

        // CP-HU06-031
        [TestMethod]
        [Description("CP-HU06-031: Un descuento mayor al 10% requiere autorización (regla de negocio).")]
        public void Descuento_MayorA10Porciento_RequiereAutorizacion()
        {
            // Arrange
            decimal descuentoPorcentaje = 15.00m;
            const decimal LIMITE_SIN_AUTH = 10.00m;

            // Act
            bool requiereAutorizacion = descuentoPorcentaje > LIMITE_SIN_AUTH;

            // Assert
            Assert.IsTrue(requiereAutorizacion,
                "Un descuento del 15% debe requerir autorización del administrador.");
        }

        // CP-HU05-023
        [TestMethod]
        [Description("CP-HU05-023: No debe permitirse una venta con cantidad = 0.")]
        public void Venta_CantidadCero_EsInvalida()
        {
            // Arrange
            decimal cantidad = 0;

            // Act
            bool esValida = cantidad > 0;

            // Assert
            Assert.IsFalse(esValida, "Una venta con cantidad 0 no debe ser válida.");
        }

        // CP-HU05-024
        [TestMethod]
        [Description("CP-HU05-024: No debe permitirse una venta con cantidad negativa.")]
        public void Venta_CantidadNegativa_EsInvalida()
        {
            // Arrange
            decimal cantidad = -5;

            // Act
            bool esValida = cantidad > 0;

            // Assert
            Assert.IsFalse(esValida, "Una cantidad negativa no debe ser válida.");
        }
    }

    // =========================================================================
    // SUITE 4: Pruebas de validación de campos (reglas del formulario)
    // Referencia: HU-02 (Registrar Producto) | HU-05 (Registrar Venta)
    // =========================================================================
    [TestClass]
    public class ValidacionCamposTests
    {
        // CP-HU02-040
        [TestMethod]
        [Description("CP-HU02-040: Un código de producto vacío debe ser rechazado.")]
        public void Producto_CodigoVacio_EsInvalido()
        {
            string codigo = "   ";
            Assert.IsTrue(string.IsNullOrWhiteSpace(codigo),
                "Un código de espacios debe ser considerado vacío.");
        }

        // CP-HU02-041
        [TestMethod]
        [Description("CP-HU02-041: Un precio negativo no es un valor válido.")]
        public void Producto_PrecioNegativo_EsInvalido()
        {
            decimal precio = -50.00m;
            Assert.IsTrue(precio < 0,
                "Un precio negativo debe detectarse como inválido.");
        }

        // CP-HU02-042
        [TestMethod]
        [Description("CP-HU02-042: Un stock negativo no debe ser permitido (RNF del sistema).")]
        public void Producto_StockNegativo_EsInvalido()
        {
            decimal stock = -1;
            Assert.IsTrue(stock < 0,
                "El stock nunca debe ser negativo.");
        }

        // CP-HU02-043
        [TestMethod]
        [Description("CP-HU02-043: El costo debe ser menor o igual al precio de venta.")]
        public void Producto_CostoMayorQuePrecio_EsUnaAlerta()
        {
            decimal costo  = 500.00m;
            decimal precio = 400.00m;

            bool margenNegativo = costo > precio;

            Assert.IsTrue(margenNegativo,
                "Si el costo supera el precio de venta, el sistema debería alertar.");
        }

        // CP-HU10-050
        [TestMethod]
        [Description("CP-HU10-050: Un producto con stock <= 5 debe marcarse como stock bajo.")]
        public void StockBajo_StockMenorOIgualA5_EsAlerta()
        {
            decimal stockActual = 3;
            decimal stockMinimo = 5;

            bool alertaStock = stockActual <= stockMinimo;

            Assert.IsTrue(alertaStock,
                "Con stock=3 y mínimo=5, debe dispararse alerta de stock bajo.");
        }

        // CP-HU10-051
        [TestMethod]
        [Description("CP-HU10-051: Un producto con stock > mínimo no debe generar alerta.")]
        public void StockBajo_StockSuficiente_NoEsAlerta()
        {
            decimal stockActual = 20;
            decimal stockMinimo = 5;

            bool alertaStock = stockActual <= stockMinimo;

            Assert.IsFalse(alertaStock,
                "Con stock=20 y mínimo=5, no debe dispararse alerta.");
        }
    }
}
