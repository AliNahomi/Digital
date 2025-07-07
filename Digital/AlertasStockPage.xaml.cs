using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace Digital
{
    public partial class AlertasStockPage : ContentPage
    {
        public ObservableCollection<Product> ProductosBajoStock { get; set; }

        public AlertasStockPage()
        {
            InitializeComponent();

            // Filtra productos con stock menor a 10
            ProductosBajoStock = new ObservableCollection<Product>(
                App.SharedProductos.Where(p => p.Cantidad < 10).ToList());

            BindingContext = this;
        }
    }
}

