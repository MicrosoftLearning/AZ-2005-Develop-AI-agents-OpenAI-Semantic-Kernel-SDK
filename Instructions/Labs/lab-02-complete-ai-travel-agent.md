---
lab:
  title: "Lab: Develop AI agents using Azure OpenAI and the Semantic Kernel SDK"
  module: "Module 01: Build your kernel"
---

# Lab: Complete an AI travel assistant
# Student lab manual

In this lab, you will complete an AI travel assistant using the Semantic Kernel SDK. You will create an endpoint for the large language model (LLM) service, create Semantic Kernel functions, and use the automatic function calling capability of the Semantic Kernel SDK to route the user's intent to the appropriate plugins, including some prebuilt plugins that have been provided. You will also provide context to the LLM by using conversation history and allow the user to continue the conversation.

## Lab Scenario

You are a developer at a travel agency that specializes in creating personalized travel experiences for your customers. You have been tasked with creating an AI travel assistant that can help customers learn more about travel destinations and plan activities for their trips. The AI travel assistant should be able to convert currency amounts, suggest destinations and activities, provide helpful phrases in different languages, and translate phrases. The AI travel assistant should also be able to provide contextually relevant responses to the user's requests by using conversation history.

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
* [The latest .NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [The C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) for Visual Studio Code

### Prepare your development environment

For these exercises, a starter project is available for you to use. Use the following steps to set up the starter project:

> [!IMPORTANT]
> You must have .NET Framework 8.0 installed as well as the extensions VS Code extensions for C# and NuGet Package Manager.

1. Paste the following URL into a new browser window:
   
     `https://github.com/MicrosoftLearning/AZ-2005-Develop-AI-agents-OpenAI-Semantic-Kernel-SDK/blob/master/Allfiles/Labs/02/Lab-02-Starter.zip`

1. Download the zip file by clicking the <kbd>...</kbd> button located on the upper right side of the page, or press <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>S</kbd>.

1. Extract the contents of the zip file to a location that is easy to find and remember, such as a folder on your Desktop.

1. Open Visual Studio Code and select **File** > **Open Folder**.

1. Navigate to the **Starter** folder you extracted and select **Select Folder**.

1. Open the **Program.cs** file in the code editor.

> [!NOTE]
> If prompted to trust the files in the folder, select **Yes, I trust the authors**

## Exercise 1: Create a plugin with the Semantic Kernel SDK

For this exercise, you create an endpoint for the large language model (LLM) service. The Semantic Kernel SDK uses this endpoint to connect to the LLM and run prompts. The Semantic Kernel SDK supports HuggingFace, OpenAI, and Azure Open AI LLMs. For this example, you use Azure Open AI.

**Estimated exercise completion time**: 10 minutes

### Task 1: Create an Azure OpenAI resource

1. Navigate to [https://portal.azure.com](https://portal.azure.com).

1. Create a new Azure OpenAI resource using the default settings.

    > [!NOTE]
    > If you already have an Azure OpenAI resource, you can skip this step.

1. After the resource is created, select **Go to resource**.

1. On the **Overview** page, select **Go to Azure Foundry portal**.

1. Select **Create New Deployment** then **from base models**.

1. On the models list, select **gpt-35-turbo-16k**.

1. Select **Confirm**

1. Enter a name for your deployment and leave the default options.

1. When the deployment completes, navigate back to your Azure OpenAI resource in the Azure Portal.

1. Under **Resource Management**, go to **Keys and Endpoint**.

    You'll use the data here in the next task to build your kernel. Remember to keep your keys private and secure!

### Task 2: Create a native plugin

In this task, you create a native function plugin that can convert an amount from a base currency to a target currency.

1. Return to your Visual Studio Code project.

1. Open the **appsettings.json** file and update the values with your Azure OpenAI Services model id, endpoint, and API key.

    ```json
    {
        "modelId": "gpt-35-turbo-16k",
        "endpoint": "",
        "apiKey": ""
    }
    ```

1. Navigate to the file named **CurrencyConverterPlugin.cs** in the **Plugins** folder

1. In the **CurrencyConverterPlugin.cs** file, add the following code under the comment **Create a kernel function that gets the exchange rate**:

    ```c#
    // Create a kernel function that gets the exchange rate
    [KernelFunction("convert_currency")]
    [Description("Converts an amount from one currency to another, for example USD to EUR")]
    public static decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        decimal exchangeRate = GetExchangeRate(fromCurrency, toCurrency);
        return amount * exchangeRate;
    }
    ```

    In this code, you use the **KernelFunction** decorator to declare your native function. You also use the **Description** decorator to add a description of what the function does. Next, you add some logic to convert a given amount from one currency to another.

1. Open the **Program.cs** file

1. Import the currency converter plugin under the comment **Add plugins to the kernel**:

    ```c#
    // Add plugins to the kernel
    kernel.ImportPluginFromType<CurrencyConverterPlugin>();
    ```

    Next, let's test your plugin.

1. Right-click your **Program.cs** file and click "Open in Integrated Terminal"

1. In the terminal, enter `dotnet run`. 

    Enter a prompt request to convert currency, for example "How much is 10 USD in Hong Kong?"

    You should see some output similar to the following:

    ```output
    Assistant: 10 USD is equivalent to 77.70 Hong Kong dollars (HKD).
    ```

## Exercise 2: Create a Handlebars prompt

In this exercise, you'll create a function from a Handlebars prompt. The function will prompt the LLM to create a travel itinerary for the user. Let's get started!

**Estimated exercise completion time**: 10 minutes

### Task 1: Create a function from a Handlebars prompt

1. Add the following `using` directive to the **Program.cs** file:

    `using Microsoft.SemanticKernel.PromptTemplates.Handlebars;`

1. Add the following code under the comment **Create a handlebars prompt**:

    ```c#
    // Create a handlebars prompt
    string hbprompt = """
        <message role="system">Instructions: Before providing the user with a travel itinerary, ask how many days their trip is</message>
        <message role="user">I'm going to {{city}}. Can you create an itinerary for me?</message>
        <message role="assistant">Sure, how many days is your trip?</message>
        <message role="user">{{input}}</message>
        <message role="assistant">
        """;
    ```

    In this code, you create a few-shot prompt using the Handlebars template format. The prompt will guide the model to retrieve more information from the user before creating a travel itinerary.

1. Add the following code under the comment **Create the prompt template config using handlebars format**:

    ```c#
    // Create the prompt template config using handlebars format
    var templateFactory = new HandlebarsPromptTemplateFactory();
    var promptTemplateConfig = new PromptTemplateConfig()
    {
        Template = hbprompt,
        TemplateFormat = "handlebars",
        Name = "GetItinerary",
    };
    ```

    This code creates a Handlebars template configuration from the prompt. You can use it to create a plugin function.

1. Add the following code under the comment **Create a plugin function from the prompt**: 

    ```c#
    // Create a plugin function from the prompt
    var promptFunction = kernel.CreateFunctionFromPrompt(promptTemplateConfig, templateFactory);
    var itineraryPlugin = kernel.CreatePluginFromFunctions("TravelItinerary", [promptFunction]);
    kernel.Plugins.Add(itineraryPlugin);
    ```

    This code creates a plugin function for the prompt and adds it to the kernel. Now you're ready to invoke your function.

1. Enter `dotnet run` in the terminal to run the code.

    Try the following input to prompt the LLM for an itinerary.

    ```output
    Assistant: How may I help you?
    User: I'm going to Hong Kong, can you create an itinerary for me?
    Assistant: Sure! How many days will you be staying in Hong Kong?
    User: 10
    Assistant: Great! Here's a 10-day itinerary for your trip to Hong Kong:
    ...
    ```

    Now you have the beginnings of an AI travel assistant. Let's use prompts and plugins to add more features

1.  Add the flight booking plugin under the comment **Add plugins to the kernel**:

    ```c#
    // Add plugins to the kernel
    kernel.ImportPluginFromType<CurrencyConverterPlugin>();
    kernel.ImportPluginFromType<FlightBookingPlugin>();
    ```

    This plugin simulates flight bookings using the **flights.json** file with mock details. Next, add some additional system prompts to the assistant.

1.  Add the following code under the comment **Add system messages to the chat**:

    ```c#
    // Add system messages to the chat
    history.AddSystemMessage("The current date is 01/10/2025");
    history.AddSystemMessage("You are a helpful travel assistant.");
    history.AddSystemMessage("Before providing destination recommendations, ask the user about their budget.");
    ```

    These prompts will help create a smooth user experience and help simulate the flight booking plugin. Now you're ready to test your code.

1. Enter `dotnet run` in the terminal.

    Try entering some of the following prompts:

    ```output
    1. Can you give me some destination recommendations for Europe?
    2. I want to go to Barcelona, can you create an itinerary for me?
    3. How many Euros is 100 USD?
    4. Can you book me a flight to Barcelona?
    ```

    Try other inputs and see how your travel assistant responds.

## Exercise 3: Require user consent for actions

In this exercise, you add a filter invocation function that will request the user's approval before allowing the assistant to book a flight on their behalf. Let's get started!

### Task 1: Create a function invocation filter

1. Create a new file named **PermissionFilter.cs**.

1. Enter the following code in the new file:

    ```c#
    #pragma warning disable SKEXP0001 
    using Microsoft.SemanticKernel;
    
    public class PermissionFilter : IFunctionInvocationFilter
    {
        public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
        {
            // Check the plugin and function names
            
            await next(context);
        }
    }
    ```

    >[!NOTE] 
    > In version 1.30.0 of the Semantic Kernel SDK, function filters are subject to change and require a warning suppression. 

    In this code, you implement the `IFunctionInvocationFilter` interface. The `OnFunctionInvocationAsync` method is always called whenever a function is invoked from an AI assistant.

1. Add the following code to detect when the `book_flight` function is invoked:

    ```c#
    // Check the plugin and function names
    if ((context.Function.PluginName == "FlightBookingPlugin" && context.Function.Name == "book_flight"))
    {
        // Request user approval

        // Proceed if approved
    }
    ```

    This code uses the `FunctionInvocationContext` to determine which plugin and function were invoked.

1. Add the following logic to request the user's permission to book the flight:

    ```c#
    // Request user approval
    Console.WriteLine("System Message: The assistant requires an approval to complete this operation. Do you approve (Y/N)");
    Console.Write("User: ");
    string shouldProceed = Console.ReadLine()!;

    // Proceed if approved
    if (shouldProceed != "Y")
    {
        context.Result = new FunctionResult(context.Result, "The operation was not approved by the user");
        return;
    }
    ```

1. Navigate to the **Program.cs** file.

1. Add the following code under the comment **Add filters to the kernel**:

    ```c#
    // Add filters to the kernel
    kernel.FunctionInvocationFilters.Add(new PermissionFilter());
    ```

1. Enter `dotnet run` in the terminal.

    Enter a prompt to book a flight. You should see a response similar to the following:

    ```output
    User: Find me a flight to Tokyo on the 19
    Assistant: I found a flight to Tokyo on the 19th of January. The flight is with Air Japan and the price is $1200.
    User: Y
    System Message: The assistant requires an approval to complete this operation. Do you approve (Y/N)
    User: N
    Assistant: I'm sorry, but I am unable to book the flight for you.
    ```

    The assistant should require user approval before proceeding with any bookings.

### Review

In this lab, you created an endpoint for the large language model (LLM) service, built a Semantic Kernel object, and ran prompts using the Semantic Kernel SDK. You also created plugins and leveraged system messages to guide the model. Congratulations on completing this lab!
