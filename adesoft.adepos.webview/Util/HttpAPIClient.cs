using adesoft.adepos.webview.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Util
{
    public class HttpAPIClient
    {

        public HttpAPIClient()
        {

        }

        public static async Task<string> PostSendRequestCRM(string json, DTOParamCRM param, string ActionMethod)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(param.CRMUrlIntegration);
                client.Timeout = new TimeSpan(0, 2, 0);//dos minutos
                client.DefaultRequestHeaders.Accept.Clear();
                //    client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", your_token);
                //client.DefaultRequestHeaders.Add("Authorization", "Hmac Y3JtLnVuaXNwYW46VU4xU1A0Tg");
                client.DefaultRequestHeaders.Add("Authorization", param.Authorization );
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("Cookie", "PHPSESSID=5i8b6hdn35pphfi4ubghhssa66");
                client.DefaultRequestHeaders.Add("Cookie", param.Cookie);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //  HttpResponseMessage response = await client.GetAsync("?accion=retornarSedes&token=d31138373de89add710e7776a61325f546c53a32");
                HttpResponseMessage response = await client.PostAsync(ActionMethod, content);
                if (response.IsSuccessStatusCode)
                {
                    string resp = await response.Content.ReadAsStringAsync();
                    return resp.Replace(@"\", "").Replace("\"{", "{").Replace("}\"", "}").Replace("\"[", "[").Replace("]\"", "]");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public static async Task<string> PostSendRequest(string json, string url, string ActionMethod)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = new TimeSpan(0, 2, 0);//dos minutos
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(json, Encoding.UTF8, "application/json");
           
                //  HttpResponseMessage response = await client.GetAsync("?accion=retornarSedes&token=d31138373de89add710e7776a61325f546c53a32");
                HttpResponseMessage response = await client.PostAsync(ActionMethod, content);
                if (response.IsSuccessStatusCode)
                {
                    string resp = await response.Content.ReadAsStringAsync();
                    return resp.Replace(@"\", "").Replace("\"{", "{").Replace("}\"", "}").Replace("\"[", "[").Replace("]\"", "]");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Como está usando .Result o .Wait o wait, esto terminará causando un punto muerto en su código.
        /// puede usar ConfigureAwait(false) en métodos asincrónicos para evitar el punto muerto
        /// //string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        /// </summary>
        /// <param name="json"></param>
        /// <param name="url"></param>
        /// <param name="ActionMethod"></param>
        /// <returns></returns>
        public static async Task<string> PostSendRequestConfigureAwait(string json, string url, string ActionMethod, bool ReplaceChars)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = new TimeSpan(0, 2, 0);//dos minutos
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //  HttpResponseMessage response = await client.GetAsync("?accion=retornarSedes&token=d31138373de89add710e7776a61325f546c53a32");
                HttpResponseMessage response = await client.PostAsync(ActionMethod, content).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string resp = await response.Content.ReadAsStringAsync();
                    if (ReplaceChars)
                        return resp.Replace(@"\", "").Replace("\"{", "{").Replace("}\"", "}").Replace("\"[", "[").Replace("]\"", "]");
                    else
                        return resp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
