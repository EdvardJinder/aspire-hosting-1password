namespace OnePassword.Cli.Sdk;

public class OnePasswordCliException : Exception
{
    public OnePasswordCliException()
    {
    }
    public OnePasswordCliException(string message) : base(message)
    {
    }
    public OnePasswordCliException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
