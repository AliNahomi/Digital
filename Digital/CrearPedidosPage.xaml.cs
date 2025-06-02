using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Digital
{
    // Clase para representar cada línea en el carrito
    public class CartItem
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }

    public partial class CrearPedidosPage : ContentPage
    {
        // Colección enlazada al CollectionView de carrito
        public ObservableCollection<CartItem> CartItems { get; set; }

        // Inventario agrupado (suma de stock por artículo)
        private ObservableCollection<Product> InventarioAgrupado { get; set; }

        // 3) NUEVO: lista de pedidos ya enviados / completados
        public ObservableCollection<Order> CompletedOrders { get; set; }

        public CrearPedidosPage()
        {
            InitializeComponent(); // MUY importante: llama siempre a InitializeComponent al inicio

            // 1) Agrupar App.SharedProductos por Nombre, Descripción y Precio
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

            // 2) Llenar el Picker con los nombres de cada producto agrupado
            foreach (var prod in InventarioAgrupado)
            {
                PickerProductos.Items.Add(prod.Nombre);
            }

            // 3) Inicializar la lista de carrito vacía
            CartItems = new ObservableCollection<CartItem>();

            // 4) Asignar BindingContext = this para enlazar CartItems y CompletedOrders
            BindingContext = this;

            // 5) Inicializar campos en estado “vacío”
            EntryPrecioUnitario.Text = string.Empty;
            LabelStockInfo.Text = "Máximo: 0 en stock";
            StepperCantidad.Minimum = 1;
            StepperCantidad.Maximum = 1;  // Se ajustará cuando el usuario seleccione
            StepperCantidad.Value = 1;
            LabelCantidad.Text = "0";
            EntryTotalLinea.Text = "$0.00";
            LabelResumenArticulo.Text = "(sin selección)";
            LabelResumenCantidad.Text = "0";
            LabelResumenTotal.Text = "$0.00";
            EntryCarritoTotal.Text = "$0.00";
        }

        // ==============================
        // 1) Cuando cambia el Picker
        // ==============================
        private void OnPickerSelectionChanged(object sender, EventArgs e)
        {
            if (PickerProductos.SelectedIndex < 0)
            {
                // Sin selección → valores por defecto
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

            // Obtener producto seleccionado
            string nombreSeleccionado = PickerProductos.Items[PickerProductos.SelectedIndex];
            var producto = InventarioAgrupado.First(p => p.Nombre == nombreSeleccionado);

            // Rellenar precio unitario
            EntryPrecioUnitario.Text = producto.Precio.ToString("F2");

            // Mostrar stock
            LabelStockInfo.Text = $"Máximo: {producto.Cantidad} en stock";

            // Ajustar Stepper (mínimo 1, máximo = stock)
            StepperCantidad.Minimum = 1;
            StepperCantidad.Maximum = producto.Cantidad;
            StepperCantidad.Value = 1;
            LabelCantidad.Text = "1";

            // Calcular total línea inicial (cantidad=1)
            decimal totalInicial = producto.Precio * 1;
            EntryTotalLinea.Text = totalInicial.ToString("C");
            LabelResumenArticulo.Text = producto.Nombre;
            LabelResumenCantidad.Text = "1";
            LabelResumenTotal.Text = totalInicial.ToString("C");
        }

        // =============================================
        // 2) Cuando cambia el Stepper de cantidad
        // =============================================
        private void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            // Verificar que haya una selección válida
            if (PickerProductos.SelectedIndex < 0)
                return;

            // Obtener producto y stock
            string nombreSeleccionado = PickerProductos.Items[PickerProductos.SelectedIndex];
            var producto = InventarioAgrupado.First(p => p.Nombre == nombreSeleccionado);

            // Evitar que supere el stock
            int valorDeseado = (int)e.NewValue;
            if (valorDeseado > producto.Cantidad)
            {
                // Forzar al máximo permitido
                StepperCantidad.Value = producto.Cantidad;
                valorDeseado = producto.Cantidad;
            }

            LabelCantidad.Text = valorDeseado.ToString();

            // Recalcular total de línea
            decimal totalLinea = producto.Precio * valorDeseado;
            EntryTotalLinea.Text = totalLinea.ToString("C");
            LabelResumenCantidad.Text = valorDeseado.ToString();
            LabelResumenTotal.Text = totalLinea.ToString("C");
        }

        // ================================================
        // 3) “Agregar al carrito” (varias líneas permitidas)
        // ================================================
        private async void OnAgregarCarritoClicked(object sender, EventArgs e)
        {
            // Validar que haya un producto seleccionado
            if (PickerProductos.SelectedIndex < 0)
            {
                await DisplayAlert("Error", "Seleccione primero un producto.", "OK");
                return;
            }

            // Obtener datos de la línea actual
            string nombreSeleccionado = PickerProductos.Items[PickerProductos.SelectedIndex];
            var producto = InventarioAgrupado.First(p => p.Nombre == nombreSeleccionado);

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

            decimal subtotal = producto.Precio * cantidad;

            // Crear CartItem y agregarlo
            var nuevaLinea = new CartItem
            {
                ProductoNombre = producto.Nombre,
                Cantidad = cantidad,
                Subtotal = subtotal
            };
            CartItems.Add(nuevaLinea);

            // Actualizar el total general del carrito
            ActualizarTotalCarrito();

            // Confirmación
            await DisplayAlert("Carrito", $"{producto.Nombre} x{cantidad} agregado al carrito.", "OK");

            // Resetear selección para otra línea
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

        // ================================================
        // 4) Método auxiliar para recalcular el total
        // ================================================
        private void ActualizarTotalCarrito()
        {
            decimal suma = CartItems.Sum(item => item.Subtotal);
            EntryCarritoTotal.Text = suma.ToString("C");
        }

        // ================================================
        // 5) “Enviar el pedido” (vaciar carrito y confirmar)
        // ================================================
        private async void OnEnviarPedidoClicked(object sender, EventArgs e)
        {
            if (CartItems.Count == 0)
            {
                await DisplayAlert("Error", "El carrito está vacío.", "OK");
                return;
            }

            // Aquí podrías enviar al backend o guardarlo en BD
            await DisplayAlert("Pedido", "Pedido enviado con éxito.", "OK");

            // Vaciar carrito y actualizar total
            CartItems.Clear();
            EntryCarritoTotal.Text = "$0.00";
        }

        // ================================================
        // 6) Botón “Regresar al inicio”
        // ================================================
        private async void OnRegresarInicioClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        public class CartItem
        {
            public string ProductoNombre { get; set; } = string.Empty;
            public int Cantidad { get; set; }
            public decimal Subtotal { get; set; }
        }

        // Aquí podrías agregar más métodos, por ejemplo:
        //   private void OnEditarPedidoPersonalizadoClicked(object sender, EventArgs e) { … }
        //   y la propiedad pública ObservableCollection<Order> CompletedOrders { get; set; }
        //   si es que también estás enlazando la colección “PedidosPersonalizadosCollection” en el XAML.
    }
}
