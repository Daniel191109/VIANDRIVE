using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public class CorreoService : ICorreoService
{
    private const string ApiKey = "D4F9BEE006CCE16C3E8547962B2A525A1D7D42DCF6B9887EFE28D939C1EE86C1ED6319F685BBF7E32312534B3C7E250D";
    private const string Remitente = "viandrive25@gmail.com";

    public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
    {
        using var client = new HttpClient();
        var content = new StringContent(
            $"apikey={HttpUtility.UrlEncode(ApiKey)}" +
            $"&from={HttpUtility.UrlEncode(Remitente)}" +
            $"&to={HttpUtility.UrlEncode(destinatario)}" +
            $"&subject={HttpUtility.UrlEncode(asunto)}" +
            $"&bodyText={HttpUtility.UrlEncode(cuerpo)}",
            Encoding.UTF8,
            "application/x-www-form-urlencoded"
        );

        var response = await client.PostAsync("https://api.elasticemail.com/v2/email/send", content);
        return response.IsSuccessStatusCode;
    }
}
