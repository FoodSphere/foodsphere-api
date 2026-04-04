using Microsoft.AspNetCore.StaticFiles;

namespace FoodSphere.Common.Service;

public interface IMimeService
{
    string? GetExtensionFromContentType(string contentType);
}

public class MimeService : IMimeService
{
    static readonly IDictionary<string, string> mappings;

    static MimeService()
    {
        var provider = new FileExtensionContentTypeProvider();
        mappings = provider.Mappings;
    }

    public string? GetExtensionFromContentType(string contentType)
    {
        var extensionName = mappings.FirstOrDefault(x => x.Value == contentType).Key;

        return extensionName;
    }
}