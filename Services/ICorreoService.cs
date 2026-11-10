using System.Threading.Tasks;

public interface ICorreoService
{
    Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);
}
