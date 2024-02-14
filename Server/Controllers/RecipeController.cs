using AIChef.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIChef.Server.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class RecipeController : ControllerBase
	{
		[HttpPost, Route("GetRecipeIdeas")]
		public async Task<ActionResult<List<Idea>>> GetRecipeIdeas(RecipeParms recipeParms) 
		{
			return SampleData.RecipeIdeas;
		}
	}
}
