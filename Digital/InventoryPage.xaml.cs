using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Digital
{
    public partial class InventoryPage : ContentPage
    {
        public ObservableCollection<Product> FilteredProductos { get; set; }

        public InventoryPage()
        {
            InitializeComponent();

            RecalcularAgrupacion();
            BindingContext = this;
        }

        // Agrupa App.SharedProductos y suma cantidades (stock) por Nombre+Descripción+Precio
        void RecalcularAgrupacion()
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

            if (FilteredProductos == null)
            {
                FilteredProductos = new ObservableCollection<Product>(agrupados);
            }
            else
            {
                FilteredProductos.Clear();
                foreach (var prod in agrupados)
                    FilteredProductos.Add(prod);
            }
        }

        // Evento cuando cambia texto en SearchBar
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarAgrupacion(e.NewTextValue);
        }

        // Evento cuando se pulsa la lupa en el teclado
        private void OnSearchBarPressed(object sender, EventArgs e)
        {
            FiltrarAgrupacion(SearchBarFiltro.Text);
        }

        // Filtra la colección agrupada según texto de búsqueda
        void FiltrarAgrupacion(string filtro)
        {
            // Primero recalculamos la agrupación completa
            RecalcularAgrupacion();

            if (string.IsNullOrWhiteSpace(filtro))
                return;

            var texto = filtro.Trim().ToLower();

            // Filtramos por Nombre o Descripción
            var resultados = FilteredProductos
                .Where(p => p.Nombre.ToLower().Contains(texto) ||
                            p.Descripcion.ToLower().Contains(texto))
                .ToList();

            // Actualizamos la colección filtrada
            FilteredProductos.Clear();
            foreach (var prod in resultados)
                FilteredProductos.Add(prod);
        }

        // ==========================
        // NUEVO MÉTODO: Regresar al inicio
        // ==========================
        private async void OnRegresarInicioClicked(object sender, EventArgs e)
        {
            // PopAsync quita esta página de la pila y regresa a MainPage (o a la anterior)
            await Navigation.PopAsync();
        }
    }
}
