using AIChef.Server.Data;
using AIChef.Server.Services.Interfaces;
using AIChef.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AIChef.Server.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class RecipeController : ControllerBase
	{
		private readonly IOpenAIApi _openAiService;

        public RecipeController(IOpenAIApi openAiService)
        {
            _openAiService = openAiService;
        }

        [HttpPost, Route("GetRecipeIdeas")]
		public async Task<ActionResult<List<Idea>>> GetRecipeIdeas(RecipeParms recipeParms) 
		{
			//return SampleData.RecipeIdeas;

			string mealTime = recipeParms.MealTime;
			List<string> ingredients = recipeParms.Ingredients
												  .Where(i => !string.IsNullOrEmpty(i.Description))
												  .Select(i => i.Description!)
												  .ToList();

			if (string.IsNullOrEmpty(mealTime))
			{
				mealTime = "Breakfast";
			}

			var ideas = await _openAiService.CreateRecipeIdeas(mealTime, ingredients);
			return ideas;

		}

		[HttpPost, Route("GetRecipe")]
		public async Task<ActionResult<Recipe?>> GetRecipe(RecipeParms recipeParms)
		{
			//return SampleData.Recipe;

			List<string> ingredients = recipeParms.Ingredients.Where(r => !string.IsNullOrEmpty(r.Description))
															  .Select(r => r.Description!)
															  .ToList();
			string? title = recipeParms.SelectedIdea;

			if (string.IsNullOrEmpty(title))
			{
				return BadRequest();
			}
			var recipe = await _openAiService.CreateRecipe(title, ingredients);
			return recipe;

		}

		[HttpGet, Route("GetRecipeImage")]
		public async Task<ActionResult<RecipeImage>> GetRecipeImage(string? title)
		{
			return SampleData.RecipeImage;
		}
	}
}
