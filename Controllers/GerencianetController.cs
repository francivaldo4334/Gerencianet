using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using RestSharp;

namespace OfficesafeAndGerencianet.Controllers;
[ApiController]
[Route("[controller]")]
public class GerencianetController:ControllerBase{
    public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    [HttpGet]
    public async Task<string> AutoraizerAsync()
    {
            var credencials = new Dictionary<string, string>{
                {"client_id", "Client_Id_..."},
                {"client_secret", "Client_Secret_..."}
            };
            var authorization = Base64Encode(credencials["client_id"] + ":" + credencials["client_secret"]);
            var certificates = new X509CertificateCollection();
            certificates.Add(new X509Certificate2("./assets/certificado.p12", ""));
            var options = new RestClientOptions("https://apis.gerencianet.com.br/oauth/token") {
                ThrowOnAnyError = true,
                ClientCertificates = certificates
            };
            var client = new RestClient(options);
            var request = new RestRequest(new Uri("https://api.github.com/users/francivaldo4334"),Method.Get);

            request.AddHeader("Authorization", "Basic " + authorization);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"grant_type\": \"client_credentials\"\r\n}", ParameterType.RequestBody);
            
            RestResponse restResponse = await client.ExecuteAsync(request);
            string response = restResponse.Content;

            Console.WriteLine($"RES : {response}");
            return response;
    }
    [HttpGet]
    [Route("Acess")]
    public async Task<string> Acess([FromQuery]string path)
    {
            var client = new RestClient();
            var request = new RestRequest(new Uri(path),Method.Get);
            RestResponse restResponse = await client.ExecuteAsync(request);
            string response = restResponse.Content;
            return response;
    }
}