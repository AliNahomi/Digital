using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using System.Collections.ObjectModel;

namespace Digital
{
    public partial class App : Application
    {
        // Productos registrados desde la página de inventario
        public static ObservableCollection<Product> SharedProductos { get; } = new();

        // Pedidos guardados desde la página CrearPedidosPage
        public static ObservableCollection<Order> SharedPedidos { get; set; } = new();
        public static ObservableCollection<Product> ProductosBajoStock =>
        new ObservableCollection<Product>(
        SharedProductos.Where(p => p.Cantidad < 10).ToList());


        public App()
        {
            InitializeComponent();

            // Inicia la app con navegación
            MainPage = new NavigationPage(new MainPage());
        }
    }
}

