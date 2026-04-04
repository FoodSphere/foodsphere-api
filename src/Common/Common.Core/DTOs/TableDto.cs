namespace FoodSphere.Common.DTO;

public record TableBriefResponse
{
    public short id { get; set; }

    /// <example>"A1"</example>
    public string? name { get; set; }

    public static readonly Expression<Func<Table, TableBriefResponse>> Projection =
        model => new()
        {
            id = model.Id,
            name = model.Name,
        };

    public static readonly Func<Table, TableBriefResponse> Project = Projection.Compile();
}