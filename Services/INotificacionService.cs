namespace VIANDRIVEFULL.Services
{
    public interface INotificacionService
    {
        Task<bool> EnviarSmsAsync(string celular, string mensaje);
    }
}
