namespace Application.Services
{
    public interface ILoggerService
    {
        void Write(string msg);

        void WriteError(string error);

    }
}
