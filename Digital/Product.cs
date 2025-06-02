// Product.cs
// Asegúrate de que está en la raíz del proyecto y con Build Action = Compile
namespace Digital
{
    public class Product
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
}
