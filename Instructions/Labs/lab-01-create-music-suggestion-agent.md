---
lab:
  title: "Lab: Develop AI agents using Azure OpenAI and the Semantic Kernel SDK"
  module: "Module 01: Build your kernel"
---

# Lab: Create an AI music recommendation assistant
# Student lab manual

In this lab, you create the code for an AI assistant that can manage the user's music library and provide personalized song and concert recommendations. You use the Semantic Kernel SDK to build the AI assistant and connect it to the large language model (LLM) service. The Semantic Kernel SDK allows you to create a smart application that can interact with the LLM service and provide personalized recommendations to the user.

## Lab Scenario

You are a developer for an international audio streaming service. You have been tasked with integrating the service with AI to provide users with a more personalized experience. The AI should be able to recommend songs and upcoming artist tours based on the user's listening history and preferences. You decide to use the Semantic Kernel SDK to build an AI assistant that can interact with the large language model (LLM) service.

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

1. Add the following code under the comment **Create a kernel builder with Azure OpenAI chat completion**:

    ```c#
    // Create a kernel builder with Azure OpenAI chat completion
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
    ```

1. Build the kernel by adding this code under the comment **Build the kernel**:

    ```c#
    // Build the kernel
    var kernel = builder.Build();
    ```

1. Add the following code under the comment **Verify the endpoint and run a prompt**:

    ```c#
    // Verify the endpoint and run a prompt
    var result = await kernel.InvokePromptAsync("Who are the top 5 most famous musicians in the world?");
    Console.WriteLine(result);
    ```

1. Right-click the **Starter** folder and select **Open in Integrated Terminal**.

1. Enter `dotnet run` in the terminal to run the code.

    Verify that you see a response from the Azure Open AI model containing the top 5 most famous musicians in the world.

    The response comes from the Azure Open AI model you passed to the kernel. The Semantic Kernel SDK is able to connect to the large language model (LLM) and run the prompt. Notice how quickly you were able to receive responses from the LLM. The Semantic Kernel SDK makes building smart applications easy and efficient.

You can remove the verification code once you confirm your response.

## Exercise 2: Create custom music library plugins

In this exercise, you create custom plugins for your music library. You create functions that can add songs to the user's recently played list, get the list of recently played songs, and provide personalized song recommendations. You also create a function that suggests a concert based on the user's location and their recently played songs.

**Estimated exercise completion time**: 15 minutes

### Task 1: Create a music library plugin

In this task, you create a plugin that allows you to add songs to the user's recently played list and get the list of recently played songs. For simplicity, the recently played songs are stored in a text file.

1. In the **Plugins** folder, open the file **MusicLibraryPlugin.cs**

1. Under the comment **Create a kernel function to get recently played songs**, add the kernel function decorator:


    ```c#
    // Create a kernel function to get recently played songs
    [KernelFunction("GetRecentPlays")]
    public static string GetRecentPlays()
    ```

    The `KernelFunction` decorator declares your native function. You use a descriptive name for the function so that the AI can call it correctly. The user's list of recent plays is stored in a text file called 'RecentlyPlayed.txt'.

1. Under the comment **Create a kernel function to add a song to the recently played list**, add the kernel function decorator:

    ```c#
    // Create a kernel function to add a song to the recently played list
    [KernelFunction("AddToRecentPlays")]
    public static string AddToRecentlyPlayed(string artist,  string song, string genre)
    ```

    Now when this plugin class is added to the kernel, it will be able to identify and invoke the functions.

1. Navigate to the **Program.cs** file.

1. Add the following code under the comment **Import plugins to the kernel**:

    ```c#
    // Import plugins to the kernel
    kernel.ImportPluginFromType<MusicLibraryPlugin>();
    ```

1. Under the comment **Create prompt execution settings**, add the following code to automatically invoke the function:

    ```c#
    // Create prompt execution settings
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };
    ```

    Using this setting will allow the kernel to automatically invoke functions without the need to specify them in the prompt.

1. Add the following code under the comment **Get chat completion service**:

    ```c#
    // Get chat completion service.
    var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    ChatHistory chatHistory = [];
    ```

1. Add the following code under the comment **Create a helper function to await and output the reply from the chat completion service**:

    ```c#
    // Create a helper function to await and output the reply from the chat completion service
    async Task GetAssistantReply() {
        ChatMessageContent reply = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            kernel: kernel,
            executionSettings: openAIPromptExecutionSettings
        );
        chatHistory.AddAssistantMessage(reply.ToString());
        Console.WriteLine(reply.ToString());
    }
    ```


1. Add the following code under the **Add system messages to the chat** comment:

    ```c#
    // Add system messages to the chat
    chatHistory.AddSystemMessage("When a user has played a song, add it to their list of recent plays.");
    chatHistory.AddSystemMessage("The listener has just played the song Danse by Tiara. It falls under these genres: French pop, electropop, pop.");
    await GetAssistantReply();
    ```

1. Run the code by entering `dotnet run` in the terminal.

    The system message prompts you added should invoke the plugin functions and should see the following output:

    ```output
    Added 'Danse' to recently played
    ```

    If you open up **Files/RecentlyPlayed.txt**, you should see the new song added to the list.

### Task 2: Provide personalized song recommendations

In this task, you create a prompt that provides personalized song recommendations to the user based on their recently played songs. The prompt combines the native functions to generate a song recommendation. You also create a function from the prompt to make it reusable.

1. In your **Program.cs** file, add the following code under the comment **Create a song suggester function using a prompt**:

    ```c#
    // Create a song suggester function using a prompt
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

1. Under the comment **Invoke the song suggester function with a prompt from the user***, add the following code:

    ```c#
    // Invoke the song suggester function with a prompt from the user
    chatHistory.AddUserMessage("What song should I play next?");
    await GetAssistantReply();
    ```

    Now your application can automatically invoke your plugin functions according to the user's request. Let's extend this code to provide concert recommendations as well based on the user's information.

### Task 3: Provide personalized concert recommendations

In this task, you create a plugin that asks the LLM to suggest a concert based on the user's recently played songs and location.

1. In your **Program.cs** file, import the concerts plugin under the **Import plugins to the kernel** comment:

    ```c#
    // Import plugins to the kernel 
    kernel.ImportPluginFromType<MusicLibraryPlugin>();
    kernel.ImportPluginFromType<MusicConcertsPlugin>();
    ```

1. Add the following code under the comment **Create a concert suggester function using a prompt**:

    ```c#
    // Create a concert suggester function using a prompt
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
    // Invoke the concert suggester function with a prompt from the user
    chatHistory.AddUserMessage("Can you recommend a concert for me? I live in Washington");
    await GetAssistantReply();
    ```

1. In the terminal, enter `dotnet run`

    You should see output similar to the following response:

    ```output
    I recommend you attend the concert of Lisa Taylor. She will be performing in Seattle, Washington, USA on 2/22/2024. Enjoy the show!
    ```
    
    Your response from the LLM may vary. Try tweaking your prompt and location to see what other results you can generate.

Now your assistant is able to automatically perform different actions based on the user's input. Great work!

### Review

In this lab, you created an AI assistant that can manage the user's music library and provide personalized song and concert recommendations. You used the Semantic Kernel SDK to build the AI assistant and connect it to the large language model (LLM) service. You created custom plugins for your music library, enabled automatic function calling to make your assistant dynamically respond to the user's input. Congratulations on completing this lab!
