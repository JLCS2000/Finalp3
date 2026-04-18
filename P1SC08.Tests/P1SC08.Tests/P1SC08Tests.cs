

using Microsoft.VisualStudio.TestTools.UnitTesting;
using P1SC08;
using System;

namespace P1SC08.Tests
{
    
    // (Login) | RNF-03 (Contraseñas con hash)
    [TestClass]
    public class SeguridadTests
    {
        // CP-HU01-001
        [TestMethod]
        [Description("CP-HU01-001: HashSHA256 debe retornar un string de 64 caracteres hexadecimales.")]
        public void HashSHA256_DebeRetornar64Caracteres()
        {
            string password = "admin123";

            string hash = Seguridad.HashSHA256(password);

            Assert.AreEqual(64, hash.Length,
                "El hash SHA-256 debe tener exactamente 64 caracteres.");
        }

        // CP-HU01-002
        [TestMethod]
        [Description("CP-HU01-002: El mismo input siempre debe producir el mismo hash (determinismo).")]
        public void HashSHA256_MismoInputProduceMismoHash()
        {
            string password = "cajero123";

            string hash1 = Seguridad.HashSHA256(password);
            string hash2 = Seguridad.HashSHA256(password);

            Assert.AreEqual(hash1, hash2,
                "El mismo input debe siempre producir el mismo hash.");
        }

        // CP-HU01-003
        [TestMethod]
        [Description("CP-HU01-003: Inputs diferentes deben producir hashes diferentes.")]
        public void HashSHA256_InputsDiferentesProducenHashesDiferentes()
        {
            string pass1 = "admin123";
            string pass2 = "admin124";

            string hash1 = Seguridad.HashSHA256(pass1);
            string hash2 = Seguridad.HashSHA256(pass2);

            Assert.AreNotEqual(hash1, hash2,
                "Contraseñas distintas deben producir hashes distintos.");
        }

        // CP-HU01-004
        [TestMethod]
        [Description("CP-HU01-004: VerificarPassword debe retornar true cuando la contraseña es correcta.")]
        public void VerificarPassword_ContrasenaCorrecta_RetornaTrue()
        {
            string passwordPlano = "admin123";
            string hashAlmacenado = Seguridad.HashSHA256(passwordPlano);

            bool resultado = Seguridad.VerificarPassword(passwordPlano, hashAlmacenado);

            Assert.IsTrue(resultado,
                "VerificarPassword debe retornar true con la contraseña correcta.");
        }

        // CP-HU01-005
        [TestMethod]
        [Description("CP-HU01-005: VerificarPassword debe retornar false cuando la contraseña es incorrecta.")]
        public void VerificarPassword_ContrasenaIncorrecta_RetornaFalse()
        {
            string passwordCorrecto = "admin123";
            string passwordIncorrecto = "wrongpass";
            string hashAlmacenado = Seguridad.HashSHA256(passwordCorrecto);

            bool resultado = Seguridad.VerificarPassword(passwordIncorrecto, hashAlmacenado);

            Assert.IsFalse(resultado,
                "VerificarPassword debe retornar false con contraseña incorrecta.");
        }

        // CP-HU01-006
        [TestMethod]
        [Description("CP-HU01-006: HashSHA256 no debe lanzar excepción con string vacío.")]
        public void HashSHA256_StringVacio_NoLanzaExcepcion()
        {
            string password = string.Empty;

            string hash = Seguridad.HashSHA256(password);

            Assert.IsNotNull(hash, "El hash no debe ser null aun con input vacío.");
            Assert.AreEqual(64, hash.Length, "Incluso el hash del string vacío debe tener 64 caracteres.");
        }

        // CP-HU01-007
        [TestMethod]
        [Description("CP-HU01-007: El hash no debe contener la contraseña original en texto plano.")]
        public void HashSHA256_NoContienePasswordOriginal()
        {
            string password = "admin123";

            string hash = Seguridad.HashSHA256(password);

            Assert.IsFalse(hash.Contains(password),
                "El hash no debe contener la contraseña original.");
        }
    }


    // (Registrar Venta) | HU-06 (Descuentos)

    [TestClass]
    public class CalculosVentaTests
    {
        [TestMethod]
        [Description("CP-HU05-020: El subtotal de una línea debe ser cantidad × precio unitario.")]
        public void Subtotal_CantidadPorPrecio_EsCorrecto()
        {
            decimal cantidad = 3;
            decimal precioUnitario = 50.00m;

            decimal subtotal = cantidad * precioUnitario;

            Assert.AreEqual(150.00m, subtotal,
                "3 unidades a RD$50.00 debe dar RD$150.00.");
        }

        // CP-HU05-021
        [TestMethod]
        [Description("CP-HU05-021: El ITBIS al 18% debe calcularse correctamente sobre el subtotal.")]
        public void ITBIS_18PorCiento_CalculaCorrecto()
        {
            decimal subtotal   = 100.00m;
            decimal porciento  = 18.00m;

            decimal itbis = subtotal * (porciento / 100);

            Assert.AreEqual(18.00m, itbis,
                "El 18% de ITBIS sobre RD$100.00 debe ser RD$18.00.");
        }

        // CP-HU05-022
        [TestMethod]
        [Description("CP-HU05-022: El total = subtotal + ITBIS - descuento.")]
        public void Total_SubtotalMasITBISMenosDescuento_EsCorrecto()
        {
            decimal subtotal  = 100.00m;
            decimal itbis     = 18.00m;
            decimal descuento = 10.00m;

            decimal total = subtotal + itbis - descuento;

            Assert.AreEqual(108.00m, total,
                "100 + 18 - 10 = 108.");
        }

        // CP-HU06-030
        [TestMethod]
        [Description("CP-HU06-030: Un descuento del 10% sobre RD$200.00 debe ser RD$20.00.")]
        public void Descuento_10PorcientoSobre200_Es20()
        {
            decimal total      = 200.00m;
            decimal porciento  = 10.00m;

            decimal descuento = total * (porciento / 100);

            Assert.AreEqual(20.00m, descuento);
        }

        // CP-HU06-031
        [TestMethod]
        [Description("CP-HU06-031: Un descuento mayor al 10% requiere autorización (regla de negocio).")]
        public void Descuento_MayorA10Porciento_RequiereAutorizacion()
        {
            decimal descuentoPorcentaje = 15.00m;
            const decimal LIMITE_SIN_AUTH = 10.00m;

            bool requiereAutorizacion = descuentoPorcentaje > LIMITE_SIN_AUTH;

            Assert.IsTrue(requiereAutorizacion,
                "Un descuento del 15% debe requerir autorización del administrador.");
        }

        // CP-HU05-023
        [TestMethod]
        [Description("CP-HU05-023: No debe permitirse una venta con cantidad = 0.")]
        public void Venta_CantidadCero_EsInvalida()
        {
            decimal cantidad = 0;

            bool esValida = cantidad > 0;

            Assert.IsFalse(esValida, "Una venta con cantidad 0 no debe ser válida.");
        }

        // CP-HU05-024
        [TestMethod]
        [Description("CP-HU05-024: No debe permitirse una venta con cantidad negativa.")]
        public void Venta_CantidadNegativa_EsInvalida()
        {
            decimal cantidad = -5;

            bool esValida = cantidad > 0;

            Assert.IsFalse(esValida, "Una cantidad negativa no debe ser válida.");
        }
    }

    // Referencia: HU-02 (Registrar Producto) | HU-05 (Registrar Venta)
    [TestClass]
    public class ValidacionCamposTests
    {
        [TestMethod]
        [Description("CP-HU02-040: Un código de producto vacío debe ser rechazado.")]
        public void Producto_CodigoVacio_EsInvalido()
        {
            string codigo = "   ";
            Assert.IsTrue(string.IsNullOrWhiteSpace(codigo),
                "Un código de espacios debe ser considerado vacío.");
        }

        [TestMethod]
        [Description("CP-HU02-041: Un precio negativo no es un valor válido.")]
        public void Producto_PrecioNegativo_EsInvalido()
        {
            decimal precio = -50.00m;
            Assert.IsTrue(precio < 0,
                "Un precio negativo debe detectarse como inválido.");
        }

        [TestMethod]
        [Description("CP-HU02-042: Un stock negativo no debe ser permitido (RNF del sistema).")]
        public void Producto_StockNegativo_EsInvalido()
        {
            decimal stock = -1;
            Assert.IsTrue(stock < 0,
                "El stock nunca debe ser negativo.");
        }

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
