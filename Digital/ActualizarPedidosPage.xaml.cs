using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace Digital
{
    public partial class ActualizarPedidosPage : ContentPage
    {
        private ObservableCollection<Order> pedidos = new();

        private Order pedidoSeleccionado;

        public ActualizarPedidosPage()
        {
            InitializeComponent();

            pedidos = App.SharedPedidos ?? new ObservableCollection<Order>();
            HistorialPedidosCollection.ItemsSource = pedidos;

            if (pedidos.Count == 0)
            {
                DisplayAlert("Aviso", "No hay pedidos pendientes por actualizar.", "OK");
            }

            PickerPedidos.Items.Clear();
            foreach (var pedido in pedidos.Where(p => p.Estado != "Entregado"))
            {
                PickerPedidos.Items.Add($"Pedido #{pedido.Id}");
            }

        }

        private void OnPedidoSeleccionado(object sender, EventArgs e)
        {
            if (PickerPedidos.SelectedIndex < 0)
                return;

            pedidoSeleccionado = pedidos[PickerPedidos.SelectedIndex];

            LabelInfoPedido.Text =
                $"Producto: {pedidoSeleccionado.ProductoNombre}\n" +
                $"Cantidad: {pedidoSeleccionado.Cantidad}\n" +
                $"Total: {pedidoSeleccionado.TotalLinea:C}\n" +
                $"Fecha: {pedidoSeleccionado.FechaPedido:dd/MM/yyyy}\n" +
                $"Cliente: {pedidoSeleccionado.Cliente}\n" +
                $"Estado actual: {pedidoSeleccionado.Estado}";

            PickerEstado.SelectedItem = pedidoSeleccionado.Estado;
            EditorObservaciones.Text = pedidoSeleccionado.Observaciones;
        }

        private async void OnGuardarCambiosClicked(object sender, EventArgs e)
        {
            if (pedidoSeleccionado == null)
            {
                await DisplayAlert("Error", "Debe seleccionar un pedido.", "OK");
                return;
            }

            pedidoSeleccionado.Estado = PickerEstado.SelectedItem?.ToString() ?? "En preparación";
            pedidoSeleccionado.Observaciones = EditorObservaciones.Text;

            await DisplayAlert("Éxito", $"Pedido #{pedidoSeleccionado.Id} actualizado ✅", "OK");

            // 🔄 Refrescar CollectionView
            HistorialPedidosCollection.ItemsSource = null;
            HistorialPedidosCollection.ItemsSource = pedidos;

            // Limpiar campos
            PickerPedidos.SelectedIndex = -1;
            PickerEstado.SelectedIndex = 0;
            EditorObservaciones.Text = string.Empty;
            LabelInfoPedido.Text = "Información del pedido...";
            pedidoSeleccionado = null;

            // Refrescar Picker sin los pedidos entregados
            PickerPedidos.Items.Clear();
            foreach (var pedido in pedidos.Where(p => p.Estado != "Entregado"))
            {
                PickerPedidos.Items.Add($"Pedido #{pedido.Id}");
            }

            // Refrescar lista del historial (CollectionView)
            HistorialPedidosCollection.ItemsSource = null;
            HistorialPedidosCollection.ItemsSource = pedidos;
        }
    }

}
