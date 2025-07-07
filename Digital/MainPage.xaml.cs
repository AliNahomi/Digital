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
            await Navigation.PushAsync(new ActualizarPedidosPage());
        }

        private async void OnRecibirAlertasBajoStockClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AlertasStockPage());
        }

        private async void OnGenerarReportesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReporteVentasPage());
        }

    }
}
