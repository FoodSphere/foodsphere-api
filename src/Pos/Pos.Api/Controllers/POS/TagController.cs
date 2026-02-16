namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/tags")]
public class TagController(
    ILogger<TagController> logger,
    RestaurantService restaurantService,
    TagService tagService
) : PosControllerBase
{
    /// <summary>
    /// list tags
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<TagResponse>>> ListTags(Guid restaurant_id)
    {
        var responses = await tagService.QueryTags()
            .Where(e =>
                e.RestaurantId == restaurant_id)
            .Select(TagResponse.Projection)
            .ToArrayAsync();

        return responses;
    }

    /// <summary>
    /// create tag
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TagResponse>> CreateTag(Guid restaurant_id, TagRequest body)
    {
        var restaurant = restaurantService.GetRestaurantStub(restaurant_id);

        var tag = await tagService.CreateTag(
            restaurant: restaurant,
            name: body.name
        );

        await tagService.SaveChanges();

        var response = await tagService.GetTag(
            restaurant_id, tag.Id,
            TagResponse.Projection
        );

        return CreatedAtAction(
            nameof(GetTag),
            new { restaurant_id, tag_id = tag.Id },
            response);
    }

    /// <summary>
    /// get tag
    /// </summary>
    [HttpGet("{tag_id}")]
    public async Task<ActionResult<TagResponse>> GetTag(Guid restaurant_id, short tag_id)
    {
        var response = await tagService.GetTag(restaurant_id, tag_id, TagResponse.Projection);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }


    /// <summary>
    /// update tag
    /// </summary>
    [HttpPut("{tag_id}")]
    public async Task<ActionResult<TagResponse>> UpdateTag(Guid restaurant_id, short tag_id, TagRequest body)
    {
        var tag = tagService.GetTagStub(restaurant_id, tag_id);

        tag.Name = body.name;

        var affected = await tagService.SaveChanges();

        if (affected == 0)
        {
            return NotFound();
        }

        return TagResponse.Project(tag);
    }

    /// <summary>
    /// delete tag
    /// </summary>
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