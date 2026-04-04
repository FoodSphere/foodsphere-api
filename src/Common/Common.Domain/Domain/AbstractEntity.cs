namespace FoodSphere.Common.Entity;

public interface IEntityKey;

public interface IEntityModel
{
    public DateTime CreateTime { get; set; }
}

public interface IUpdatableEntityModel : IEntityModel
{
    public DateTime? UpdateTime { get; set; }
}

public interface ISoftDeleteEntityModel : IEntityModel
{
    public DateTime? DeleteTime { get; set; }

    public bool IsDeleted => DeleteTime is not null;
}

public interface IImageEntityModel : IEntityModel
{
    public string? ImageUrl { get; set; }
}