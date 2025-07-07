using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Digital
{
    public class CartItem
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }

    public partial class CrearPedidosPage : ContentPage

    {
        private List<Product> carrito = new();

        public ObservableCollection<CartItem> CartItems { get; set; }
        private ObservableCollection<Product> InventarioAgrupado { get; set; }

        public CrearPedidosPage()
        {
            InitializeComponent();

            var agrupados = App.SharedProductos
                .GroupBy(p => new { p.Nombre, p.Descripcion, p.Precio })
                .Select(g => new Product
                {
                    Nombre = g.Key.Nombre,
                    Descripcion = g.Key.Descripcion,
                    Precio = g.Key.Precio,
                    Cantidad = g.Sum(p => p.Cantidad)
                })
                .OrderBy(p => p.Nombre)
                .ToList();

            InventarioAgrupado = new ObservableCollection<Product>(agrupados);

            foreach (var prod in InventarioAgrupado)
            {
                PickerProductos.Items.Add(prod.Nombre);
            }

            CartItems = new ObservableCollection<CartItem>();
            BindingContext = this;

            EntryPrecioUnitario.Text = string.Empty;
            LabelStockInfo.Text = "Máximo: 0 en stock";
            StepperCantidad.Minimum = 1;
            StepperCantidad.Maximum = 1;
            StepperCantidad.Value = 1;
            LabelCantidad.Text = "0";
            EntryTotalLinea.Text = "$0.00";
            LabelResumenArticulo.Text = "(sin selección)";
            LabelResumenCantidad.Text = "0";
            LabelResumenTotal.Text = "$0.00";
            EntryCarritoTotal.Text = "$0.00";
        }
        private void RecargarInventarioAgrupado()
        {
            var agrupados = App.SharedProductos
                .GroupBy(p => new { p.Nombre, p.Descripcion, p.Precio })
                .Select(g => new Product
                {
                    Nombre = g.Key.Nombre,
                    Descripcion = g.Key.Descripcion,
                    Precio = g.Key.Precio,
                    Cantidad = g.Sum(p => p.Cantidad)
                })
                .OrderBy(p => p.Nombre)
                .ToList();

            InventarioAgrupado.Clear();
            foreach (var item in agrupados)
            {
                InventarioAgrupado.Add(item);
            }

            // Refrescar el Picker
            PickerProductos.Items.Clear();
            foreach (var prod in InventarioAgrupado)
            {
                PickerProductos.Items.Add(prod.Nombre);
            }
        }


        private void OnPickerSelectionChanged(object sender, EventArgs e)
        {
            if (PickerProductos.SelectedIndex < 0)
            {
                EntryPrecioUnitario.Text = string.Empty;
                LabelStockInfo.Text = "Máximo: 0 en stock";
                StepperCantidad.Maximum = 1;
                StepperCantidad.Value = 1;
                LabelCantidad.Text = "0";
                EntryTotalLinea.Text = "$0.00";
                LabelResumenArticulo.Text = "(sin selección)";
                LabelResumenCantidad.Text = "0";
                LabelResumenTotal.Text = "$0.00";
                return;
            }

            string nombreSeleccionado = PickerProductos.Items[PickerProductos.SelectedIndex];
            var producto = InventarioAgrupado.First(p => p.Nombre == nombreSeleccionado);

            EntryPrecioUnitario.Text = producto.Precio.ToString("F2");
            LabelStockInfo.Text = $"Máximo: {producto.Cantidad} en stock";
            StepperCantidad.Minimum = 1;
            StepperCantidad.Maximum = producto.Cantidad;
            StepperCantidad.Value = 1;
            LabelCantidad.Text = "1";

            decimal totalInicial = producto.Precio * 1;
            EntryTotalLinea.Text = totalInicial.ToString("C");
            LabelResumenArticulo.Text = producto.Nombre;
            LabelResumenCantidad.Text = "1";
            LabelResumenTotal.Text = totalInicial.ToString("C");
        }

        private void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (PickerProductos.SelectedIndex < 0)
                return;

            string nombreSeleccionado = PickerProductos.Items[PickerProductos.SelectedIndex];
            var producto = InventarioAgrupado.First(p => p.Nombre == nombreSeleccionado);

            int valorDeseado = (int)e.NewValue;
            if (valorDeseado > producto.Cantidad)
            {
                StepperCantidad.Value = producto.Cantidad;
                valorDeseado = producto.Cantidad;
            }

            LabelCantidad.Text = valorDeseado.ToString();

            decimal totalLinea = producto.Precio * valorDeseado;
            EntryTotalLinea.Text = totalLinea.ToString("C");
            LabelResumenCantidad.Text = valorDeseado.ToString();
            LabelResumenTotal.Text = totalLinea.ToString("C");
        }

        private async void OnAgregarCarritoClicked(object sender, EventArgs e)
        {
            if (PickerProductos.SelectedIndex < 0)
            {
                await DisplayAlert("Error", "Seleccione primero un producto.", "OK");
                return;
            }

            string nombreSeleccionado = PickerProductos.Items[PickerProductos.SelectedIndex];
            var producto = App.SharedProductos.First(p => p.Nombre == nombreSeleccionado);


            int cantidad = (int)StepperCantidad.Value;
            if (cantidad <= 0)
            {
                await DisplayAlert("Error", "La cantidad debe ser al menos 1.", "OK");
                return;
            }
            if (cantidad > producto.Cantidad)
            {
                await DisplayAlert("Error", "No puede pedir más del stock disponible.", "OK");
                StepperCantidad.Value = producto.Cantidad;
                return;
            }

            // 🟨 BUSCAR si ya está en el carrito
            var existente = CartItems.FirstOrDefault(c => c.ProductoNombre == producto.Nombre);
            if (existente != null)
            {
                existente.Cantidad += cantidad;
                existente.Subtotal += producto.Precio * cantidad;
            }
            else
            {
                CartItems.Add(new CartItem
                {
                    ProductoNombre = producto.Nombre,
                    Cantidad = cantidad,
                    Subtotal = producto.Precio * cantidad
                });
            }

            ActualizarTotalCarrito();

            await DisplayAlert("Carrito", $"{producto.Nombre} x{cantidad} agregado al carrito.", "OK");

            // Limpiar selección
            PickerProductos.SelectedIndex = -1;
            EntryPrecioUnitario.Text = string.Empty;
            LabelStockInfo.Text = "Máximo: 0 en stock";
            StepperCantidad.Maximum = 1;
            StepperCantidad.Value = 1;
            LabelCantidad.Text = "0";
            EntryTotalLinea.Text = "$0.00";
            LabelResumenArticulo.Text = "(sin selección)";
            LabelResumenCantidad.Text = "0";
            LabelResumenTotal.Text = "$0.00";
        }


        private void ActualizarTotalCarrito()
        {
            decimal suma = CartItems.Sum(item => item.Subtotal);
            EntryCarritoTotal.Text = suma.ToString("C");
        }

        private async void OnEnviarPedidoClicked(object sender, EventArgs e)
        {
            if (CartItems.Count == 0)
            {
                await DisplayAlert("Error", "El carrito está vacío.", "OK");
                return;
            }

            // Crear descripción resumen del pedido
            string descripcion = string.Join(", ", CartItems.Select(c => $"{c.ProductoNombre} x{c.Cantidad}"));

            // Calcular total
            decimal total = CartItems.Sum(c => c.Subtotal);

            // Crear un solo pedido consolidado
            App.SharedPedidos.Add(new Order
            {
                Id = App.SharedPedidos.Count + 1001,
                ProductoNombre = "Pedido personalizado",
                Descripcion = descripcion,
                PrecioUnitario = 0, // No aplica para consolidado
                Cantidad = CartItems.Sum(c => c.Cantidad),
                TotalLinea = total,
                Estado = "En preparación",
                FechaPedido = DateTime.Now,
                Cliente = "Cliente general"
            });

            // Actualizar el inventario por cada producto
            foreach (var item in CartItems)
            {
                var producto = App.SharedProductos.FirstOrDefault(p => p.Nombre == item.ProductoNombre);
                if (producto != null)
                {
                    producto.Cantidad -= item.Cantidad;
                    if (producto.Cantidad < 0)
                        producto.Cantidad = 0;
                }
            }

            await DisplayAlert("Pedido", "Pedido enviado con éxito y stock actualizado. ✅", "OK");

            // Limpiar carrito
            CartItems.Clear();
            EntryCarritoTotal.Text = "$0.00";
        }


        private async void OnRegresarInicioClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
