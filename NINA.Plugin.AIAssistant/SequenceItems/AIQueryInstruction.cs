using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Core.Utility.Notification;
using NINA.Sequencer.SequenceItem;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NINA.Plugin.AIAssistant.AI;
using NINA.Core.Utility;

namespace NINA.Plugin.AIAssistant.SequenceItems
{
    [ExportMetadata("Name", "AI Query Assistant")]
    [ExportMetadata("Description", "Ask the AI assistant any astrophotography question")]
    [ExportMetadata("Icon", "QuestionSVG")]
    [ExportMetadata("Category", "AI Assistant")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class AIQueryInstruction : SequenceItem
    {
        private readonly AIService _aiService;

        [ImportingConstructor]
        public AIQueryInstruction()
        {
            var plugin = (AIAssistantPlugin?)Utility.GetPluginInstance("NINA.Plugin.AIAssistant");
            _aiService = plugin?.GetAIService() ?? throw new InvalidOperationException("AI Assistant plugin not loaded");
        }

        [JsonProperty]
        public string Question { get; set; } = "What are the best practices for imaging nebulae?";

        public override async Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token)
        {
            try
            {
                var status = new ApplicationStatus
                {
                    Status = "Querying AI assistant..."
                };
                progress.Report(status);

                var request = new AIRequest
                {
                    Prompt = Question,
                    Temperature = 0.7,
                    MaxTokens = 600
                };

                var response = await _aiService.SendRequestAsync(request, token);

                if (response.Success && !string.IsNullOrEmpty(response.Content))
                {
                    Logger.Info($"AI Response: {response.Content}");
                    
                    Notification.ShowInformation($"AI Assistant:\n\n{response.Content}");
                    
                    status.Status = "AI response received";
                    progress.Report(status);
                }
                else
                {
                    Logger.Error($"AI query failed: {response.Error}");
                    Notification.ShowError($"AI query failed: {response.Error}");
                    throw new Exception(response.Error ?? "Unknown error");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error querying AI assistant: {ex.Message}");
                Notification.ShowError($"AI query error: {ex.Message}");
                throw;
            }
        }

        public override object Clone()
        {
            return new AIQueryInstruction
            {
                Question = Question
            };
        }

        public override string ToString()
        {
            return $"Category: AI Assistant, Item: AI Query";
        }
    }
}
