﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AIChef.Server.ChatEndPoint
{
	public class ChatRequest
	{
		/// <summary>
		/// Required <br />
		/// The OpenAI model to use in the request
		/// </summary>
		[Required]
		public string Model { get; set; } = "gpt-4.1";

		/// <summary>
		/// Required <br />
		/// A list of messages representing the chat with the AI model
		/// </summary>
		[Required]
		public ChatMessage[] Messages { get; set; } = Array.Empty<ChatMessage>();

		/// <summary>
		/// A list of functions the model may generate JSON inputs for
		/// </summary>
		public ChatFunction[]? Functions { get; set; }

		/// <summary>
		/// Controls how the model responds to function calls. <br />
		/// "none": model will not call a function <br />
		/// "auto": default if functions are present - model may pick a function or may generate a message for the user <br />
		/// {"name": "function_name"}: forces the model to call that function
		/// </summary>
		[JsonPropertyName("function_call")]
		public object? FunctionCall { get; set; }

		/// <summary>
		/// A value between 0 and 2 - higher values will be more random,
		/// lower values will be more focused and deterministic <br />
		/// <b>Either change this or top_p but not both</b><br />
		/// Defaults to 1
		/// </summary>
		public float? Temperature { get; set; }

		/// <summary>
		/// A value between 0 and 1. A value of 0.1 means only the tokens
		/// comprising the top 10% probability will be considered <br />
		/// <b>Either change this or temperature, but not both</b><br />
		/// Defaults to 1
		/// </summary>
		[JsonPropertyName("top_p")]
		public float? TopP { get; set; }

		/// <summary>
		/// How many alternative responses to generate for each input message <br />
		/// Defaults to 1
		/// </summary>
		public int? N { get; set; }

		/// <summary>
		/// Up to 4 sequences where the API will stop generating further tokens <br />
		/// Defaults to null
		/// </summary>
		public string[]? Stop { get; set; }

		/// <summary>
		/// The maximum number of tokens to generate in the response <br />
		/// Defaults to the selected model's maximum supported tokens
		/// </summary>
		[JsonPropertyName("max_tokens")]
		public int? MaxTokens { get; set; }

		/// <summary>
		/// A number between -2.0 and 2.0. Positive values penalize new tokens
		/// based on whether they've appeared so far, increasing the model's
		/// likelihood to talk about new topics <br />
		/// Defaults to 0
		/// </summary>
		[JsonPropertyName("presence_penalty")]
		public float? PresencePenalty { get; set; }

		/// <summary>
		/// A number between -2.0 and 2.0. Positive values penalize new tokens
		/// based on their existing frequency in the text so far, reducing
		/// the model's likelihood to repeat the same line verbatim <br />
		/// Defaults to 0
		/// </summary>
		[JsonPropertyName("frequency_penalty")]
		public float? FrequencyPenalty { get; set; }

		/// <summary>
		/// An optional ID representing the end user, which can help OpenAI 
		/// monitor and detect abuse
		/// </summary>
		public string? User { get; set; }
	}

	public class ChatMessage
	{
		/// <summary>
		/// "system" role - prompt for "setting up" the conversation <br />
		/// "user" role - message from the user that the model should respond to <br />
		/// "assistant" role - the AI model's responses
		/// </summary>
		[Required]
		public string? Role { get; set; } // "system", "user", or "assistant"

		/// <summary>
		/// The content of the message sent by the indicated role
		/// </summary>
		public string? Content { get; set; }

		/// <summary>
		/// An optional name identifying the author of the message<br />
		/// If the Message Role is "function", this is the name of the function
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// The name and arguments of a function that should be called,
		/// as generated by the model
		/// </summary>
		[JsonPropertyName("function_call")]
		public ChatFunctionResponse? FunctionCall { get; set; }
	}

	public class ChatResponse
	{
		/// <summary>
		/// The unique ID of the generated response
		/// </summary>
		public string? Id { get; set; }

		/// <summary>
		/// The type of response being sent by the model. Check the
		/// <a href="https://platform.openai.com/docs/api-reference/">OpenAI docs</a>
		/// for reference of the object's schema
		/// </summary>
		public string? Object { get; set; }

		/// <summary>
		/// Timestamp for when the response was generated
		/// </summary>
		public int? Created { get; set; }

		/// <summary>
		/// An array of generated responses by the AI. The length of this
		/// array will match the N property of the request.
		/// </summary>
		public ChatResponseChoice[]? Choices { get; set; }

		/// <summary>
		/// How many tokens were used to generate this response
		/// </summary>
		public Usage? Usage { get; set; }
	}

	public class ChatResponseChoice
	{
		/// <summary>
		/// The index in the array of choices where this option lives
		/// </summary>
		public int? Index { get; set; }

		/// <summary>
		/// The generated message in response
		/// </summary>
		public ChatMessage? Message { get; set; }

		/// <summary>
		/// Why the model stopped generating this response - i.e. stopped
		/// because it was completed, the token limit was exceeded, or it hit a 
		/// stop sequence defined by the request
		/// </summary>
		[JsonPropertyName("finish_reason")]
		public string? FinishReason { get; set; }
	}

	public class ChatFunction
	{
		/// <summary>
		/// The name of the function to be called. Must be
		/// a-z, A-Z, 0-9, underscores, or dashes. Max length
		/// of 64 characters
		/// </summary>
		[Required]
		[StringLength(64)]
		public string? Name { get; set; }

		/// <summary>
		/// A description of what the function does
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// The parameters the function accepts, as a
		/// JSON Schema object <br />
		/// See the <a href="https://platform.openai.com/docs/guides/gpt/function-calling">guide</a>
		/// from OpenAI or <a href="https://json-schema.org/understanding-json-schema/">JSON Schema</a> 
		/// for reference
		/// </summary>
		public object? Parameters { get; set; }

		public class Parameter
		{
			/// <summary>
			/// The JSON type of this parameter
			/// </summary>
			public string? Type { get; set; }

			/// <summary>
			/// A description of this paramter to give the AI context on how to use it
			/// </summary>
			public string? Description { get; set; }

			/// <summary>
			/// If the type is an object, this defines each property of that object
			/// </summary>
			public object? Properties { get; set; }

			/// <summary>
			/// An array of properties which are required for this parameter, if it is
			/// an object type
			/// </summary>
			public string[]? Required { get; set; }
		}
		public class Property
		{
			/// <summary>
			/// The JSON type of this property
			/// </summary>
			public string? Type { get; set; }

			/// <summary>
			/// A desciption of this property to give the AI context on how to use it
			/// </summary>
			public string? Description { get; set; }

			/// <summary>
			/// If this property is an array, this is a JSON type definition for the items
			/// contained in the array
			/// </summary>
			public string? Items { get; set; }
		}
	}

	public class Result<T>
	{
		/// <summary>
		/// If successful, Data is the item that was attempted to be received
		/// </summary>
		public T? Data { get; set; }

		/// <summary>
		/// If unsuccessful, the exception that may have been thrown when trying
		/// to obtain the Data
		/// </summary>
		public Exception? Exception { get; set; }

		/// <summary>
		/// An error message indicating what went wrong if the Data could not be obtained
		/// </summary>
		public string? ErrorMessage { get; set; }
	}

	public class ChatFunctionResponse
	{
		/// <summary>
		/// The name of the function which the model would like to call
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// A JSON object which can be used as arguments to complete the function
		/// which the AI would like to call
		/// </summary>
		public string? Arguments { get; set; }
	}

	public class Usage
	{
		/// <summary>
		/// How many tokens were used when reading the prompt
		/// </summary>
		[JsonPropertyName("prompt_tokens")]
		public int? PromptTokens { get; set; }

		/// <summary>
		/// How many tokens were used when generating a response
		/// to the prompt
		/// </summary>

		[JsonPropertyName("completion_tokens")]
		public int? CompletionTokens { get; set; }

		/// <summary>
		/// How many tokens were used in total for the request
		/// </summary>

		[JsonPropertyName("total_tokens")]
		public int? TotalTokens { get; set; }
	}

	public class ImageGenerationRequest
	{
		/// <summary>
		/// A text description of the image you would like to generate
		/// </summary>
		public string? Prompt { get; set; }

		/// <summary>
		/// How many image options should be generated <br />
		/// This is optional, and defaults to 1 if not specified
		/// </summary>
		public int N { get; set; } = 1;

		/// <summary>
		/// The size of the image(s) that should be generated.<br />
		/// Valid options are: <br /><br />
		///
		/// "256x256" <br />
		/// "512x512"<br />
		/// "1024x1024"<br />
		/// <br />
		/// Defaults to 1024x1024
		/// </summary>
		public string? Size { get; set; } = "512x512"; // "256x256", "512x512", or "1024x1024"

		/// <summary>
		/// The format of the response. Either "url" or "b64_json". Defaults to "url"
		/// </summary>
		[JsonPropertyName("response_format")]
		public string? ResponseFormat { get; set; } = "url"; // "url" or "b64_json"

		/// <summary>
		/// A unique ID of the user requesting this image - optional
		/// </summary>
		public string? User { get; set; }
	}

}
