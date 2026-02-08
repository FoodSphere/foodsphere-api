namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/tags")]
public class TagController(
    ILogger<TagController> logger,
    RestaurantService restaurantService,
    TagService tagService
) : PosControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TagResponse>>> ListTags(Guid restaurant_id)
    {
        var tags = await tagService.ListTags(restaurant_id);

        return tags
            .Select(TagResponse.FromModel)
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult<TagResponse>> CreateTag(Guid restaurant_id, TagRequest body)
    {
        var restaurant = await restaurantService.GetRestaurant(restaurant_id);

        if (restaurant is null)
        {
            return NotFound();
        }

        var tag = await tagService.CreateTag(
            restaurant: restaurant,
            name: body.name
        );

        await tagService.SaveChanges();

        return CreatedAtAction(
            nameof(GetTag),
            new { restaurant_id, tag_id = tag.Id },
            TagResponse.FromModel(tag)
        );
    }

    [HttpGet("{tag_id}")]
    public async Task<ActionResult<TagResponse>> GetTag(Guid restaurant_id, short tag_id)
    {
        var tag = await tagService.GetTag(restaurant_id, tag_id);

        if (tag is null)
        {
            return NotFound();
        }

        return TagResponse.FromModel(tag);
    }

    [HttpPut("{tag_id}")]
    public async Task<ActionResult<TagResponse>> UpdateTag(Guid restaurant_id, short tag_id, TagRequest body)
    {
        var tag = await tagService.GetTag(restaurant_id, tag_id);

        if (tag is null)
        {
            return NotFound();
        }

        await tagService.UpdateTag(tag, body.name);
        await tagService.SaveChanges();

        return TagResponse.FromModel(tag);
    }

    [HttpDelete("{tag_id}")]
    public async Task<ActionResult> DeleteTag(Guid restaurant_id, short tag_id)
    {
        var tag = await tagService.GetTag(restaurant_id, tag_id);

        if (tag is null)
        {
            return NotFound();
        }

        await tagService.DeleteTag(tag);
        await tagService.SaveChanges();

        return NoContent();
    }
}