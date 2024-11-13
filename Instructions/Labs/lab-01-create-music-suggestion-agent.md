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

1. On the **Overview** page, select **Go to Azure OpenAI Studio**.

:::image type="content" source="../media/model-deployments.png" alt-text="A screenshot of the Azure OpenAI deployments page.":::

1.Go to **Model Catalog**, select **gpt-35-turbo-16k** and **Deploy**. 

1. Enter a name for your deployment and leave default options. Click **Deploy**.

1. When the deployment completes, navigate back to your Azure OpenAI resource in the Azure Portal.

1. Under **Resource Management**, go to **Keys and Endpoint**.

    You'll use the data here in the next task to build your kernel. Remember to keep your keys private and secure!

### Task 2: Build your kernel

In this exercise, you learn how to build your first Semantic Kernel SDK project. You learn how to create a new project, add the Semantic Kernel SDK NuGet package, and add a reference to the Semantic Kernel SDK. Let's get started!

1. Return to your Visual Studio Code project.

1. Open the Terminal by selecting **Terminal** > **New Terminal**.

1. In the Terminal, run the following command to install the Semantic Kernel SDK:

    `dotnet add package Microsoft.SemanticKernel --version 1.2.0`

1. To create the kernel, add the following code to your **Program.cs** file:
    
    ```c#
    using Microsoft.SemanticKernel;

    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(
        "your-deployment-name",
        "your-endpoint",
        "your-api-key");
    var kernel = builder.Build();
    ```

    Be sure to replace the placeholders with the values from your Azure resource.

1. To verify that your kernel and endpoint is working, enter the following code:

    ```c#
    var result = await kernel.InvokePromptAsync(
        "Who are the top 5 most famous musicians in the world?");
    Console.WriteLine(result);
    ```

1. Enter `dotnet run` to run the code and check that you see a response from the Azure Open AI model containing the top 5 most famous musicians in the world.

    The response comes from the Azure Open AI model you passed to the kernel. The Semantic Kernel SDK is able to connect to the large language model (LLM) and run the prompt. Notice how quickly you were able to receive responses from the LLM. The Semantic Kernel SDK makes building smart applications easy and efficient.

## Exercise 2: Create custom music library plugins

In this exercise, you create custom plugins for your music library. You create functions that can add songs to the user's recently played list, get the list of recently played songs, and provide personalized song recommendations. You also create a function that suggests a concert based on the user's location and their recently played songs.

**Estimated exercise completion time**: 15 minutes

### Task 1: Create a music library plugin

In this task, you create a plugin that allows you to add songs to the user's recently played list and get the list of recently played songs. For simplicity, the recently played songs are stored in a text file.

1. Create a new folder in the 'Starter' directory and name it 'Plugins.'

1. In the 'Plugins' folder, create a new file 'MusicLibraryPlugin.cs'

    First, create some quick functions to get and add songs to the user's "Recently Played" list.

1. Enter the following code:

    ```c#
    using System.ComponentModel;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using Microsoft.SemanticKernel;

    public class MusicLibraryPlugin
    {
        [KernelFunction, 
        Description("Get a list of music recently played by the user")]
        public static string GetRecentPlays()
        {
            string content = File.ReadAllText($"Files/RecentlyPlayed.txt");
            return content;
        }
    }
    ```

    In this code, you use the `KernelFunction` decorator to declare your native function. You also use the `Description` decorator to add a description of what the function does. The user's list of recent plays is stored in a text file called 'RecentlyPlayed.txt'. Next, you can add code to add a song to the list.

1. Add the following code to your `MusicLibraryPlugin` class:

    ```c#
    [KernelFunction, Description("Add a song to the recently played list")]
    public static string AddToRecentlyPlayed(
        [Description("The name of the artist")] string artist, 
        [Description("The title of the song")] string song, 
        [Description("The song genre")] string genre)
    {
        // Read the existing content from the file
        string filePath = "Files/RecentlyPlayed.txt";
        string jsonContent = File.ReadAllText(filePath);
        var recentlyPlayed = (JsonArray) JsonNode.Parse(jsonContent);

        var newSong = new JsonObject
        {
            ["title"] = song,
            ["artist"] = artist,
            ["genre"] = genre
        };

        recentlyPlayed.Insert(0, newSong);
        File.WriteAllText(filePath, JsonSerializer.Serialize(recentlyPlayed,
            new JsonSerializerOptions { WriteIndented = true }));

        return $"Added '{song}' to recently played";
    }
    ```

    In this code, you create a function accepts the artist, song, and genre as strings. In addition to the `Description` of the function, you also add descriptions of the input variables. The 'RecentlyPlayed.txt' file contains json formatted list of songs that the user has recently played. This code reads the existing content from the file, parses it, and adds the new song to the list. Afterwards, the updated list is written back to the file.

1. Update your **Program.cs** file with the following code under **var kernel = builder.Build();**:

    ```c#
    
    kernel.ImportPluginFromType<MusicLibraryPlugin>();

    var result = await kernel.InvokeAsync(
        "MusicLibraryPlugin", 
        "AddToRecentlyPlayed", 
        new() {
            ["artist"] = "Tiara", 
            ["song"] = "Danse", 
            ["genre"] = "French pop, electropop, pop"
        }
    );
    
    Console.WriteLine(result);
    ```

    In this code, you import the MusicLibraryPlugin to the kernel using ImportPluginFromType. Then you call InvokeAsync with the plugin name and function name that you want to call. You also pass in the artist, song, and genre as arguments.
1. Remove the following lines from Exercise 1:
  
    ```c#
      var result = await kernel.InvokePromptAsync(
          "Who are the top 5 most famous musicians in the world?");
    Console.WriteLine(result);
    ```
   
1. Run the code by entering `dotnet run` in the terminal.

    You should see the following output:

    ```output
    Added 'Danse' to recently played
    ```

    If you open up 'Files/RecentlyPlayed.txt,' you should see the new song added to the list.

> [!NOTE]
> If the terminal displays warnings for null values, you may ignore them as they will not affect the result.

### Task 2: Provide personalized song recommendations

In this task, you create a prompt that provides personalized song recommendations to the user based on their recently played songs. The prompt combines the native functions to generate a song recommendation. You also create a function from the prompt to make it reusable.

1. In your `MusicLibraryPlugin.cs` file, add the following function:

    ```c#
    [KernelFunction, Description("Get a list of music available to the user")]
    public static string GetMusicLibrary()
    {
        string dir = Directory.GetCurrentDirectory();
        string content = File.ReadAllText($"Files/MusicLibrary.txt");
        return content;
    }
    ```

    This function reads the list of available music from a file named 'MusicLibrary.txt'. The file contains a json formatted list of songs available to the user.

1. Replace your **Program.cs** file with the following code under **var kernel = builder.Build();**:

    ```c#
    
    kernel.ImportPluginFromType<MusicLibraryPlugin>();
    
    string prompt = @"This is a list of music available to the user:
        {{MusicLibraryPlugin.GetMusicLibrary}} 

        This is a list of music the user has recently played:
        {{MusicLibraryPlugin.GetRecentPlays}}

        Based on their recently played music, suggest a song from
        the list to play next";

    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine(result);
    ```

In this code, you combine your native functions with a semantic prompt. The native functions are able to retrieve user data that the large language model (LLM) couldn't access on its own, and the LLM is able to generate a song recommendation based on the text input.

1. To test your code, enter `dotnet run` in the terminal.

    You should see a response similar to the following output:

    ```output 
    Based on the user's recently played music, a suggested song to play next could be "Sabry Aalil" by Yasemin since the user seems to enjoy pop and Egyptian pop music.
    ```

    >[!NOTE] The recommended song may be different than the one shown here.

1. Replace  the code under **kernel.ImportPluginFromType<MusicLibraryPlugin>();**  to create a function from the prompt:

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

    var result = await kernel.InvokeAsync(songSuggesterFunction);
    Console.WriteLine(result);
    ```

    1. To test your code, enter `dotnet run` in the terminal.

    You should see a response similar to the following output:

    ```output 
    Based on their recently played music, I would suggest the song "Sofia" by Gaby from the list to play next.
    ```
In this code, you create a function from a prompt that suggests a song to the user. Then you add it to the kernel Plugins. Finally, you tell the kernel to run the function.

### Task 3: Provide personalized concert recommendations

In this task, you create a plugin that retrieves upcoming concert details. You also create a plugin that asks the LLM to suggest a concert based on the user's recently played songs and location.

1. In the 'Plugins' folder, create a new file named 'MusicConcertsPlugin.cs'

1. In the MusicConcertsPlugin' file, add the following code:

    ```c#
    using System.ComponentModel;
    using Microsoft.SemanticKernel;

    public class MusicConcertsPlugin
    {
        [KernelFunction, Description("Get a list of upcoming concerts")]
        public static string GetConcerts()
        {
            string content = File.ReadAllText($"Files/ConcertDates.txt");
            return content;
        }
    }
    ```

    In this code, you create a function that reads the list of upcoming concerts from a file named 'ConcertDates.txt'. The file contains a json formatted list of upcoming concerts. Next, you need to create a prompt to ask the LLM to suggest a concert.

1. In the 'Prompts' folder, create a new folder named 'SuggestConcert'

1. Create a 'config.json' file in the 'SuggestConcert' folder with the following content:

    ```json
    {
        "schema": 1,
        "type": "completion",
        "description": "Suggest a concert to the user",
        "execution_settings": {
            "default": {
                "max_tokens": 4000,
                "temperature": 0
            }
        },
        "input_variables": [
            {
                "name": "location",
                "description": "The user's location",
                "required": true
            }
        ]
    }
    ```

1. Create a 'skprompt.txt' file in the 'SuggestConcert' folder with the following content:

    ```output
    This is a list of the user's recently played songs:
    {{MusicLibraryPlugin.GetRecentPlays}}

    This is a list of upcoming concert details:
    {{MusicConcertsPlugin.GetConcerts}}

    Suggest an upcoming concert based on the user's recently played songs. 
    The user lives in {{$location}}, 
    please recommend a relevant concert that is close to their location.
    ```

    This prompt helps the LLM filter the user's input and retrieve just the destination from the text. Next, you test your plugins to verify the output.

1. Open your **Program.cs** file and update it with the following code:

    ```c#
    var kernel = builder.Build();    
    kernel.ImportPluginFromType<MusicLibraryPlugin>();
    kernel.ImportPluginFromType<MusicConcertsPlugin>();
    var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");

    var songSuggesterFunction = kernel.CreateFunctionFromPrompt(
      promptTemplate: @"This is a list of music available to the user:
      {{MusicLibraryPlugin.GetMusicLibrary}}
      This is a list of music the user has recently played:
      {{MusicLibraryPlugin.GetRecentPlays}}
      Based on their recently played music, suggest a song from    the list to play next",
      functionName: "SuggestSong",
      description: "Suggest a song to the user"
    );

    kernel.Plugins.AddFromFunctions("SuggestSongPlugin", [songSuggesterFunction]);

    string location = "Redmond WA USA";
    var result = await kernel.InvokeAsync<string>(prompts["SuggestConcert"],
        new() {
            { "location", location }
        }
    );
    Console.WriteLine(result);
    ```

1. In the terminal, enter `dotnet run`

    You should see output similar to the following response:

    ```output
    Based on the user's recently played songs and their location in Redmond WA USA, a relevant concert recommendation would be the upcoming concert of Lisa Taylor in Seattle WA, USA on February 22, 2024. Lisa Taylor is an indie-folk artist, and her music genre aligns with the user's recently played songs, such as "Loanh Quanh" by Ly Hoa. Additionally, Seattle is close to Redmond, making it a convenient location for the user to attend the concert.
    ```

    Try tweaking your prompt and location to see what other results you can generate.

## Exercise 3: Automate suggestions based on user input

You can avoid manually invoking the plugin functions by using automatic function calling instead. The LLM will automatically select and combine the plugins registered to the kernel to achieve the goal. In this exercise, you enable automatic function calling to automate recommendations.

**Estimated exercise completion time**: 10 minutes

### Task 1: Automate suggestions based on user input

In this task, you enable automatic function calling to generate suggestions based on the user's input.

1. In your **Program.cs** file, update your code to the following (keep **AddAzureOpenAIChatCompletion** settings values):

    ```c#
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Connectors.OpenAI;
    
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(
        "your-deployment-name",
        "your-endpoint",
        "your-api-key",
        "deployment-model");
    var kernel = builder.Build();
    kernel.ImportPluginFromType<MusicLibraryPlugin>();
    kernel.ImportPluginFromType<MusicConcertsPlugin>();
    kernel.ImportPluginFromPromptDirectory("Prompts");

    OpenAIPromptExecutionSettings settings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    string location = "Redmond WA USA";
    string prompt = @$"Based on the user's recently played music, suggest a 
        concert for the user living in ${location}";

    var autoInvokeResult = await kernel.InvokePromptAsync(prompt, new(settings));
    Console.WriteLine(autoInvokeResult);
    ```

1. In the terminal, enter `dotnet run`

    You should see a response similar to the following output:

    ```output
    Based on the user's recently played songs, the artist "Mademoiselle" has an upcoming concert in Seattle WA, USA on February 22, 2024, which is close to Redmond WA. Therefore, the recommended concert for the user would be Mademoiselle's concert in Seattle.
    ```

    The semantic kernel is able to automatically call the `SuggestConcert` function using the right parameters. Now your agent is able to suggest a concert to the user based on the list of recently played music and their location. Next you can add support for music recommendations.

1. Modify your **Program.cs** file with the following code:

    ```c#
    OpenAIPromptExecutionSettings settings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };
    
    var songSuggesterFunction = kernel.CreateFunctionFromPrompt(
        promptTemplate: @"Based on the user's recently played music:
        {{$recentlyPlayedSongs}}
        recommend a song to the user from the music library:
        {{$musicLibrary}}",
        functionName: "SuggestSong",
        description: "Recommend a song from the music library"
    );

    kernel.Plugins.AddFromFunctions("SuggestSongPlugin", [songSuggesterFunction]);

    string prompt = "Can you recommend a song from the music library?";

    var autoInvokeResult = await kernel.InvokePromptAsync(prompt, new(settings));
    Console.WriteLine(autoInvokeResult);
    ```

    In this code, you create a KernelFunction from a prompt template that tells the LLM how to suggest a song. Afterwards, register it to the kernel and invoke a prompt with the auto function calling setting enabled. The kernel is able to run the function and supply the correct parameters to complete the prompt.

1. In the terminal, enter `dotnet run` to run your code.

    The output generated should recommend a song to the user based on their recently played music. Your response may look similar to the following output:
    
    ```
    Based on your recently played music, I recommend you listen to the song "Luv(sic)". It falls under the genres of hiphop and rap, which aligns with some of your recently played songs. Enjoy!  
    ```

    Next, let's try a prompt to update the recently played songs list.

1. Update your **Program.cs** file with the following code:

    ```c#
    string prompt = @"Add this song to the recently played songs list:  title: 'Touch', artist: 'Cat's Eye', genre: 'Pop'";

    var result = await kernel.InvokePromptAsync(prompt, new(settings));

    Console.WriteLine(result);
    ```

1. Enter `dotnet run` in the terminal

    The output should be similar to the following:

    ```
    I have added the song 'Touch' by Cat's Eye to the recently played songs list.
    ```

    When you open the recentlyplayed.txt file, you should see the new song added to the top of the list.
    

Using the `AutoInvokeKernelFunctions` setting allows you to focus on building plugins to suit your user's needs. Now your agent is able to automatically perform different actions based on the user's input. Great work!

### Review

In this lab, you created an AI agent that can manage the user's music library and provide personalized song and concert recommendations. You used the Semantic Kernel SDK to build the AI agent and connect it to the large language model (LLM) service. You created custom plugins for your music library, enabled automatic function calling to make your agent dynamically respond to the user's input. Congratulations on completing this lab!
