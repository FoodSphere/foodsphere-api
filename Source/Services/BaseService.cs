using Microsoft.EntityFrameworkCore;

namespace FoodSphere.Services;

public class BaseService(AppDbContext context)
{
    protected readonly AppDbContext _ctx = context;

    public async Task<int> Save()
    {
        return await _ctx.SaveChangesAsync();
    }
}

/*
# Eager loading
```
var blog = context.Blogs
    .Include(b => b.Posts)
    .FirstOrDefault(b => b.Id == id);
```

# Explicit loading
```
var blog = context.Blogs.Find(id);
context.Entry(blog).Collection(b => b.Posts).Load();
```
*/