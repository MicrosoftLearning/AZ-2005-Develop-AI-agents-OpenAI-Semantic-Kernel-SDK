---
lab:
  title: "Lab: Develop AI agents using Azure OpenAI and the Semantic Kernel SDK"
  module: "Module 01: Build your kernel"
---

# Lab: Complete an AI travel agent
# Student lab manual

In this lab, you will complete an AI travel agent using the Semantic Kernel SDK. You will create an endpoint for the large language model (LLM) service, create Semantic Kernel functions, and use the automatic function calling capability of the Semantic Kernel SDK to route the user's intent to the appropriate plugins, including some prebuilt plugins that have been provided. You will also provide context to the LLM by using conversation history and allow the user to continue the conversation.

## Lab Scenario

You are a developer at a travel agency that specializes in creating personalized travel experiences for your customers. You have been tasked with creating an AI travel agent that can help customers learn more about travel destinations and plan activities for their trips. The AI travel agent should be able to convert currency amounts, suggest destinations and activities, provide helpful phrases in different languages, and translate phrases. The AI travel agent should also be able to provide contextually relevant responses to the user's requests by using conversation history.

## Objectives

By completing this lab, you will accomplish the following:

* Create an endpoint for the large language model (LLM) service
* Build a Semantic Kernel object
* Run prompts using the Semantic Kernel SDK
* Create Semantic Kernel functions and plugins
* Use the automatic function calling capability of the Semantic Kernel SDK

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

1. Download the zip file located at `https://github.com/MicrosoftLearning/AZ-2005-Develop-AI-agents-OpenAI-Semantic-Kernel-SDK/blob/master/Allfiles/Labs/02/Lab-02-Starter.zip`.

1. Extract the contents of the zip file to a location that is easy to find and remember, such as a folder on your Desktop.

1. Open Visual Studio Code and select **File** > **Open Folder**.

1. Navigate to the **Starter** folder you extracted and select **Select Folder**.

1. Open the **Program.cs** file in the code editor.

## Exercise 1: Create a plugin with the Semantic Kernel SDK

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

1. Select **Create New Deployment** then select **+Create New Deployment**.

1. On the **Deploy Model** pop-up, select **gpt-35-turbo-16k**.

    Use the default Model version

1. Enter a name for your deployment

1. When the deployment completes, navigate back to your Azure OpenAI resource.

1. Under **Resource Management**, go to **Keys and Endpoint**.

    You'll use these values in the next task to build your kernel. Remember to keep your keys private and secure!

1. Return to the **Program.cs** file in Visual Studio Code.

1. Update the following variables with your Azure Open AI Services deployment name, API key, endpoint

    ```csharp
    string yourDeploymentName = "";
    string yourEndpoint = "";
    string yourApiKey = "";
    ```

    > [!NOTE]
    > The deployment model must be "gpt-35-turbo-16k" for some of the Semantic Kernel SDK features to work.

### Task 2: Create a native function

In this task, you create a native function that can convert an amount from a base currency currency to a target currency.

1. Create a new file named `CurrencyConverter.cs` in the **Plugins/ConvertCurrency** folder

1. In the `CurrencyConverter.cs` file, add the following code to create a plugin function:

    ```c#
    using AITravelAgent;
    using System.ComponentModel;
    using Microsoft.SemanticKernel;

    class CurrencyConverter
    {
        [KernelFunction, 
        Description("Convert an amount from one currency to another")]
        public static string ConvertAmount()
        {
            var currencyDictionary = Currency.Currencies;
        }
    }
    ```

    In this code, you use the `KernelFunction` decorator to declare your native function. You also use the `Description` decorator to add a description of what the function does. You can use `Currency.Currencies` to get a dictionary of currencies and their exchange rates. Next, add some logic to convert a given amount from one currency to another.

1. Modify your `ConvertAmount` function with the following code:

    ```c#
    [KernelFunction, Description("Convert an amount from one currency to another")]
    public static string ConvertAmount(
        [Description("The target currency code")] string targetCurrencyCode, 
        [Description("The amount to convert")] string amount, 
        [Description("The starting currency code")] string baseCurrencyCode)
    {
        var currencyDictionary = Currency.Currencies;
        
        Currency targetCurrency = currencyDictionary[targetCurrencyCode];
        Currency baseCurrency = currencyDictionary[baseCurrencyCode];

        if (targetCurrency == null)
        {
            return targetCurrencyCode + " was not found";
        }
        else if (baseCurrency == null)
        {
            return baseCurrencyCode + " was not found";
        }
        else
        {
            double amountInUSD = Double.Parse(amount) * baseCurrency.USDPerUnit;
            double result = amountInUSD * targetCurrency.UnitsPerUSD;
            return @$"${amount} {baseCurrencyCode} is approximately 
                {result.ToString("C")} in {targetCurrency.Name}s ({targetCurrencyCode})";
        }
    }
    ```

    In this code, you use the `Currency.Currencies` dictionary to get the `Currency` object for the target and base currencies. You then use the `Currency` object to convert the amount from the base currency to the target currency. Finally, you return a string with the converted amount. Next, let's test your plugin.

1. In the `Program.cs` file, import and invoke your new plugin function with the following code:

    ```c#
    kernel.ImportPluginFromType<CurrencyConverter>();
    kernel.ImportPluginFromType<ConversationSummaryPlugin>();
    var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");

    var result = await kernel.InvokeAsync("CurrencyConverter", 
        "ConvertAmount", 
        new() {
            {"targetCurrencyCode", "USD"}, 
            {"amount", "52000"}, 
            {"baseCurrencyCode", "VND"}
        }
    );

    Console.WriteLine(result);
    ```

    In this code, you use the `ImportPluginFromType` method to import your plugin. Then you use the `InvokeAsync` method to invoke your plugin function. The `InvokeAsync` method takes the plugin name, function name, and a dictionary of parameters. Finally, you print the result to the console. Next, run the code to make sure it's working.

1. In the terminal, enter `dotnet run`. You should see the following output:

    ```output
    $52000 VND is approximately $2.13 in US Dollars (USD)
    ```

    Now that your plugin is working correctly, let's create a natural language prompt that can detect what currencies and amount the user wants to convert.

### Task 3: Parse user input with a prompt

In this task, you create a prompt that parses the user's input to identify the target currency, base currency, and amount to convert.

1. Create a new folder named `GetTargetCurrencies` in the **Prompts** folder

1. In the `GetTargetCurrencies` folder, create a new file named `config.json`

1. Enter the following text into the `config.json` file:

    ```output
    {
        "schema": 1,
        "type": "completion",
        "description": "Identify the target currency, base currency, and amount to convert",
        "execution_settings": {
            "default": {
                "max_tokens": 800,
                "temperature": 0
            }
        },
        "input_variables": [
            {
                "name": "input",
                "description": "Text describing some currency amount to convert",
                "required": true
            }
        ]
    }
    ```

1. In the `GetTargetCurrencies` folder, create a new file named `skprompt.txt`

1. Enter the following text into the `skprompt.txt` file:

    ```html
    <message role="system">Identify the target currency, base currency, and 
    amount from the user's input in the format target|base|amount</message>

    For example: 

    <message role="user">How much in GBP is 750.000 VND?</message>
    <message role="assistant">GBP|VND|750000</message>

    <message role="user">How much is 60 USD in New Zealand Dollars?</message>
    <message role="assistant">NZD|USD|60</message>

    <message role="user">How many Korean Won is 33,000 yen?</message>
    <message role="assistant">KRW|JPY|33000</message>

    <message role="user">{{$input}}</message>
    <message role="assistant">target|base|amount</message>
    ```

### Task 4: Check your work

In this task, you run your application and verify your code is working correctly. 

1. Test your new prompt by updating your `Program.cs` file with the following code:

    ```c#
    kernel.ImportPluginFromType<CurrencyConverter>();
    kernel.ImportPluginFromType<ConversationSummaryPlugin>();
    var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");

    var result = await kernel.InvokeAsync(prompts["GetTargetCurrencies"],
        new() {
            {"input", "How many Australian Dollars is 140,000 Korean Won worth?"}
        }
    );

    Console.WriteLine(result);
    ```

1. Enter `dotnet run` in the terminal. You should see the following output:

    ```output
    AUD|KRW|140000
    ```

    > [!NOTE]
    > If your code doesn't produce the output you expected, you can review the code in the **Solution** folder. You may need to adjust the prompt in the `skprompt.txt` file to produce more exact results.

Now you have a plugin that can convert an amount from one currency to another, and a prompt that can be used to parse the user's input into a format the `ConvertAmount` function can use. This will allow users to easily convert currency amounts using your AI travel agent.

## Exercise 2: Automate plugin selection based on user intent

In this exercise, you detect the user's intent and route the conversation to the desired plugins. You can use a provided plugin to retrieve the user's intent. Let's get started!

**Estimated exercise completion time**: 10 minutes

### Task 1: Use the GetIntent plugin

1. Update your `Program.cs` file with the following code:

    ```c#
    kernel.ImportPluginFromType<CurrencyConverter>();
    kernel.ImportPluginFromType<ConversationSummaryPlugin>();
    var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");

    Console.WriteLine("What would you like to do?");
    var input = Console.ReadLine();

    var intent = await kernel.InvokeAsync<string>(
        prompts["GetIntent"], 
        new() {{ "input",  input }}
    );

    ```

    In this code, you use the `GetIntent` prompt to detect the user's intent. You then store the intent in a variable called `intent`. Next, you route the intent to your `CurrencyConverter` plugin.

1. Add the following code to your `Program.cs` file:

    ```c#
    switch (intent) {
        case "ConvertCurrency": 
            var currencyText = await kernel.InvokeAsync<string>(
                prompts["GetTargetCurrencies"], 
                new() {{ "input",  input }}
            );
            var currencyInfo = currencyText!.Split("|");
            var result = await kernel.InvokeAsync("CurrencyConverter", 
                "ConvertAmount", 
                new() {
                    {"targetCurrencyCode", currencyInfo[0]}, 
                    {"baseCurrencyCode", currencyInfo[1]},
                    {"amount", currencyInfo[2]}, 
                }
            );
            Console.WriteLine(result);
            break;
        default:
            Console.WriteLine("Other intent detected");
            break;
    }
    ```

    The `GetIntent` plugin returns the following values: ConvertCurrency, SuggestDestinations, SuggestActivities, Translate, HelpfulPhrases, Unknown. You use a `switch` statement to route the user's intent to the appropriate plugin. 
    
    If the user's intent is to convert currency, you use the `GetTargetCurrencies` prompt to retrieve the currency information. Then you use the `CurrencyConverter` plugin to convert the amount.

    Next, you add some cases to handle the other intents. For now, let's use the automatic function calling capability of the Semantic Kernel SDK to route the intent to the available plugins.

1. Create the automatic function calling setting by adding the following code to your `Program.cs` file:

    ```c#
    kernel.ImportPluginFromType<CurrencyConverter>();
    kernel.ImportPluginFromType<ConversationSummaryPlugin>();
    var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");

    OpenAIPromptExecutionSettings settings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    Console.WriteLine("What would you like to do?");
    var input = Console.ReadLine();
    var intent = await kernel.InvokeAsync<string>(
        prompts["GetIntent"], 
        new() {{ "input",  input }}
    );
    ```

    Next, you add cases to the switch statement for the other intents.

1. Update your `Program.cs` file with the following code:

    ```c#
    switch (intent) {
        case "ConvertCurrency": 
            // ...Code you entered previously...
            break;
        case "SuggestDestinations":
        case "SuggestActivities":
        case "HelpfulPhrases":
        case "Translate":
            var autoInvokeResult = await kernel.InvokePromptAsync(input!, new(settings));
            Console.WriteLine(autoInvokeResult);
            break;
        default:
            Console.WriteLine("Other intent detected");
            break;
    }
    ```

    In this code, you use the `AutoInvokeKernelFunctions` setting to automatically call functions and prompts that are referenced in your kernel. If the user's intent is to convert currency, the `CurrencyConverter` plugin performs its task. 
    
    If the user's intent is to get destination or activity suggestions, translate a phrase, or get helpful phrases in a language, the `AutoInvokeKernelFunctions` setting automatically calls the existing plugins that were included in the project code.

    You can also add code to run the user's input as a prompt to the large language model (LLM) if it doesn't fall under any of these intent cases.

1. Update the default case with the following code:

    ```c#
    default:
        Console.WriteLine("Sure, I can help with that.");
        var otherIntentResult = await kernel.InvokePromptAsync(input!, new(settings));
        Console.WriteLine(otherIntentResult);
        break;
    ```

    Now if the user has a different intent, the LLM can handle the user's request. Let's try it out!

### Task 2: Check your work

In this task, you run your application and verify your code is working correctly. 

1. Enter `dotnet run` in the terminal. When prompted, enter some text similar to the following prompt:

    ```output
    What would you like to do?
    How many TTD is 50 Qatari Riyals?    
    ```

1. You should see output similar to the following response:

    ```output
    $50 QAR is approximately $93.10 in Trinidadian Dollars (TTD)
    ```

1. Enter `dotnet run` in the terminal. When prompted, enter some text similar to the following prompt:

    ```output
    What would you like to do?
    I want to go somewhere that has lots of warm sunny beaches and delicious, spicy food!
    ```

1. You should see output similar to the following response:

    ```output
    Based on your preferences for warm sunny beaches and delicious, spicy food, I have a few destination recommendations for you:

    1. Thailand: Known for its stunning beaches, Thailand offers a perfect combination of relaxation and adventure. You can visit popular beach destinations like Phuket, Krabi, or Koh Samui, where you'll find crystal-clear waters and white sandy shores. Thai cuisine is famous for its spiciness, so you'll have plenty of mouthwatering options to try, such as Tom Yum soup, Pad Thai, and Green Curry.

    2. Mexico: Mexico is renowned for its beautiful coastal regions and vibrant culture. You can explore destinations like Cancun, Playa del Carmen, or Tulum, which boast stunning beaches along the Caribbean Sea. Mexican cuisine is rich in flavors and spices, offering a wide variety of dishes like tacos, enchiladas, and mole sauces that will satisfy your craving for spicy food.

    ...

    These destinations offer a perfect blend of warm sunny beaches and delicious, spicy food, ensuring a memorable trip for you. Let me know if you need any further assistance or if you have any specific preferences for your trip!
    ```

1. Enter `dotnet run` in the terminal. When prompted, enter some text similar to the following prompt:

    ```output
    What would you like to do?
    Can you give me a recipe for chicken satay?

1. You should see a response similar to the following response:

    ```output
    Sure, I can help with that.
    Certainly! Here's a recipe for chicken satay:

    ...
    ```

    The intent should be routed to your default case and the LLM should handle the request for a chicken satay recipe.

    > [!NOTE]
    > If your code doesn't produce the output you expected, you can review the code in the **Solution** folder.

Next, let's modify the routing logic to provide some conversation history to certain plugins. Providing history allows the plugins to retrieve more contextually relevant responses to the user's requests.

### Task 3: Complete plugin routing

In this exercise, you use the conversation history to provide context to the large language model (LLM). You also adjust the code so that it allows the user to continue the conversation, just like a real chatbot. Let's get started!

1. Modify the code to use a `do`-`while` loop to accept the user's input:

    ```c#
    string input;

    do 
    {
        Console.WriteLine("What would you like to do?");
        input = Console.ReadLine();

        // ...
    }
    while (!string.IsNullOrWhiteSpace(input));
    ```

    Now you can keep the conversation going until the user enters a blank line.

1. Capture details about the user's trip by modifying the `SuggestDestinations` case:

    ```c#
    case "SuggestDestinations":
        chatHistory.AppendLine("User:" + input);
        var recommendations = await kernel.InvokePromptAsync(input!);
        Console.WriteLine(recommendations);
        break;
    ```

1. Use the trip details in the `SuggestActivities` case with the following code:

    ```c#
     case "SuggestActivities":
        var chatSummary = await kernel.InvokeAsync(
            "ConversationSummaryPlugin", 
            "SummarizeConversation", 
            new() {{ "input", chatHistory.ToString() }});
        break;
    ```

    In this code, you use the built-in `SummarizeConversation` function to summarize the chat with the user. Next, let's use the summary to suggest activities at the destination.

1. Extend the `SuggestActivities` case with the following code:

    ```c#
    var activities = await kernel.InvokePromptAsync(
        input,
        new () {
            {"input", input},
            {"history", chatSummary},
            {"ToolCallBehavior", ToolCallBehavior.AutoInvokeKernelFunctions}
    });

    chatHistory.AppendLine("User:" + input);
    chatHistory.AppendLine("Assistant:" + activities.ToString());
    
    Console.WriteLine(activities);
    break;
    ```

    In this code, you add `input` and `chatSummary` as kernel arguments. Then the kernel invokes the prompt and routes it to the `SuggestActivities` plugin. You also append the user's input and the assistant's response to the chat history and display the results. Next, you need to add the `chatSummary` variable to the `SuggestActivities` plugin.

1. Navigate to **Prompts/SuggestActivities/config.json** and open the file in Visual Studio Code

1. Under `input_variables`, add a variable for the chat history:

    ```json
    "input_variables": [
      {
          "name": "history",
          "description": "Some background information about the user",
          "required": false
      },
      {
          "name": "destination",
          "description": "The destination a user wants to visit",
          "required": true
      }
   ]
   ```

1. Navigate to **Prompts/SuggestActivities/skprompt.txt** and open the file

1. Replace the beginning half of the prompt with the following prompt that uses the chat history variable:

    ```html 
    You are an experienced travel agent. 
    You are helpful, creative, and very friendly. 
    Consider the traveler's background: {{$history}}
    ```

    Leave the rest of the prompt as is. Now the plugin uses the chat history to provide context to the LLM.

### Task 4: Check your work

In this task, you run your application and verify the code is working correctly.

1. Compare your updated switch cases to the following code:

    ```c#
    case "SuggestDestinations":
            chatHistory.AppendLine("User:" + input);
            var recommendations = await kernel.InvokePromptAsync(input!);
            Console.WriteLine(recommendations);
            break;
    case "SuggestActivities":

        var chatSummary = await kernel.InvokeAsync(
            "ConversationSummaryPlugin", 
            "SummarizeConversation", 
            new() {{ "input", chatHistory.ToString() }});

        var activities = await kernel.InvokePromptAsync(
            input!,
            new () {
                {"input", input},
                {"history", chatSummary},
                {"ToolCallBehavior", ToolCallBehavior.AutoInvokeKernelFunctions}
        });

        chatHistory.AppendLine("User:" + input);
        chatHistory.AppendLine("Assistant:" + activities.ToString());
        
        Console.WriteLine(activities);
        break;
    ```

1. Enter `dotnet run` in the terminal. When prompted, enter some text similar to the following:

    ```output
    What would you like to do?
    How much is 60 USD in new zealand dollars?
    ```

1. You should receive some output similar to the following:

    ```output
    $60 USD is approximately $97.88 in New Zealand Dollars (NZD)
    What would you like to do?
    ```

1. Enter a prompt for destination suggestions with some context cues, for example:

    ```output
    What would you like to do?
    I'm planning an anniversary trip with my spouse, but they are currently using a wheelchair and accessibility is a must. What are some destinations that would be romantic for us?
    ```

1. You should receive some output with recommendations of accessible destinations.

1. Enter a prompt for activity suggestions, for example:

    ```output
    What would you like to do?
    What are some things to do in Barcelona?
    ```

1. You should receive recommendations that fit within the previous context, for example, accessible activities in Barcelona similar to the following:

    ```output
    1. Visit the iconic Sagrada Família: This breathtaking basilica is an iconic symbol of Barcelona's architecture and is known for its unique design by Antoni Gaudí.

    2. Explore Park Güell: Another masterpiece by Gaudí, this park offers stunning panoramic views of the city, intricate mosaic work, and whimsical architectural elements.

    3. Visit the Picasso Museum: Explore the extensive collection of artworks by the iconic painter Pablo Picasso, showcasing his different periods and styles.
    ```

    > [!NOTE]
    > If your code doesn't produce the output you expected, you can review the code in the **Solution** folder.

You can continue to test the application with different prompts and context cues. Great work! You've successfully provided context cues to the LLM and adjusted the code to allow the user to continue the conversation.

### Review

In this lab, you created an endpoint for the large language model (LLM) service, built a Semantic Kernel object, ran prompts using the Semantic Kernel SDK, created Semantic Kernel functions and plugins, and used the automatic function calling capability of the Semantic Kernel SDK to route the user's intent to the appropriate plugins. You also provided context to the LLM by using conversation history and allowed the user to continue the conversation. Congratulations on completing this lab!
