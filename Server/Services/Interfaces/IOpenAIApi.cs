using AIChef.Shared;

namespace AIChef.Server.Services.Interfaces
{
	public interface IOpenAIApi
	{
		Task<List<Idea>> CreateRecipeIdeas(string mealTime, List<string> ingredients);


	}
}
