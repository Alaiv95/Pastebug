using AutoMapper;
using System.Reflection;

namespace Pastebug.BLL.Mapping;

public class AssemblyMapProfile : Profile
{
    public AssemblyMapProfile(Assembly assembly) => ApplyMapping(assembly);

    private void ApplyMapping(Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapWith<>))
             )
            .ToList();

        foreach ( var type in types )
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod("Mapping");
            methodInfo?.Invoke(instance, new object[] { this });
        }
    }
}
