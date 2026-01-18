namespace NINA.Plugin.AIAssistant.AI
{
    /// <summary>
    /// Represents the different AI provider types supported by the plugin
    /// </summary>
    public enum AIProviderType
    {
        /// <summary>
        /// GitHub Models - Free tier available, perfect for getting started
        /// </summary>
        GitHub,

        /// <summary>
        /// Microsoft Foundry (formerly Azure AI Foundry) - Enterprise-grade deployment
        /// </summary>
        MicrosoftFoundry,

        /// <summary>
        /// Azure OpenAI Service
        /// </summary>
        AzureOpenAI,

        /// <summary>
        /// OpenAI API - Direct access to OpenAI models
        /// </summary>
        OpenAI,

        /// <summary>
        /// Anthropic Claude models
        /// </summary>
        Anthropic,

        /// <summary>
        /// Google AI (Gemini models)
        /// </summary>
        GoogleAI
    }

    /// <summary>
    /// Configuration for an AI provider
    /// </summary>
    public class AIProviderConfig
    {
        public AIProviderType ProviderType { get; set; }
        public string? ApiKey { get; set; }
        public string? Endpoint { get; set; }
        public string? ModelId { get; set; }
        public bool IsEnabled { get; set; }
        public string? DeploymentName { get; set; } // For Azure deployments
        public string? ProjectName { get; set; } // For Foundry projects
    }

    /// <summary>
    /// Represents an AI request to analyze image data or make decisions
    /// </summary>
    public class AIRequest
    {
        public string Prompt { get; set; } = string.Empty;
        public byte[]? ImageData { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public double Temperature { get; set; } = 0.7;
        public int MaxTokens { get; set; } = 1000;
    }

    /// <summary>
    /// Response from an AI provider
    /// </summary>
    public class AIResponse
    {
        public bool Success { get; set; }
        public string? Content { get; set; }
        public string? Error { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public string? ModelUsed { get; set; }
        public int? TokensUsed { get; set; }
    }
}
