using FoodSphere.Data.Models;

namespace FoodSphere.Data.DTOs;

public class ContactDTO
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

    public static ContactDTO? FromModel(Contact? model)
    {
        if (model is null) return null;

        return new ContactDTO
        {
            name = model.Name,
            email = model.Email,
            phone = model.Phone,
        };
    }
}