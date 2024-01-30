namespace Pastebug.BLL.Utils;


public interface IHashGenerator
{ 
    string Generate();
    int HashLength { get; set; }
}