namespace Pastebug.BLL.Exceptions;

public class ApiException : Exception
{
    public ApiException(string message) : base(message) { }
    public int Code { get; set; } = 500;
}
