
namespace MvcCubos.Models
{
    public class Pedido
    {

        public int IdPedido { get; set; }

        public int IdCubo { get; set; }

        public int IdUsuario { get; set; }

        public DateTime Fecha { get; set; }
    }
}
