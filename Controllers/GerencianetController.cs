using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using OfficesafeAndGerencianet.Dtos;
using RestSharp;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Http;
using Gerencianet.SDK;


namespace OfficesafeAndGerencianet.Controllers{
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
            var request = new RestRequest(new Uri($"{_gerencianet.RouteHttp}/oauth/token"),Method.Post);
            request.AddHeader("Authorization", "Basic " + authorization);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"grant_type\": \"client_credentials\"\r\n}", ParameterType.RequestBody);
            RestResponse restResponse = await client.ExecuteAsync(request);
            string response = restResponse.Content;
            return response;
        }
        [HttpGet]
        [Route("Access")]
        public async Task<string> Access([FromBody]AccessOjt access)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ClientCertificates.Add(new X509Certificate2(_gerencianet.CrtPath, ""));
            var client = new RestClient(handler);
    
            Method method = Method.Get;
            switch(access.Type){
                case "GET":{
                    method = Method.Get;
                    break;
                };
                case "POST":{
                    method = Method.Post;
                    break;
                };
                case "PUT":{
                    method = Method.Put;
                    break;
                };
                case "DELETE":{
                    method = Method.Delete;
                    break;
                };
                case "HEAD":{
                    method = Method.Head;
                    break;
                };
                case "OPTIONS":{
                    method = Method.Options;
                    break;
                };
                case "PATCH":{
                    method = Method.Patch;
                    break;
                };
                case "MERGE":{
                    method = Method.Merge;
                    break;
                };
                case "COPY":{
                    method = Method.Copy;
                    break;
                };
                case "SEARCH":{
                    method = Method.Search;
                    break;
                };
            }
            var request = new RestRequest(new Uri($"{_gerencianet.RouteHttp+access.Path}"),method);
            
            request.AddHeader("Authorization", $"Bearer {access.Token}");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", access.Json, ParameterType.RequestBody);
            
            RestResponse restResponse = await client.ExecuteAsync(request);
            string response = restResponse.Content;
            return response;
        }}
}