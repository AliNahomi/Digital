using System;
using Microsoft.Maui.Controls;

namespace Digital
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnRegistrarProductosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrarProductosPage());
        }

        private async void OnControlarInventarioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InventoryPage());
        }

        private async void OnCrearPedidosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CrearPedidosPage());
        }

        private async void OnActualizarEstadoPedidoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Menú", "Presionaste: Actualizar estado del pedido", "OK");
        }

        private async void OnRecibirAlertasBajoStockClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Menú", "Presionaste: Recibir alertas de bajo stock", "OK");
        }

        private async void OnGenerarReportesClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Menú", "Presionaste: Generar reportes de ventas y ganancias", "OK");
        }
    }
}
