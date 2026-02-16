namespace FoodSphere.Common.DTO;

public class ContactDto
{
    /// <example>บิ๊กบัง</example>
    public string? name { get; set; }

    /// <example>bigbang@foodsphere.com</example>
    public string? email { get; set; }

    /// <example>0812345678</example>
    public string? phone { get; set; }

    public Contact ToModel()
    {
        return new Contact
        {
            Name = name,
            Email = email,
            Phone = phone,
        };
    }

    public static readonly Func<Contact, ContactDto> Project = Projection.Compile();

    public static Expression<Func<Contact, ContactDto>> Projection =>
        model => new ContactDto
        {
            name = model.Name,
            email = model.Email,
            phone = model.Phone,
        };
}