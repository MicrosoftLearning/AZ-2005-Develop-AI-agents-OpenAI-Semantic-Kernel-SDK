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
* Use the Handlebars planner to automate tasks

## Lab Setup

### Prerequisites

To complete the exercise you need to have the following installed on your system:

* [Visual Studio Code](https://code.visualstudio.com)
* [The latest .NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
* [The C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) for Visual Studio Code


### Prepare your development environment

For these exercises, a starter project is available for you to use. Use the following steps to set up the starter project:

> [!IMPORTANT]
> You must have .NET Framework 8.0 installed as well as the extensions VS Code extensions for C# and NuGet Package Manager.

1. Download the zip file located at `https://github.com/MicrosoftLearning/AZ-2005-Develop-AI-agents-OpenAI-Semantic-Kernel-SDK/blob/master/Allfiles/Labs/01/Lab-01-Starter.zip`.

1. Extract the contents of the zip file to a location that is easy to find and remember, such as a folder on your Desktop.

1. Open Visual Studio Code and select **File** > **Open Folder**.

1. Navigate to the **Starter** folder you extracted and select **Select Folder**.

1. Open the **Program.cs** file in the code editor.

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

1. Select **Create New Deployment** then **Deploy Model**.

1. Under **Select a model**, select **gpt-35-turbo-16k**.

    Use the default Model version

1. Enter a name for your deployment

1. When the deployment completes, navigate back to your Azure OpenAI resource.

1. Under **Resource Management**, go to **Keys and Endpoint**.

    You'll use the data here in the next task to build your kernel. Remember to keep your keys private and secure!

### Task 2: Build your kernel

In this exercise, you learn how to build your first Semantic Kernel SDK project. You learn how to create a new project, add the Semantic Kernel SDK NuGet package, and add a reference to the Semantic Kernel SDK. Let's get started!

1. Return to your Visual Studio Code project.

1. Open the Terminal by selecting **Terminal** > **New Terminal**.

1. In the Terminal, run the following command to install the Semantic Kernel SDK:

    `dotnet add package Microsoft.SemanticKernel --version 1.2.0`

1. To create the kernel, add the following code to your 'Program.cs' file:
    
    ```c#
    using Microsoft.SemanticKernel;

    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(
        "your-deployment-name",
        "your-endpoint",
        "your-api-key",
        "deployment-model");
    var kernel = builder.Build();
    ```

    Be sure to replace the placeholders with the values from your Azure resource.

1. To verify that your kernel and endpoint is working, enter the following code:

    ```c#
    var result = await kernel.InvokePromptAsync(
        "Who are the top 5 most famous musicians in the world?");
    Console.WriteLine(result);
    ```

1. Run the code and check that you see a response from the Azure Open AI model containing the top 5 most famous musicians in the world.

    The response comes from the Azure Open AI model you passed to the kernel. The Semantic Kernel SDK is able to connect to the large language model (LLM) and run the prompt. Notice how quickly you were able to receive responses from the LLM. The Semantic Kernel SDK makes building smart applications easy and efficient.

## Exercise 2: Create custom music library plugins

In this exercise, you create custom plugins for your music library. You create functions that can add songs to the user's recently played list, get the list of recently played songs, and provide personalized song recommendations. You also create a function that suggests a concert based on the user's location and their recently played songs.

**Estimated exercise completion time**: 15 minutes

### Task 1: Create a music library plugin

In this task, you create a plugin that allows you to add songs to the user's recently played list and get the list of recently played songs. For simplicity, the recently played songs are stored in a text file.

1. Create a new folder in the 'Lab01-Project' directory and name it 'Plugins.'

1. In the 'Plugins' folder, create a new file 'MusicLibrary.cs'

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

1. Update your 'Program.cs' file with the following code:

    ```c#
    var kernel = builder.Build();
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

    In this code, you import the `MusicLibraryPlugin` to the kernel using `ImportPluginFromType`. Then you call `InvokeAsync` with the plugin name and function name that you want to call. You also pass in the artist, song, and genre as arguments.

1. Run the code by entering `dotnet run` in the terminal.

    You should see the following output:

    ```output
    Added 'Danse' to recently played
    ```

    If you open up 'RecentlyPlayed.txt,' you should see the new song added to the list.

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

1. Update your 'Program.cs' file with the following code:

    ```c#
    var kernel = builder.Build();
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

    var result = await kernel.InvokeAsync(songSuggesterFunction);
    Console.WriteLine(result);
    ```

    In this code, you create a function from a prompt that suggests a song to the user. Then you add it to the kernel Plugins. Finally, you tell the kernel to run the function.

### Task 3: Provide personalized concert recommendations

In this task, you create a plugin that retrieves upcoming concert details. You also create a plugin that asks the LLM to suggest a concert based on the user's recently played songs and location.

1. In the 'Plugins' folder, create a new file named 'MusicConcertPlugin.cs'

1. In the MusicConcertPlugin' file, add the following code:

    ```c#
    using System.ComponentModel;
    using Microsoft.SemanticKernel;

    public class MusicConcertPlugin
    {
        [KernelFunction, Description("Get a list of upcoming concerts")]
        public static string GetTours()
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

    This prompt helps the LLM filter the user's input and retrieve just the destination from the text. Next, you invoke the planner to create a plan that combines the plugins together to accomplish the goal.

1. Open your 'Program.cs' file and update it with the following code:

    ```c#
    var kernel = builder.Build();    
    kernel.ImportPluginFromType<MusicLibraryPlugin>();
    kernel.ImportPluginFromType<MusicConcertsPlugin>();
    var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");

    var songSuggesterFunction = kernel.CreateFunctionFromPrompt(
    // code omitted for brevity
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

## Exercise 3: Automate suggestions with Handlebars plan

The Handlebars planner is useful when you have several steps required to accomplish a task. Planners use AI to select and combine the plugins registered to the kernel into a series of steps to achieve a goal. In this exercise, you generate a plan template using the Handlebars planner and use it to automate suggestions.

**Estimated exercise completion time**: 10 minutes

### Task 1: Generate a plan template

In this task, you generate a plan template using the Handlebars planner. The plan template will be used to automate suggestions based on the user's input.

1. Install the Handlebars planner by entering the following in the terminal:

    `dotnet add package Microsoft.SemanticKernel.Planners.Handlebars --version 1.2.0-preview`

    Next, you replace the SuggestConcert prompt and use the Handlebars planner to perform the task instead.

1. In your 'Program.cs' file, update your code to the following:

    ```c#
    var kernel = builder.Build();
    kernel.ImportPluginFromType<MusicLibraryPlugin>();
    kernel.ImportPluginFromType<MusicConcertPlugin>();
    kernel.ImportPluginFromPromptDirectory("Prompts");

    var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions() { AllowLoops = true });

    string location = "Redmond WA USA";
    string goal = @$"Based on the user's recently played music, suggest a 
        concert for the user living in ${location}";

    var plan = await planner.CreatePlanAsync(kernel, goal);
    var result = await plan.InvokeAsync(kernel);

    Console.WriteLine($"{result}");
    ```

1. In the terminal, enter `dotnet run`

    You should see a response similar to the following output:

    ```output
    Based on the user's recently played songs, the artist "Mademoiselle" has an upcoming concert in Seattle WA, USA on February 22, 2024, which is close to Redmond WA. Therefore, the recommended concert for the user would be Mademoiselle's concert in Seattle.
    ```

    Next, you change the code to output the Handlebars plan template.

1. In your 'Program.cs' file, update your code to the following:

    ```c#
    var plan = await planner.CreatePlanAsync(kernel, goal);
    Console.WriteLine("Plan:");
    Console.WriteLine(plan);
    ```

    Now you're able to see the generated plan. Next, modify the plan to include song suggestions or adding a song to the user's recently played list.

1. Extend your code with the following snippet:

    ```c#
    var plan = await planner.CreatePlanAsync(kernel, 
        @$"If add song:
        Add a song to the user's recently played list.
        
        If concert recommendation:
        Based on the user's recently played music, suggest a concert for 
        the user living in a given location.

        If song recommendation:
        Suggest a song from the music library to the user based on their 
        recently played songs.");

    Console.WriteLine("Plan:");
    Console.WriteLine(plan);
    ```

1. Enter `dotnet run` in the terminal to see the output of the plans you created.

    You should see a template similar to the following output:

    ```output
    Plan:
    {{!-- Step 1: Identify Key Values --}}
    {{set "location" location}}
    {{set "addSong" addSong}}
    {{set "concertRecommendation" concertRecommendation}}
    {{set "songRecommendation" concertRecommendation}}

    {{!-- Step 2: Use the Right Helpers --}}
    {{#if addSong}}
        {{set "song" song}}
        {{set "artist" artist}}
        {{set "genre" genre}}
        {{set "songAdded" (MusicLibraryPlugin-AddToRecentlyPlayed artist=artist song=song genre=genre)}}  
        {{json songAdded}}
    {{/if}}

    {{#if concertRecommendation}}
        {{set "concertSuggested" (Prompts-SuggestConcert location=location recentlyPlayedSongs=recentlyPlayedSongs musicLibrary=musicLibrary)}}
        {{json concertSuggested}}
    {{/if}}

    {{#if songRecommendation}}
        {{set "songSuggested" (SuggestSongPlugin-SuggestSong recentlyPlayedSongs=recentlyPlayedSongs musicLibrary=musicLibrary)}}
        {{json songSuggested}}
    {{/if}}

    {{!-- Step 3: Output the Result --}}
    {{json "Goal achieved"}}
    ```

     Notice the `{{#if ...}}` syntax. This syntax acts as a conditional statement that the Handlebars planner can use, similar to a traditional `if`-`else` block in C#. The `if` statement must be closed with `{{/if}}`.

    Next, you use this generated template to create your own Handlebars plan. 

1. Create a new file named 'handlebarsTemplate.txt' with the following text:

    ```output
    {{set "addSong" addSong}}
    {{set "concertRecommendation" concertRecommendation}}
    {{set "songRecommendation" songRecommendation}}

    {{#if addSong}}
        {{set "song" song}}
        {{set "artist" artist}}
        {{set "genre" genre}}
        {{set addedSong (MusicLibraryPlugin-AddToRecentlyPlayed artist song genre)}}  
        Output The following content, do not make any modifications:
        {{json addedSong}}     
    {{/if}}

    {{#if concertRecommendation}}
        {{set "location" location}}
        {{set "concert" (Prompts-SuggestConcert location)}}
        Output The following content, do not make any modifications:
        {{json concert}}
    {{/if}}

    {{#if songRecommendation}}
        {{set "recentlyPlayedSongs" (MusicLibraryPlugin-GetRecentPlays)}}
        {{set "musicLibrary" (MusicLibraryPlugin-GetMusicLibrary)}}
        {{set "song" (SuggestSongPlugin-SuggestSong recentlyPlayedSongs musicLibrary)}}
        Output The following content, do not make any modifications:
        {{json song}}
    {{/if}}
    ```

    In this template, you add an instruction to the LLM not to perform any text generation to ensure the output is strictly managed by the plugins. Now let's try out the template!

### Task 2: Use the Handlebars planner to automate suggestions

In this task, you create a function from the Handlebars plan template and use it to automate suggestions based on the user's input.

1. Remove the handlebars plans by modifying your existing code:

    ```c#
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
    
    var songSuggesterFunction = kernel.CreateFunctionFromPrompt(
        promptTemplate: @"Based on the user's recently played music:
        {{$recentlyPlayedSongs}}
        recommend a song to the user from the music library:
        {{$musicLibrary}}",
        functionName: "SuggestSong",
        description: "Suggest a song to the user"
    );

    kernel.Plugins.AddFromFunctions("SuggestSongPlugin", [songSuggesterFunction]);
    ```

1. Add code that reads the template file creates a function:

    ```c#
    string template = File.ReadAllText($"handlebarsTemplate.txt");

    var handlebarsPromptFunction = kernel.CreateFunctionFromPrompt(
        new() {
            Template = template,
            TemplateFormat = "handlebars"
        }, new HandlebarsPromptTemplateFactory()
    );
    ```

    In this code, you pass a `Template` object to the kernel method `CreateFunctionFromPrompt` along with the `TemplateFormat`. `CreateFunctionFromPrompt` also accepts an `IPromptTemplateFactory` type that tells the kernel how to parse a given template. Since you're using a Handlebars template, you use the `HandlebarsPromptTemplateFactory` type.

    Next let's run the function with some arguments and check out the results!

1. Add the following code to your `Program.cs` file:

    ```c#
    string location = "Redmond WA USA";
    var templateResult = await kernel.InvokeAsync(handlebarsPromptFunction,
        new() {
            { "location", location },
            { "concertRecommendation", true },
            { "songRecommendation", false },
            { "addSong", false },
            { "artist", "" },
            { "song", "" },
            { "genre", "" }
        });

    Console.WriteLine(templateResult);
    ```

1. Enter `dotnet run` in the terminal to see the output of your planner template.

    You should see a response similar to the following output:

    ```output
    Based on the user's recently played songs, Ly Hoa seems to be a relevant artist. The closest concert to Redmond WA, USA would be in Portland OR, USA on April 16th, 2024.  
    ```

    The prompt was able to suggest a concert to the user based on the list of recently played music and their location. You can also try setting other variables to true and see what happens!

Now your code is able to perform different actions based on the user's input. Great work!

### Review

In this lab, you created an AI agent that can manage the user's music library and provide personalized song and concert recommendations. You used the Semantic Kernel SDK to build the AI agent and connect it to the large language model (LLM) service. You created custom plugins for your music library, used the Handlebars planner to automate suggestions, and created a function from the Handlebars plan template to automate suggestions based on the user's input. Congratulations on completing this lab!