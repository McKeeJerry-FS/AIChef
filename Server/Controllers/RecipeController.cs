using AIChef.Server.Services.Interfaces;
using AIChef.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
	}
}
