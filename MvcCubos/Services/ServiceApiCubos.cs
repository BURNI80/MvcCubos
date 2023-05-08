using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using MvcCubos.Models;

namespace MvcCubos.Services
{
    public class ServiceApiCubos
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;

        public ServiceApiCubos(IConfiguration configuration)
        {
            this.UrlApi =
                configuration.GetValue<string>("ApiUrls:ApiOAuthCubos");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<string> GetTokenAsync(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    Email = email,
                    Password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data =
                        await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T>
            (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "Bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }


        public async Task<Usuario> GetPerfil(string token)
        {
            string request = "/api/PerfilUsuario";
            Usuario usu = await this.CallApiAsync<Usuario>(request, token);
            return usu;
        }

        public async Task<List<Pedido>> GetPedidos(string token)
        {
            string request = "/api/GetPedidosUsuario";
            List<Pedido> pedidos = await this.CallApiAsync<List<Pedido>>(request, token);
            return pedidos;
        }


        public async Task ComprarCubo(int id, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/CreatePedido/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                string json = JsonConvert.SerializeObject(id);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        public async Task RegisterUser(string nombre, string email, string imagen, string pass)
        {
            Usuario user = new Usuario
            {
                Nombre = nombre,
                Email = email,
                Imagen = imagen,
                Pass = pass,
                Id = 0
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/CreateUsuario/";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(user);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }


        public async Task<List<Cubo>> GetCubos()
        {
            string request = "/api/GetCubos/";
            List<Cubo> cubos = await this.CallApiAsync<List<Cubo>>(request);
            return cubos;
        }

        public async Task<List<Cubo>> GetCubosMarca(string marca)
        {
            string request = "/api/FindCuboMarca/" + marca;
            List<Cubo> cubos = await this.CallApiAsync<List<Cubo>>(request);
            return cubos;
        }

        public async Task CreateCubo(string nombre, string marca, string imagen, int precio)
        {
            Cubo cubo = new Cubo
            {
                Id = 0,
                Nombre = nombre,
                Marca = marca,
                Imagen = imagen,
                Precio = precio
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/CreateCubo";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(cubo);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }


        public async Task<Usuario> Login(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                LoginModel user = new LoginModel
                {
                    Email = email,
                    Password = password
                };

                string request = "/api/GetUsuario";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(user);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);

                string data = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(data);

                return null;
            }
        }
    }

}
