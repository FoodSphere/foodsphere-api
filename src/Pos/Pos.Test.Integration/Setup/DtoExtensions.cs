namespace FoodSphere.Pos.Test;

public static class DtoExtensions
{
    extension(IEnumerable<Ingredient> ingredients)
    {
        public ICollection<IngredientItemResponse> ToIngredientItemResponses()
        {
            return ingredients
                .Select(ingredient =>
                    new IngredientItemResponse
                    {
                        ingredient = new MenuIngredientResponse
                        {
                            id = ingredient.Id,
                            name = ingredient.Name,
                            unit = ingredient.Unit,
                            image_url = ingredient.ImageUrl,
                        },
                        amount = TestSeedingGenerator.GetAmount(),
                    })
                .ToArray();
        }
    }

    extension(IEnumerable<IngredientItemResponse> responses)
    {
        public ICollection<IngredientItemDto> ToIngredientItemDtos()
        {
            return responses
                .Select(res =>
                    new IngredientItemDto
                    {
                        ingredient_id = res.ingredient.id,
                        amount = res.amount,
                    })
                .ToArray();
        }
    }

    extension(IEnumerable<Tag> tags)
    {
        public ICollection<AssignTagRequest> ToAssignTagRequests()
        {
            return tags
                .Select(tag =>
                    new AssignTagRequest
                    {
                        tag_id = tag.Id,
                    })
                .ToArray();
        }

        public ICollection<TagDto> ToTagDtos()
        {
            return tags
                .Select(tag =>
                    new TagDto
                    {
                        tag_id = tag.Id,
                        name = tag.Name,
                    })
                .ToArray();
        }
    }
}