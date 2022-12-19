using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using Gerencianet.Dtos;
using RestSharp;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Http;
using Gerencianet.SDK;
using Newtonsoft.Json.Linq;
namespace API.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class GerencianetController:ControllerBase{
        public GerencianetCredenciais _gerencianet { get; set; }
        public GerencianetController(GerencianetCredenciais gerencianet){
            _gerencianet = gerencianet;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        [HttpGet]
        [Route("Autorizer")]
        public async Task<string> Autoraizer()
        {
            var credencials = new Dictionary<string, string>{
                {"client_id",_gerencianet.ClientId},
                {"client_secret",_gerencianet.ClientSecret}
            };
            var authorization = Base64Encode(credencials["client_id"] + ":" + credencials["client_secret"]);
            HttpClientHandler handler = new HttpClientHandler();
            handler.ClientCertificates.Add(new X509Certificate2(_gerencianet.CrtPath, ""));
            var client = new RestClient(handler);
            var request = new RestRequest(new Uri("https://apis-h.gerencianet.com.br/oauth/token"),Method.Post);
            request.AddHeader("Authorization", "Basic " + authorization);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"grant_type\": \"client_credentials\"\r\n}", ParameterType.RequestBody);
            RestResponse restResponse = await client.ExecuteAsync(request);
            string response = restResponse.Content;
            return response;
        }
        [HttpPost]
        [Route("GerarBoleto/Balancete")]
        public async Task<string> GerarBoletoBalancete([FromBody]GerencianetBolix bolix){
            dynamic endpoints = new Endpoints(_gerencianet.ClientId, _gerencianet.ClientSecret,_gerencianet.Sandbox);
            var body = new
            {
                items = new[] {
                    new {
                        name = bolix.NomeProduto,
                        value = bolix.Valor,
                        amount = 1
                    }
                }
            };
            var response = endpoints.CreateCharge(null, body);
            // Console.WriteLine(response.ToString());
            return response.ToString();
        }
        [HttpPost]
        [Route("GerarBoleto/Bolix")]
        public async Task<string> GerarBoletoBolix([FromBody]GerencianetBolix bolix){
            dynamic endpoints = new Endpoints(_gerencianet.ClientId, _gerencianet.ClientSecret,_gerencianet.Sandbox);
            var body = new
            {
                items = new[]
                {
                    new
                    {
                        name = bolix.NomeProduto,
                        value = bolix.Valor,
                        amount = 1
                    }
                },
                payment = new
                {
                    banking_billet = new
                    {
                        expire_at = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                        customer = new
                        {
                            name = bolix.NomeClient,
                            email = bolix.EmailClient,
                            cpf = bolix.CpfClient,
                            phone_number = bolix.TelefoneClient
                        }
                    }
                }
            };
            var response = endpoints.OneStep(null, body);
            // Console.WriteLine(response.ToString());
            return response.ToString();
        }
        [HttpPost]
        [Route("PagarComCartao")]
        public Task<string> PagarComCartao([FromBody] GerencianetPagarComCartao dadosCartao){
            dynamic endpoints = new Endpoints(_gerencianet.ClientId, _gerencianet.ClientSecret, _gerencianet.Sandbox);

            var body = new
            {
                items = new[] {
                    new {
                        name = dadosCartao.NomeProduto,
                        value = dadosCartao.ValorProduto,
                        amount = 1
                    }
                },
                payment = new
                {
                    credit_card = new
                    {
                        installments = 1,
                        payment_token = dadosCartao.TokenCartao,//"7d0a3fe0f0c9caab4f3b6578317a9d7e8ed6303f",
                        billing_address = new
                        {
                            street = "Av. JK",
                            number = 909,
                            neighborhood = "Bauxita",
                            zipcode = "35400000",
                            city = "Ouro Preto",
                            state = "MG"
                        },
                        customer = new
                        {
                            name = dadosCartao.NomeClient,
                            email = dadosCartao.EmailClient,
                            birth = "2002-01-29",
                            cpf = dadosCartao.CpfClient,
                            phone_number = dadosCartao.TelefoneClient
                        }
                    }
                }
                };
            var response = endpoints.OneStep(null, body);
            return response.ToString();
        }
    }
}