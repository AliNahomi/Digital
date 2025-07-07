public class Order
{
    public int Id { get; set; }
    public string ProductoNombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public decimal TotalLinea { get; set; }
    public string Estado { get; set; } = "En preparación";
    public DateTime FechaPedido { get; set; } = DateTime.Now;
    public string Cliente { get; set; } = "Cliente general";
    public string Observaciones { get; set; } = "";
}

