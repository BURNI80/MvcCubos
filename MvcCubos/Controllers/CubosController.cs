using Microsoft.AspNetCore.Mvc;
using MvcCubos.Models;
using MvcCubos.Services;

namespace MvcCubos.Controllers
{
    public class CubosController : Controller
    {

        private ServiceApiCubos service;
        private ServiceStorageBlobs storageBlob;

        public CubosController(ServiceApiCubos service, ServiceStorageBlobs storageBlob)
        {
            this.service = service;
            this.storageBlob = storageBlob;
        }

        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos = await this.service.GetCubos();

            foreach (Cubo cubo in cubos)
            {
                cubo.Imagen = await this.storageBlob.GetUrl("cubospublic", cubo.Imagen);
            }

            return View(cubos);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string marca)
        {
            List<Cubo> cubos = await this.service.GetCubosMarca(marca);

            foreach (Cubo cubo in cubos)
            {
                cubo.Imagen = await this.storageBlob.GetUrl("cubospublic", cubo.Imagen);
            }

            return View(cubos);
        }


        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string nombre, string marca, IFormFile imagen, int precio)
        {

            string blobName = imagen.FileName;
            using (Stream stream = imagen.OpenReadStream())
            {
                await this.storageBlob.UploadBlobAsync("cubospublic", blobName, stream);
            }

            await this.service.CreateCubo(nombre, marca, blobName, precio);



            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Comprar(int id)
        {
            await this.service.ComprarCubo(id, HttpContext.Session.GetString("TOKEN"));
            return RedirectToAction("Index");
        }
    }
}
