using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Digital
{
    public partial class RegistrarProductosPage : ContentPage
    {
        // Exponemos la colección global directamente
        public ObservableCollection<Product> Productos => App.SharedProductos;

        private Product productoEnEdicion = null;

        public RegistrarProductosPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        // ==========================
        // 1) Botón “Agregar Producto”
        // ==========================
        private void OnAgregarProductoClicked(object sender, EventArgs e)
        {
            if (FormularioBorder.IsVisible)
            {
                LimpiarFormulario();
                FormularioBorder.IsVisible = false;
                productoEnEdicion = null;
            }
            else
            {
                productoEnEdicion = null;
                LimpiarFormulario();
                FormularioBorder.IsVisible = true;
            }
        }

        // ==========================
        // 2) Botón “Guardar”
        //      - Siempre agrega una nueva entrada,
        //        sin fusionar stock aquí.
        // ==========================
        private void OnGuardarClicked(object sender, EventArgs e)
        {
            // Validar Nombre
            if (string.IsNullOrWhiteSpace(EntryNombre.Text))
            {
                DisplayAlert("Error", "El nombre no puede quedar vacío.", "OK");
                return;
            }

            // Validar Precio > 0
            if (string.IsNullOrWhiteSpace(EntryPrecio.Text) ||
                !decimal.TryParse(EntryPrecio.Text, out decimal precioDecimal) ||
                precioDecimal <= 0m)
            {
                DisplayAlert("Error", "Ingrese un precio válido mayor que 0.", "OK");
                return;
            }

            // Validar Cantidad > 0
            if (string.IsNullOrWhiteSpace(EntryCantidad.Text) ||
                !int.TryParse(EntryCantidad.Text, out int cantidadEntera) ||
                cantidadEntera <= 0)
            {
                DisplayAlert("Error", "Ingrese una cantidad entera válida mayor que 0.", "OK");
                return;
            }

            // Descripción opcional
            var descripcion = EntryDescripcion.Text?.Trim() ?? "";

            if (productoEnEdicion == null)
            {
                // MODO ALTA puros registros: AGREGAMOS SIEMPRE UNA NUEVA ENTRADA
                var nuevoProducto = new Product
                {
                    Nombre = EntryNombre.Text.Trim(),
                    Descripcion = descripcion,
                    Precio = precioDecimal,
                    Cantidad = cantidadEntera
                };
                Productos.Add(nuevoProducto);
            }
            else
            {
                // MODO EDICIÓN: modificamos directamente la instancia existente
                productoEnEdicion.Nombre = EntryNombre.Text.Trim();
                productoEnEdicion.Descripcion = descripcion;
                productoEnEdicion.Precio = precioDecimal;
                productoEnEdicion.Cantidad = cantidadEntera;
            }

            LimpiarFormulario();
            FormularioBorder.IsVisible = false;
            productoEnEdicion = null;
        }

        // ==========================
        // 3) Botón “Cancelar”
        // ==========================
        private void OnCancelarClicked(object sender, EventArgs e)
        {
            LimpiarFormulario();
            FormularioBorder.IsVisible = false;
            productoEnEdicion = null;
        }

        // ==========================
        // 4) Ícono “✎ Editar producto”
        // ==========================
        private void OnEditarProductoClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Product prod)
            {
                productoEnEdicion = prod;
                EntryNombre.Text = prod.Nombre;
                EntryDescripcion.Text = prod.Descripcion;
                EntryPrecio.Text = prod.Precio.ToString();
                EntryCantidad.Text = prod.Cantidad.ToString();
                FormularioBorder.IsVisible = true;
            }
        }

        // ==========================
        // 5) Ícono “🗑 Eliminar producto”
        // ==========================
        private async void OnEliminarProductoClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Product prod)
            {
                bool confirmar = await DisplayAlert(
                    "Confirmar",
                    $"¿Seguro que deseas eliminar \"{prod.Nombre}\"?",
                    "Sí",
                    "No");

                if (confirmar)
                {
                    Productos.Remove(prod);
                    if (productoEnEdicion == prod)
                    {
                        productoEnEdicion = null;
                        LimpiarFormulario();
                        FormularioBorder.IsVisible = false;
                    }
                }
            }
        }

        // ==========================
        // 6) Botón “Regresar al inicio”
        // ==========================
        private async void OnRegresarInicioClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        // ==========================
        // Limpia los campos del formulario
        // ==========================
        private void LimpiarFormulario()
        {
            EntryNombre.Text = string.Empty;
            EntryDescripcion.Text = string.Empty;
            EntryPrecio.Text = string.Empty;
            EntryCantidad.Text = string.Empty;
        }
    }
}
