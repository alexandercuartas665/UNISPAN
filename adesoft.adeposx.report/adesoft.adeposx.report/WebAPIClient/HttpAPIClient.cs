using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using System.Web;

namespace adesoft.adeposx.report.WebAPIClient
{
    public class HttpAPIClient
    {
        public HttpClient client = new HttpClient();

        public HttpAPIClient(string baseadr)
        {

            var cookieContainer = new CookieContainer();
           
            //client.BaseAddress = new Uri("http://localhost:64195/");
            client.BaseAddress = new Uri(baseadr);
            client.DefaultRequestHeaders.Accept.Clear();
            client.Timeout = new TimeSpan(0, 5, 0);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //private static HttpAPIClient _instance;

        //// This is the static method that controls the access to the singleton
        //// instance. On the first run, it creates a singleton object and places
        //// it into the static field. On subsequent runs, it returns the client
        //// existing object stored in the static field.
        //public static HttpAPIClient GetInstance()
        //{
        //    if (_instance == null)
        //    {
        //        _instance = new HttpAPIClient();
        //    }
        //    return _instance;
        //}


        //static async Task<T> GetProductAsync<T>(string path)
        //{
        //    T product ;
        //    HttpResponseMessage response = await client.GetAsync(path);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        product = await response.Content.ReadAsAsync<T>();
        //    }
        //    return product;
        //}
        public async Task<T> PostGenericAsync<T>(string path, string Jsonobj)
        {
            T entity = default(T);

            //var formData = new MultipartFormDataContent();

            //formData.Add(paramacccion, "accion"); // Le paramètre P1 aura la valeur contenue dans param1String
            //formData.Add(paramid, "id");
            //formData.Add(paramsecreto, "secreto");


            //var payload = "{\"CustomerId\": 5,\"CustomerName\": \"Pepsi\"}";
           // HttpContent c = new StringContent(Jsonobj, Encoding.UTF8);

            HttpContent c = new StringContent(Jsonobj, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(path, c);
            if (response.IsSuccessStatusCode)
            {
                ///  product = await response.Content.ReadAsAsync<T>();
                string respo = await response.Content.ReadAsStringAsync();
                entity = JsonConvert.DeserializeObject<T>(respo);
            }
            return entity;
        }


        public async Task PostGenericAsync(string path, string Jsonobj)
        {
            //var formData = new MultipartFormDataContent();

            //formData.Add(paramacccion, "accion"); // Le paramètre P1 aura la valeur contenue dans param1String
            //formData.Add(paramid, "id");
            //formData.Add(paramsecreto, "secreto");


            //var payload = "{\"CustomerId\": 5,\"CustomerName\": \"Pepsi\"}";

            HttpContent c = new StringContent(Jsonobj, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(path, c);
            if (response.IsSuccessStatusCode)
            {
                ///  product = await response.Content.ReadAsAsync<T>();
                string respo = await response.Content.ReadAsStringAsync();

            }
        }



        public async Task<T> GetGenericAsync<T>(string path)
        {
            T entity = default(T);
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                ///  product = await response.Content.ReadAsAsync<T>();
                string respo = await response.Content.ReadAsStringAsync();
                entity = JsonConvert.DeserializeObject<T>(respo);
            }
            return entity;
        }




        public async Task<dynamic> GetGenericAsync(string path)
        {
            dynamic entity = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                ///  product = await response.Content.ReadAsAsync<T>();
                string respo = await response.Content.ReadAsStringAsync();
                entity = JsonConvert.DeserializeObject(respo);
            }
            return entity;
        }




    }
}