using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
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
    [Route("Autorizer")]
    public async Task<string> Autoraizer()
    {
        var credencials = new Dictionary<string, string>{
            {"client_id", "you_Client_Id"},
            {"client_secret", "you_Client_Secret_"}
        };
        var authorization = Base64Encode(credencials["client_id"] + ":" + credencials["client_secret"]);
        HttpClientHandler handler = new HttpClientHandler();
        handler.ClientCertificates.Add(new X509Certificate2("./assets/certificado.p12", ""));
        var client = new RestClient(handler);
        var request = new RestRequest(new Uri("https://apis.gerencianet.com.br/open-finance/oauth/token"),Method.Post);
        request.AddHeader("Authorization", "Basic " + authorization);
        request.AddHeader("Content-Type", "application/json");
        request.AddParameter("application/json", "{\r\n    \"grant_type\": \"client_credentials\"\r\n}", ParameterType.RequestBody);
        RestResponse restResponse = await client.ExecuteAsync(request);
        string response = restResponse.Content;
        return response;
    }
    [HttpGet]
    [Route("Access")]
    public async Task<string> Acess([FromQuery]string path)
    {
        var client = new RestClient();
        var request = new RestRequest(new Uri(path),Method.Get);
        RestResponse restResponse = await client.ExecuteAsync(request);
        string response = restResponse.Content;
        return response;
    }
}