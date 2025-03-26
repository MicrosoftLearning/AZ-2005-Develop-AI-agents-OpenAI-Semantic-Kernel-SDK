using Microsoft.Extensions.Configuration;

string filePath = Path.GetFullPath("appsettings.json");
var config = new ConfigurationBuilder()
.AddJsonFile(filePath)
.Build();

// Set your values in appsettings.json
string modelId = config["modelId"]!;
string endpoint = config["endpoint"]!;
string apiKey = config["apiKey"]!;

// Create a kernel builder with Azure OpenAI chat completion

// Build the kernel

// Import plugins to the kernel

// Create prompt execution settings

// Verify the endpoint and run a prompt

// Get chat completion service

// Add system messages to the chat

// Create a song suggester function using a prompt

// Create a concert suggester function using a prompt

// Invoke the song suggester function with a prompt from the user

// Create a helper function to await and output the reply from the chat completion service
