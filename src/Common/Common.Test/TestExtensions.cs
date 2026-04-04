using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace FoodSphere.Common.Test;

public static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        public async Task<HttpResponseMessage> PatchAsJsonAsync<T>(
            [StringSyntax(StringSyntaxAttribute.Uri)]
            string requestUri,
            JsonPatchDocument<T> patchDoc,
            CancellationToken ct = default) where T : class
        {
            var content = JsonContent.Create(patchDoc, new MediaTypeHeaderValue("application/json-patch+json"));

            return await client.PatchAsync(requestUri, content, ct);
        }
    }
}