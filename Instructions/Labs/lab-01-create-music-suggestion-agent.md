---
lab:
  title: "Lab: Develop AI agents using Azure OpenAI and the Semantic Kernel SDK"
  module: "Module 01: Build your kernel"
---

# Lab: Create an AI music recommendation agent
# Student lab manual

In this lab, you create the code for an AI agent that can manage the user's music library and provide personalized song and concert recommendations. You use the Semantic Kernel SDK to build the AI agent and connect it to the large language model (LLM) service. The Semantic Kernel SDK allows you to create a smart application that can interact with the LLM service and provide personalized recommendations to the user.

## Lab Scenario

You are a developer for an international audio streaming service. You have been tasked with integrating the service with AI to provide users with a more personalized experience. The AI should be able to recommend songs and upcoming artist tours based on the user's listening history and preferences. You decide to use the Semantic Kernel SDK to build an AI agent that can interact with the large language model (LLM) service.

## Objectives

By completing this lab, you will accomplish the following:

* Create an endpoint for the large language model (LLM) service
* Build a Semantic Kernel object
* Run prompts using the Semantic Kernel SDK
* Create Semantic Kernel functions and plugins
* Enable automatic function calling to automate tasks

## Lab Setup

### Prerequisites

To complete the exercise you need to have the following installed on your system:

* [Visual Studio Code](https://code.visualstudio.com)
* [The latest .NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [The C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) for Visual Studio Code


### Prepare your development environment

For these exercises, a starter project is available for you to use. Use the following steps to set up the starter project:

> [!IMPORTANT]
> You must have .NET Framework 8.0 installed as well as the extensions VS Code extensions for C# and NuGet Package Manager.

1. Paste the following URL into a new browser window:
   
     `https://github.com/MicrosoftLearning/AZ-2005-Develop-AI-agents-OpenAI-Semantic-Kernel-SDK/blob/master/Allfiles/Labs/01/Lab-01-Starter.zip`

1. Download the zip file by clicking the `...` button located on the upper right side of the page, or press <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>S</kbd>.

1. Extract the contents of the zip file to a location that is easy to find and remember, such as a folder on your Desktop.

1. Open Visual Studio Code and select **File** > **Open Folder**.

1. Navigate to the **Starter** folder you extracted and select **Select Folder**.

1. Open the **Program.cs** file in the code editor.

> [!NOTE]
> If prompted to trust the files in the folder, select **Yes, I trust the authors** 

## Exercise 1: Run a prompt with the Semantic Kernel SDK

For this exercise, you create an endpoint for the large language model (LLM) service. The Semantic Kernel SDK uses this endpoint to connect to the LLM and run prompts. The Semantic Kernel SDK supports HuggingFace, OpenAI, and Azure Open AI LLMs. For this example, you use Azure Open AI.

**Estimated exercise completion time**: 10 minutes

### Task 1: Create an Azure OpenAI resource

1. Navigate to [https://portal.azure.com](https://portal.azure.com).

1. Create a new Azure OpenAI resource using the default settings.

    > [!NOTE]
    > If you already have an Azure OpenAI resource, you can skip this step.

1. After the resource is created, select **Go to resource**.

1. On the **Overview** page, select **Go to Azure AI Foundry portal**.

1. Select **Create New Deployment** then **from base models**.

1. On the models list, select **gpt-35-turbo-16k**.

1. Select **Confirm**

1. Enter a name for your deployment and leave the default options.

1. When the deployment completes, navigate back to your Azure OpenAI resource in the Azure Portal.

1. Under **Resource Management**, go to **Keys and Endpoint**.

    You'll use the data here in the next task to build your kernel. Remember to keep your keys private and secure!

### Task 2: Build your kernel

In this exercise, you learn how to build your first Semantic Kernel SDK project. You learn how to create a new project, add the Semantic Kernel SDK NuGet package, and add a reference to the Semantic Kernel SDK. Let's get started!

1. Return to your Visual Studio Code project.

1. Open the **appsettings.json** file and update the values with your Azure OpenAI Services model id, endpoint, and API key.

    ```json
    {
        "modelId": "gpt-35-turbo-16k",
        "endpoint": "",
        "apiKey": ""
    }
    ```

1. Open the Terminal by selecting **Terminal** > **New Terminal**.

1. In the Terminal, run the following command to install the Semantic Kernel SDK:

    `dotnet add package Microsoft.SemanticKernel --version 1.30.0`

1. 1. Add the following `using` directives to the **Program.cs** file:

    ```c#
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;
    using Microsoft.SemanticKernel.Connectors.OpenAI;
    ```

1. To create the kernel, add the following code to your **Program.cs** file:
    
    ```c#
    // Create a kernel builder with Azure OpenAI chat completion
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

    // Build the kernel
    var kernel = builder.Build();
    ```

1. To verify that your kernel and endpoint is working, enter the following code:

    ```c#
    var result = await kernel.InvokePromptAsync("Who are the top 5 most famous musicians in the world?");
    Console.WriteLine(result);
    ```

1. Right-click the **Starter** folder and select **Open in Integrated Terminal**.

1. Enter `dotnet run` in the terminal to run the code.

    Verify that you see a response from the Azure Open AI model containing the top 5 most famous musicians in the world.

    The response comes from the Azure Open AI model you passed to the kernel. The Semantic Kernel SDK is able to connect to the large language model (LLM) and run the prompt. Notice how quickly you were able to receive responses from the LLM. The Semantic Kernel SDK makes building smart applications easy and efficient.

## Exercise 2: Create custom music library plugins

In this exercise, you create custom plugins for your music library. You create functions that can add songs to the user's recently played list, get the list of recently played songs, and provide personalized song recommendations. You also create a function that suggests a concert based on the user's location and their recently played songs.

**Estimated exercise completion time**: 15 minutes

### Task 1: Create a music library plugin

In this task, you create a plugin that allows you to add songs to the user's recently played list and get the list of recently played songs. For simplicity, the recently played songs are stored in a text file.

1. In the **Plugins** folder, create a new file **MusicLibraryPlugin.cs**

    First, create some quick functions to get and add songs to the user's "Recently Played" list.

1. Enter the following code:

    ```c#
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using Microsoft.SemanticKernel;

    public class MusicLibraryPlugin
    {
        [KernelFunction("GetRecentPlays")]
        public static string GetRecentPlays()
        {
            string content = File.ReadAllText($"Files/RecentlyPlayed.txt");
            return content;
        }
    }
    ```

    In this code, you use the `KernelFunction` decorator to declare your native function. You use a descriptive name for the function so that the AI can call it correctly. The user's list of recent plays is stored in a text file called 'RecentlyPlayed.txt'. Next, you can add code to add a song to the list.

1. Add the following code to your `MusicLibraryPlugin` class:

    ```c#
    [KernelFunction("AddToRecentPlays")]
    public static string AddToRecentlyPlayed(string artist,  string song, string genre)
    {
        // Read the existing content from the file
        string filePath = "Files/RecentlyPlayed.txt";
        string jsonContent = File.ReadAllText(filePath);
        var recentlyPlayed = (JsonArray) JsonNode.Parse(jsonContent)!;

        var newSong = new JsonObject
        {
            ["title"] = song,
            ["artist"] = artist,
            ["genre"] = genre
        };

        // Insert the new song
        recentlyPlayed.Insert(0, newSong);
        File.WriteAllText(filePath, JsonSerializer.Serialize(recentlyPlayed,
            new JsonSerializerOptions { WriteIndented = true }));

        return $"Added '{song}' to recently played";
    }
    ```

    In this code, you create a function accepts the artist, song, and genre as strings. The 'RecentlyPlayed.txt' file contains json formatted list of songs that the user has recently played. This code reads the existing content from the file, parses it, and adds the new song to the list. Afterwards, the updated list is written back to the file.

1. Update your **Program.cs** file with the following code:

    ```c#
    var kernel = builder.Build();
    kernel.ImportPluginFromType<MusicLibraryPlugin>();

    // Get chat completion service.
    var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

    // Create a chat history object
    ChatHistory chatHistory = [];
    ```

    In this code, you import the plugin to the kernel and add set up for chat completion.

1. Add the following prompts to invoke the plugin:

    ```c#
    chatHistory.AddSystemMessage("When a user has played a song, add it to their list of recent plays.");
    chatHistory.AddSystemMessage("The listener has just played the song Danse by Tiara. It falls under these genres: French pop, electropop, pop.");

    ChatMessageContent reply = await chatCompletionService.GetChatMessageContentAsync(
        chatHistory,
        kernel: kernel
    );
    Console.WriteLine(reply.ToString());
    chatHistory.AddAssistantMessage(reply.ToString());
    ```

1. Run the code by entering `dotnet run` in the terminal.

    You should see the following output:

    ```output
    Added 'Danse' to recently played
    ```

    If you open up 'Files/RecentlyPlayed.txt,' you should see the new song added to the list.

### Task 2: Provide personalized song recommendations

In this task, you create a prompt that provides personalized song recommendations to the user based on their recently played songs. The prompt combines the native functions to generate a song recommendation. You also create a function from the prompt to make it reusable.

1. In your **MusicLibraryPlugin.cs** file, add the following function:

    ```c#
    [KernelFunction("GetMusicLibrary")]
    public static string GetMusicLibrary()
    {
        string dir = Directory.GetCurrentDirectory();
        string content = File.ReadAllText($"Files/MusicLibrary.txt");
        return content;
    }
    ```

    This function reads the list of available music from a file named 'MusicLibrary.txt'. The file contains a json formatted list of songs available to the user.

1. Update your **Program.cs** file with the following code:

    ```c#
    chatHistory.AddSystemMessage("When a user has played a song, add it to their list of recent plays.");
    
    string prompt = @"This is a list of music available to the user:
        {{MusicLibraryPlugin.GetMusicLibrary}} 

        This is a list of music the user has recently played:
        {{MusicLibraryPlugin.GetRecentPlays}}

        Based on their recently played music, suggest a song from
        the list to play next";

    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine(result);
    ```

    First you can remove the code that appends a song to the list. Afterwards, you combine your native plugin functions with a semantic prompt. The native functions are able to retrieve user data that the large language model (LLM) couldn't access on its own, and the LLM is able to generate a song recommendation based on the text input.

1. To test your code, enter `dotnet run` in the terminal.

    You should see a response similar to the following output:

    ```output 
    Based on the user's recently played music, a suggested song to play next could be "Sabry Aalil" by Yasemin since the user seems to enjoy pop and Egyptian pop music.
    ```

    >[!NOTE] The recommended song may be different than the one shown here.

1. Modify the code to create a function from the prompt:

    ```c#
    var songSuggesterFunction = kernel.CreateFunctionFromPrompt(
        promptTemplate: @"This is a list of music available to the user:
        {{MusicLibraryPlugin.GetMusicLibrary}} 

        This is a list of music the user has recently played:
        {{MusicLibraryPlugin.GetRecentPlays}}

        Based on their recently played music, suggest a song from
        the list to play next",
        functionName: "SuggestSong",
        description: "Suggest a song to the user"
    );

    kernel.Plugins.AddFromFunctions("SuggestSongPlugin", [songSuggesterFunction]);
    ```

    In this code, you create a function from your prompt and add it to the kernel plugins.

1. Add the following code to automatically invoke the function:

    ```c#
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };

    chatHistory.AddUserMessage("What song should I play next?");

    reply = await chatCompletionService.GetChatMessageContentAsync(
        chatHistory,
        kernel: kernel,
        executionSettings: openAIPromptExecutionSettings
    );
    Console.WriteLine(reply.ToString());
    chatHistory.AddAssistantMessage(reply.ToString());
    ```

    In this code, you create the setting to enable automatic function calling. Then you add a prompt that will invoke the function and retrieve the reply.

### Task 3: Provide personalized concert recommendations

In this task, you create a plugin that asks the LLM to suggest a concert based on the user's recently played songs and location.

1. In your **Program.cs** file, add the music concerts plugin to the kernel:

    ```c#
    var kernel = builder.Build();    
    kernel.ImportPluginFromType<MusicLibraryPlugin>();
    kernel.ImportPluginFromType<MusicConcertsPlugin>();
    ```

1. Add code to create a function from a prompt:

    ```c#
    var concertSuggesterFunction = kernel.CreateFunctionFromPrompt(
        promptTemplate: @"This is a list of the user's recently played songs:
        {{MusicLibraryPlugin.GetRecentPlays}}

        This is a list of upcoming concert details:
        {{MusicConcertsPlugin.GetConcerts}}

        Suggest an upcoming concert based on the user's recently played songs. 
        The user lives in {{$location}}, 
        please recommend a relevant concert that is close to their location.",
        functionName: "SuggestConcert",
        description: "Suggest a concert to the user"
    );

    kernel.Plugins.AddFromFunctions("SuggestConcertPlugin", [concertSuggesterFunction]);
    ```

    This function prompt takes the music library and upcoming concert information as well as a location from the user and provides a recommendation.

1. Add the following prompt to invoke the new plugin function:

    ```c#
    chatHistory.AddUserMessage("Can you recommend a concert for me? I live in Washington");

    reply = await chatCompletionService.GetChatMessageContentAsync(
        chatHistory,
        kernel: kernel,
        executionSettings: openAIPromptExecutionSettings
    );
    Console.WriteLine(reply.ToString());
    chatHistory.AddAssistantMessage(reply.ToString());
    ```

1. In the terminal, enter `dotnet run`

    You should see output similar to the following response:

    ```output
    I recommend you attend the concert of Lisa Taylor. She will be performing in Seattle, Washington, USA on 2/22/2024. Enjoy the show!
    ```
    
    Your response from the LLM may vary. Try tweaking your prompt and location to see what other results you can generate.

Now your agent is able to automatically perform different actions based on the user's input. Great work!

### Review

In this lab, you created an AI agent that can manage the user's music library and provide personalized song and concert recommendations. You used the Semantic Kernel SDK to build the AI agent and connect it to the large language model (LLM) service. You created custom plugins for your music library, enabled automatic function calling to make your agent dynamically respond to the user's input. Congratulations on completing this lab!
