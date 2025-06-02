// Order.cs
// Asegúrate de que está en la raíz del proyecto y con Build Action = Compile
namespace Digital
{
    public class Order
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal TotalLinea { get; set; }
    }
}
