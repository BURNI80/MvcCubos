using Microsoft.AspNetCore.Mvc;
using MvcCubos.Models;
using MvcCubos.Services;

namespace MvcCubos.Controllers
{
    public class UsuariosController : Controller
    {

        private ServiceApiCubos service;
        private ServiceStorageBlobs storageBlob;

        public UsuariosController(ServiceApiCubos service, ServiceStorageBlobs storageBlob)
        {
            this.service = service;
            this.storageBlob = storageBlob;
        }

        public async Task<IActionResult> Perfil()
        { 
            Usuario usuario = await this.service.GetPerfil(HttpContext.Session.GetString("TOKEN"));
            usuario.Imagen = await this.storageBlob.GetUrl("cubosprivate", usuario.Imagen);

            List<Pedido> pedidos = await this.service.GetPedidos(HttpContext.Session.GetString("TOKEN"));
            ViewData["PEDIDOS"] = pedidos;
            return View(usuario);
        }
    }
}
