---
lab:
    title: 'Create a Devops Assistant with the Semantic Kernel SDK'
---

# Create a Devops Assistant with the Semantic Kernel SDK

In this lab, you create the code for an AI assistant that TODO. You use the Semantic Kernel SDK to build the AI assistant and connect it to the large language model (LLM) service. The Semantic Kernel SDK allows you to create a smart application that can interact with the LLM service and provide personalized recommendations to the user.

## Deploy a chat completion model

1. Navigate to [https://portal.azure.com](https://portal.azure.com).

1. Create a new Azure OpenAI resource using the default settings.

1. After the resource is created, select **Go to resource**.

1. On the **Overview** page, select **Go to Azure Foundry portal**.

1. Select **Create New Deployment** then **from base models**.

1. Search for **gpt-4o** on the model list, then select and confirm it.

1. Enter a name for your deployment and leave the default options.

1. When the deployment completes, navigate back to your Azure OpenAI resource in the Azure Portal.

1. Under **Resource Management**, go to **Keys and Endpoint**.

    You'll use the data here in the next task to build your kernel. Remember to keep your keys private and secure!

## Prepare the application configuration

1. Open a new browser tab (keeping the Azure AI Foundry portal open in the existing tab). Then in the new tab, browse to the [Azure portal](https://portal.azure.com) at `https://portal.azure.com`; signing in with your Azure credentials if prompted.

    Close any welcome notifications to see the Azure portal home page.

1. Use the **[\>_]** button to the right of the search bar at the top of the page to create a new Cloud Shell in the Azure portal, selecting a ***PowerShell*** environment with no storage in your subscription.

    The cloud shell provides a command-line interface in a pane at the bottom of the Azure portal. You can resize or maximize this pane to make it easier to work in.

    > **Note**: If you have previously created a cloud shell that uses a *Bash* environment, switch it to ***PowerShell***.

1. In the cloud shell toolbar, in the **Settings** menu, select **Go to Classic version** (this is required to use the code editor).

    **<font color="red">Ensure you've switched to the classic version of the cloud shell before continuing.</font>**

1. In the cloud shell pane, enter the following commands to clone the GitHub repo containing the code files for this exercise (type the command, or copy it to the clipboard and then right-click in the command line and paste as plain text):

    ```
    rm -r semantic-kernel -f
    git clone https://github.com/MicrosoftLearning/AZ-2005-Develop-AI-agents-OpenAI-Semantic-Kernel-SDK semantic-kernel
    ```

    > **Tip**: As you paste commands into the cloudshell, the output may take up a large amount of the screen buffer. You can clear the screen by entering the `cls` command to make it easier to focus on each task.

1. After the repo has been cloned, navigate to the folder containing the chat application code files:

    > **Note**: Follow the steps for your chosen programming language.

    **Python**
    ```
    cd semantic-kernel/Allfiles/Labs/Devops/python
    ```

    **C#**
    ```
    cd semantic-kernel/Allfiles/Labs/Devops/c-sharp
    ```

1. In the cloud shell command-line pane, enter the following command to install the libraries you'll use:

    **Python**
    ```
    python -m venv labenv
    ./labenv/bin/Activate.ps1
    pip install python-dotenv azure-identity semantic-kernel[azure] 
    ```

    **C#**
    ```
    dotnet add package Microsoft.Extensions.Configuration
    dotnet add package Microsoft.Extensions.Configuration.Json
    dotnet add package Microsoft.SemanticKernel
    dotnet add package Microsoft.SemanticKernel.PromptTemplates.Handlebars
    ```

1. Enter the following command to edit the configuration file that has been provided:

    **Python**
    ```
    code .env
    ```

    **C#**
    ```
    code appsettings.json
    ```

    The file is opened in a code editor.

1. Update the values with your Azure OpenAI Services model id, endpoint, and API key.

    **Python**
    ```python
    MODEL_DEPLOYMENT=""
    BASE_URL=""
    API_KEY="
    ```

    **C#**
    ```json
    {
        "modelName": "",
        "endpoint": "",
        "apiKey": ""
    }
    ```

1. After you've updated the values, use the **CTRL+S** command to save your changes and then use the **CTRL+Q** command to close the code editor while keeping the cloud shell command line open.

## Create a Semantic Kernel plugin

1. Enter the following command to edit the code file that has been provided:

    **Python**
    ```
    code devops.py
    ```

    **C#**
    ```
    code Program.cs
    ```

1. Add the following code under the comment **Create a kernel builder with Azure OpenAI chat completion**:

    **Python**
    ```python
    # Create a kernel builder with Azure OpenAI chat completion
    kernel = Kernel()
    chat_completion = AzureChatCompletion(
        deployment_name=deployment_name,
        api_key=api_key,
        base_url=base_url,
    )
    kernel.add_service(chat_completion)
    ```
    **C#**
     ```c#
    // Create a kernel builder with Azure OpenAI chat completion
    var builder = Kernel.CreateBuilder();
    builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
    var kernel = builder.Build();
    ```

1. Near the bottom of the file, find the comment **Create a kernel function to build the stage environment**, and add the following code to create a mock plugin functin that will build the staging environment:

    **Python**
    ```python
    # Create a kernel function to build the stage environment
    @kernel_function(name="BuildStageEnvironment")
    def build_stage_environment(self):
        return "Stage build completed."
    ```

    **C#**
    ```c#
    // Create a kernel function to build the stage environment
    [KernelFunction("BuildStageEnvironment")]
    public string BuildStageEnvironment() 
    {
        return "Stage build completed.";
    }
    ```

    The `KernelFunction` decorator declares your native function. You use a descriptive name for the function so that the AI can call it correctly. 

1. Navigate to the comment **Import plugins to the kernel** and add the following code:

    **Python**
    ```python
    # Import plugins to the kernel
    kernel.add_plugin(DevopsPlugin(), plugin_name="DevopsPlugin")
    ```

    **C#**
    ```c#
    // Import plugins to the kernel
    kernel.ImportPluginFromType<DevopsPlugin>();
    ```


1. Under the comment **Create prompt execution settings**, add the following code to automatically invoke the function:

    **Python**
    ```python
    # Create prompt execution settings
    execution_settings = AzureChatPromptExecutionSettings()
    execution_settings.function_choice_behavior = FunctionChoiceBehavior.Auto()
    ```

    **C#**
    ```c#
    // Create prompt execution settings
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };
    ```

    Using this setting will allow the kernel to automatically invoke functions without the need to specify them in the prompt.

1. Add the following code under the comment **Create chat history**:

    **Python**
    ```python
    # Create chat history
    chat_history = ChatHistory()
    ```

    **C#**
    ```c#
    // Create chat history
    var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    ChatHistory chatHistory = [];
    ```

1. Uncomment the code block located after the comment **User interaction logic**

1. Use the **CTRL+S** command to save your changes to the code file.

## Run your devops assistant code

1. In the cloud shell command-line pane, enter the following command to sign into Azure.

    ```
    az login
    ```

    **<font color="red">You must sign into Azure - even though the cloud shell session is already authenticated.</font>**

    > **Note**: In most scenarios, just using *az login* will be sufficient. However, if you have subscriptions in multiple tenants, you may need to specify the tenant by using the *--tenant* parameter. See [Sign into Azure interactively using the Azure CLI](https://learn.microsoft.com/cli/azure/authenticate-azure-cli-interactively) for details.

1. When prompted, follow the instructions to open the sign-in page in a new tab and enter the authentication code provided and your Azure credentials. Then complete the sign in process in the command line, selecting the subscription containing your Azure AI Foundry hub if prompted.

1. After you have signed in, enter the following command to run the application:


    **Python**
    ```
    python devops.py
    ```

    **C#**
    ```
    dotnet run
    ```

1. When prompted, enter the following prompt `Please build the stage environment`

1. You should see a response similar to the following output:

    ```output
    Assistant: The stage environment has been successfully built.
    ```

1. Next, enter the following prompt `Please deploy the stage environment`

1. You should see a response similar to the following output:

    ```output
    Assistant: The staging site has been deployed successfully.
    ```

1. Press <kbd>Enter</kbd> to end the program.

## Create a kernel function from a prompt

1. Add the following code under the comment `Create a kernel function to deploy the staging environment`

     **Python**
    ```python
    # Create a kernel function to deploy the staging environment
    deploy_stage_function = KernelFunctionFromPrompt(
        prompt="""This is the most recent build log:
        {{DevopsPlugin.ReadLogFile}}

        If there are errors, do not deploy the stage environment. Otherwise, invoke the stage deployment function""",
        function_name="DeployStageEnvironment",
        description="Deploy the staging environment"
    )

    kernel.add_function(plugin_name="DeployStageEnvironment", function=deploy_stage_function)
    ```

    **C#**
    ```c#
    // Create a kernel function to deploy the staging environment
    var deployStageFunction = kernel.CreateFunctionFromPrompt(
    promptTemplate: @"This is the most recent build log:
    {{DevopsPlugin.ReadLogFile}}

    If there are errors, do not deploy the stage environment. Otherwise, invoke the stage deployment function",
    functionName: "DeployStageEnvironment",
    description: "Deploy the staging environment"
    );

    kernel.Plugins.AddFromFunctions("DeployStageEnvironment", [deployStageFunction]);
    ```

1. Use the **CTRL+S** command to save your changes to the code file.

1. In the cloud shell command-line pane, enter the following command to run the application:

    **Python**
    ```
    python devops.py
    ```

    **C#**
    ```
    dotnet run
    ```

1. When prompted, enter the following prompt `Please deploy the stage environment`

1. You should see a response similar to the following output:

    ```output
    Assistant: The stage environment cannot be deployed because the earlier stage build failed due to unit test errors. Deploying a faulty build to stage may cause eventual issues and compromise the environment.
    ```

    Your response from the LLM may vary, but still prevent you from deploying the stage site.

## Create a handlebars prompt

1. Add the following code under the comment **Create a handlebars prompt**:

    **Python**
    ```python
    # Create a handlebars prompt
    hb_prompt = """<message role="system">Instructions: Before creating a new branch for a user, request the new branch name and base branch name/message>
        <message role="user">Can you create a new branch?</message>
        <message role="assistant">Sure, what would you like to name your branch? And which base branch would you like to use?</message>
        <message role="user">{{input}}</message>
        <message role="assistant">"""
    ```

    **C#**
    ```c#
    // Create a handlebars prompt
    string hbprompt = """
        <message role="system">Instructions: Before creating a new branch for a user, request the new branch name and base branch name/message>
        <message role="user">Can you create a new branch?</message>
        <message role="assistant">Sure, what would you like to name your branch? And which base branch would you like to use?</message>
        <message role="user">{{input}}</message>
        <message role="assistant">
        """;
    ```

    In this code, you create a few-shot prompt using the Handlebars template format. The prompt will guide the model to retrieve more information from the user before creating a new branch.

1. Add the following code under the comment **Create the prompt template config using handlebars format**:

    **Python**
    ```python
    # Create the prompt template config using handlebars format
    hb_template = HandlebarsPromptTemplate(
        prompt_template_config=PromptTemplateConfig(
            template=hb_prompt, 
            template_format="handlebars",
            name="CreateBranch", 
            description="Creates a new branch for the user",
            input_variables=[
                InputVariable(name="input", description="The user input", is_required=True)
            ]
        ),
        allow_dangerously_set_content=True,
    )
    ```

    **C#**
    ```c#
    // Create the prompt template config using handlebars format
    var templateFactory = new HandlebarsPromptTemplateFactory();
    var promptTemplateConfig = new PromptTemplateConfig()
    {
        Template = hbprompt,
        TemplateFormat = "handlebars",
        Name = "CreateBranch",
    };
    ```

    This code creates a Handlebars template configuration from the prompt. You can use it to create a plugin function.

1. Add the following code under the comment **Create a plugin function from the prompt**: 

    **Python**
    ```python
    # Create a plugin function from the prompt
    prompt_function = KernelFunctionFromPrompt(
        function_name="CreateBranch",
        description="Creates a branch for the user",
        template_format="handlebars",
        prompt_template=hb_template,
    )
    kernel.add_function(plugin_name="BranchPlugin", function=prompt_function)
    ```

    **C#**
    ```c#
    // Create a plugin function from the prompt
    var promptFunction = kernel.CreateFunctionFromPrompt(promptTemplateConfig, templateFactory);
    var branchPlugin = kernel.CreatePluginFromFunctions("BranchPlugin", [promptFunction]);
    kernel.Plugins.Add(branchPlugin);
    ```

    This code creates a plugin function for the prompt and adds it to the kernel. Now you're ready to invoke your function.

1. Use the **CTRL+S** command to save your changes to the code file.

1. In the cloud shell command-line pane, enter the following command to run the application:

    **Python**
    ```
    python devops.py
    ```

    **C#**
    ```
    dotnet run
    ```

1. When prompted, enter the following text: `Please create a new branch`

1. You should see a response similar to the following output:

    ```output
    Assistant: Could you please provide the following details?

    1. The name of the new branch.
    2. The base branch from which the new branch should be created.
    ```

1. Enter the following text `feature-login main`

1. You should see a response similar to the following output:

    ```output
    Assistant: The new branch `feature-login` has been successfully created from `main`.
    ```

## Require user consent for actions

1. Near the bottom of the file, find the comment **Create a function filter**, and add the following code:

    **Python**
    ```python
    async def permission_filter(context: FunctionInvocationContext, next: Callable[[FunctionInvocationContext], Awaitable[None]]) -> None:
        await next(context)
        result = context.result
        
        # Check the plugin and function names
    ```

    **C#**
    ```c#
    // Create a function filter
    class PermissionFilter : IFunctionInvocationFilter
    {
        public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
        {
            // Check the plugin and function names
            
            await next(context);
        }
    }
    ```

1. Add the following code under the comment **Check the plugin and function names** to detect when the `DeployToProd` function is invoked:

     **Python**
    ```python
    # Check the plugin and function names
    if context.function.plugin_name == "DevopsPlugin" and context.function.name == "DeployToProd":
        # Request user approval
        
        # Proceed if approved
    ```

    **C#**
    ```c#
    // Check the plugin and function names
    if ((context.Function.PluginName == "DevopsPlugin" && context.Function.Name == "DeployToProd"))
    {
        // Request user approval

        // Proceed if approved
    }
    ```

    This code uses the `FunctionInvocationContext` object to determine which plugin and function were invoked.

1. Add the following logic to request the user's permission to book the flight:

     **Python**
    ```python
    # Request user approval
    print("System Message: The assistant requires approval to complete this operation. Do you approve (Y/N)")
    should_proceed = input("User: ").strip()

    # Proceed if approved
    if should_proceed.upper() != "Y":
        context.result = FunctionResult(
            function=result.function,
            value="The operation was not approved by the user",
        )
    ```

    **C#**
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

1. Navigate to the comment **Add filters to the kernel** and add the following code:

    **Python**
    ```python
    # Add filters to the kernel
    kernel.add_filter('function_invocation', permission_filter)
    ```

    **C#**
    ```c#
    // Add filters to the kernel
    kernel.FunctionInvocationFilters.Add(new PermissionFilter());
    ```

1. Use the **CTRL+S** command to save your changes to the code file.

1. In the cloud shell command-line pane, enter the following command to run the application:

    **Python**
    ```
    python devops.py
    ```

    **C#**
    ```
    dotnet run
    ```

1. Enter a prompt to deploy the build to production. You should see a response similar to the following:

    ```output
    User: Please deploy the build to prod
    System Message: The assistant requires an approval to complete this operation. Do you approve (Y/N)
    User: N
    Assistant: I'm sorry, but I am unable to proceed with the deployment.
    ```

### Review

In this lab, you created an endpoint for the large language model (LLM) service, built a Semantic Kernel object, and ran prompts using the Semantic Kernel SDK. You also created plugins and leveraged system messages to guide the model. Congratulations on completing this lab!