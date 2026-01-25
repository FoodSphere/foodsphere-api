namespace FoodSphere.Core.DTOs;

public class ContactDto
{
    public string? name { get; set; }
    public string? email { get; set; }
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

    public static ContactDto? FromModel(Contact? model)
    {
        if (model is null) return null;

        return new ContactDto
        {
            name = model.Name,
            email = model.Email,
            phone = model.Phone,
        };
    }
}