namespace Perfumes.Api.Services;
public sealed class RegraException(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}

