using System.Text;

namespace Pastebug.BLL.Utils;

public class HashGenerator : IHashGenerator
{
    public int HashLength { get; set; } = 8;

    public string Generate()
    {
        string baseValue = Guid.NewGuid().ToString();
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(baseValue)).Remove(HashLength);
    }
}
