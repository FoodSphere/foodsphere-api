using Microsoft.AspNetCore.StaticFiles;

namespace FoodSphere.Common.Service;

public class MimeService
{
    public string? GetExtensionFromContentType(string contentType)
    {
        var provider = new FileExtensionContentTypeProvider();
        var extensionName = provider.Mappings.FirstOrDefault(x => x.Value == contentType).Key;

        if (extensionName is not null)
        {
            return extensionName;
        }

        return null;
    }
}