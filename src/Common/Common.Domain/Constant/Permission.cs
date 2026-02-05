using System.Reflection;

namespace FoodSphere.Common.Constant;

// CREATE
// GET, READ
// LIST
// UPDATE
// DELETE
// UNDELETE
public static class PERMISSION
{
    static readonly Lazy<List<Permission>> _allPermissions = new(() =>
    {
        var permissions = new List<Permission>();
        var types = new Queue<Type>();

        types.Enqueue(typeof(PERMISSION));

        while (types.Count > 0)
        {
            var type = types.Dequeue();

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(Permission))
                {
                    if (field.GetValue(null) is Permission permission)
                    {
                        permissions.Add(permission);
                    }
                }
            }

            foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
            {
                types.Enqueue(nestedType);
            }
        }

        return permissions;
    });

    // Lazy<T> delays the creation of an object of type T until the first time it is accessed
    public static Permission[] GetAll() => _allPermissions.Value.ToArray();

    public static class Restaurant
    {
        public static class Settings
        {
            public static readonly Permission READ = new()
            {
                Id = 1000,
                Name = "restaurant.read",
            };
            public static readonly Permission UPDATE = new()
            {
                Id = 1010,
                Name = "restaurant.update",
            };
        }
    }

    public static class Ingredient
    {
        public static readonly Permission CREATE = new()
        {
            Id = 2000,
            Name = "ingredient.create",
        };

        public static readonly Permission READ = new()
        {
            Id = 2010,
            Name = "ingredient.read",
        };

        public static readonly Permission UPDATE = new()
        {
            Id = 2020,
            Name = "ingredient.update",
        };
    }

    public static class Menu
    {
        public static readonly Permission CREATE = new()
        {
            Id = 3000,
            Name = "menu.create",
        };
        public static readonly Permission UPDATE = new()
        {
            Id = 3010,
            Name = "menu.update",
        };
    }

    public static class Branch
    {
        public static class Settings
        {
            public static readonly Permission READ = new()
            {
                Id = 4000,
                Name = "branch.read",
            };
            public static readonly Permission UPDATE = new()
            {
                Id = 4010,
                Name = "branch.update",
            };
        }
    }

    public static class Stock
    {
        public static readonly Permission READ = new()
        {
            Id = 5000,
            Name = "stock.read",
        };

        public static readonly Permission UPDATE = new()
        {
            Id = 5010,
            Name = "stock.update",
        };
    }

    public static class Table
    {
        public static readonly Permission CREATE = new()
        {
            Id = 6000,
            Name = "table.create",
        };
        public static readonly Permission UPDATE = new()
        {
            Id = 6010,
            Name = "table.update",
        };
    }

    public static class Order
    {
        public static readonly Permission CREATE = new()
        {
            Id = 7000,
            Name = "order.create",
        };

        public static readonly Permission GET = new()
        {
            Id = 7010,
            Name = "order.get",
        };

        public static readonly Permission LIST = new()
        {
            Id = 7020,
            Name = "order.list",
        };

        public static readonly Permission UPDATE = new()
        {
            Id = 7030,
            Name = "order.update",
        };
    }

    public static class Dashboard
    {
        public static readonly Permission READ = new()
        {
            Id = 8000,
            Name = "dashboard.read",
        };
    }

    public static class Role
    {
        public static readonly Permission CREATE = new()
        {
            Id = 9000,
            Name = "role.create",
        };

        public static readonly Permission READ = new()
        {
            Id = 9010,
            Name = "role.read",
        };

        public static readonly Permission UPDATE = new()
        {
            Id = 9020,
            Name = "role.update",
        };

        public static readonly Permission DELETE = new()
        {
            Id = 9030,
            Name = "role.delete",
        };
    }
}