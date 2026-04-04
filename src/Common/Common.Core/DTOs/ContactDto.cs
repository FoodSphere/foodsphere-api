namespace FoodSphere.Common.DTO;

public record ContactDto
{
    /// <example>บิ๊กบัง</example>
    public string? name { get; set; }

    /// <example>bigbang@foodsphere.com</example>
    public string? email { get; set; }

    /// <example>0812345678</example>
    public string? phone { get; set; }

    public static readonly Expression<Func<Contact, ContactDto>> Projection =
        model => new()
        {
            name = model.Name,
            email = model.Email,
            phone = model.Phone,
        };

    public static readonly Func<Contact, ContactDto> Project = Projection.Compile();

    public static implicit operator Contact(ContactDto? contact)
    {
        if (contact is null)
            return new();

        return new()
        {
            Name = contact.name,
            Email = contact.email,
            Phone = contact.phone,
        };
    }
}