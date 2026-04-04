namespace FoodSphere.Pos.Api.Controller;

[Route("restaurants/{restaurant_id}/tags")]
public class TagController(
    ILogger<TagController> logger,
    TagServiceBase tagService
) : PosControllerBase
{
    /// <summary>
    /// list tags
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<TagResponse>>> ListTags(
        Guid restaurant_id,
        [FromQuery] IReadOnlyCollection<string> type)
    {
        Expression<Func<Tag, bool>> predicate = e =>
            e.RestaurantId == restaurant_id;

        if (type.Count > 0)
            predicate = predicate.And(e => type.Contains(e.Type));

        return await tagService.ListTags(
            TagResponse.Projection, predicate);
    }

    /// <summary>
    /// create tag
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TagResponse>> CreateTag(
        Guid restaurant_id, TagRequest body)
    {
        var tag = await tagService.CreateTag(
            TagResponse.Projection, new(
                new(restaurant_id),
                body.name,
                body.type));

        if (tag is null)
            return NotFound();

        var (tagKey, response) = tag.Value;

        return CreatedAtAction(
            nameof(GetTag),
            new { restaurant_id, tag_id = tagKey.Id },
            response);
    }

    /// <summary>
    /// get tag
    /// </summary>
    [HttpGet("{tag_id}")]
    public async Task<ActionResult<TagResponse>> GetTag(
        Guid restaurant_id, short tag_id)
    {
        var response = await tagService.GetTag(
            TagResponse.Projection,
            new(restaurant_id, tag_id));

        if (response is null)
            return NotFound();

        return response;
    }


    /// <summary>
    /// update tag
    /// </summary>
    [HttpPut("{tag_id}")]
    public async Task<ActionResult> UpdateTag(
        Guid restaurant_id, short tag_id, TagRequest body)
    {
        var result = await tagService.UpdateTag(
            new(restaurant_id, tag_id), new(
                body.name));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }

    /// <summary>
    /// delete tag
    /// </summary>
    [HttpDelete("{tag_id}")]
    public async Task<ActionResult> DeleteTag(Guid restaurant_id, short tag_id)
    {
        var result = await tagService.DeleteTag(
            new(restaurant_id, tag_id));

        if (result.IsFailed)
            return result.Errors.ToActionResult();

        return NoContent();
    }
}