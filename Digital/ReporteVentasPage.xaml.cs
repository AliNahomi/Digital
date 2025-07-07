using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Digital
{
    public partial class ReporteVentasPage : ContentPage
    {
        public ObservableCollection<Order> Pedidos { get; set; }

        public ReporteVentasPage()
        {
            InitializeComponent();
            Pedidos = new ObservableCollection<Order>(App.SharedPedidos);
            BindingContext = this;

            MostrarResumen();
            GenerarGraficoBarras();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Pedidos.Clear();
            foreach (var p in App.SharedPedidos)
                Pedidos.Add(p);

            MostrarResumen();
            GenerarGraficoBarras();

            // Llena el Picker solo si está vacío
            if (PickerFiltroEstado.Items.Count == 0)
            {
                var estadosUnicos = App.SharedPedidos
                    .Select(p => p.Estado)
                    .Where(e => !string.IsNullOrWhiteSpace(e)) // Evita valores nulos o vacíos
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                PickerFiltroEstado.Items.Add("Todos");
                foreach (var estado in estadosUnicos)
                    PickerFiltroEstado.Items.Add(estado);

                PickerFiltroEstado.SelectedIndex = 0;
            }
        }



        private void OnFiltroEstadoChanged(object sender, EventArgs e)
        {
            var estadoSeleccionado = PickerFiltroEstado.SelectedItem?.ToString();

            var filtrados = (estadoSeleccionado == "Todos" || string.IsNullOrEmpty(estadoSeleccionado))
            ? App.SharedPedidos.ToList()
            : App.SharedPedidos.Where(p => p.Estado == estadoSeleccionado).ToList();


            Pedidos.Clear();
            foreach (var pedido in filtrados)
                Pedidos.Add(pedido);

            MostrarResumen();
            GenerarGraficoBarras();
        }


       



        private void MostrarResumen()
        {
            decimal totalVentas = Pedidos.Sum(p => p.TotalLinea);
            int cantidad = Pedidos.Count;
            decimal promedio = cantidad > 0 ? totalVentas / cantidad : 0;

            LabelResumenTotal.Text = $"💰 Total vendido: ${totalVentas:F2}";
            LabelCantidadPedidos.Text = $"📦 Pedidos registrados: {cantidad}";
            LabelPromedio.Text = $"📈 Promedio por pedido: ${promedio:F2}";
        }

        private void GenerarGraficoBarras()
        {
            var estados = Pedidos
                .GroupBy(p => p.Estado)
                .Select(g => new { Estado = g.Key, Total = g.Sum(p => p.TotalLinea) })
                .ToList();

            var canvas = new DrawableBarChart(estados);
            BarChartView.Drawable = canvas;
        }
    }


    // Clase auxiliar para dibujar gráfico de barras simple
    public class DrawableBarChart : IDrawable
    {
        private readonly List<(string Estado, float Valor)> datos;

        public DrawableBarChart(IEnumerable<dynamic> resumen)
        {
            datos = resumen.Select(r => ((string)r.Estado, (float)r.Total)).ToList();
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float anchoBarra = 40;
            float espacio = 30;
            float inicioX = 20;
            float baseY = dirtyRect.Height - 40; // deja más espacio para las etiquetas
            float maxAltura = dirtyRect.Height - 80;

            float maxValor = datos.Max(d => d.Valor);
            if (maxValor == 0) maxValor = 1;

            canvas.FontSize = 12;
            canvas.FontColor = Colors.Black;

            for (int i = 0; i < datos.Count; i++)
            {
                var (estado, valor) = datos[i];
                float alto = (valor / maxValor) * maxAltura;
                float x = inicioX + i * (anchoBarra + espacio);

                // Dibuja barra
                canvas.FillColor = Colors.Coral;
                canvas.FillRectangle(x, baseY - alto, anchoBarra, alto);

                // Dibuja texto (más espacio para no cortar)
                canvas.DrawString(estado, x - 10, baseY + 5, anchoBarra + 20, 30,
                    HorizontalAlignment.Center, VerticalAlignment.Top);
            }
        }
    }

}

