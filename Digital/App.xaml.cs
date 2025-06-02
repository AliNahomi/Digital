// App.xaml.cs
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using System.Collections.ObjectModel;

namespace Digital
{
    public partial class App : Application
    {
        // Colección global donde se guardan TODOS los registros que provienen
        // de la página "Registrar Productos". Cada registro es un Product con 
        // Nombre, Descripción, Precio y Cantidad (stock individual).
        public static ObservableCollection<Product> SharedProductos { get; }
            = new ObservableCollection<Product>();

        public App()
        {
            InitializeComponent();
        }

        // Sobrescribimos CreateWindow para iniciar navigation stack
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new MainPage()));
        }
    }
}
