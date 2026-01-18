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
    [ExportMetadata("Name", "AI Image Analysis")]
    [ExportMetadata("Description", "Use AI to analyze the current image and provide quality assessment")]
    [ExportMetadata("Icon", "BrainSVG")]
    [ExportMetadata("Category", "AI Assistant")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class AIImageAnalysisInstruction : SequenceItem
    {
        private readonly AIService _aiService;

        [ImportingConstructor]
        public AIImageAnalysisInstruction()
        {
            // Get AI service from the plugin
            var plugin = (AIAssistantPlugin?)Utility.GetPluginInstance("NINA.Plugin.AIAssistant");
            _aiService = plugin?.GetAIService() ?? throw new InvalidOperationException("AI Assistant plugin not loaded");
        }

        [JsonProperty]
        public string AnalysisPrompt { get; set; } = "Analyze this astrophotography image. Assess star quality, focus, tracking accuracy, and any visible issues. Provide a brief quality score and recommendations.";

        public override async Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token)
        {
            try
            {
                var status = new ApplicationStatus
                {
                    Status = "Requesting AI image analysis..."
                };
                progress.Report(status);

                var request = new AIRequest
                {
                    Prompt = AnalysisPrompt,
                    Temperature = 0.3, // Lower temperature for more consistent analysis
                    MaxTokens = 500
                };

                var response = await _aiService.SendRequestAsync(request, token);

                if (response.Success && !string.IsNullOrEmpty(response.Content))
                {
                    Logger.Info($"AI Analysis Result: {response.Content}");
                    
                    Notification.ShowSuccess($"AI Analysis Complete:\n\n{response.Content}");
                    
                    status.Status = "AI analysis complete";
                    progress.Report(status);
                }
                else
                {
                    Logger.Error($"AI analysis failed: {response.Error}");
                    Notification.ShowError($"AI analysis failed: {response.Error}");
                    throw new Exception(response.Error ?? "Unknown error");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during AI image analysis: {ex.Message}");
                Notification.ShowError($"AI analysis error: {ex.Message}");
                throw;
            }
        }

        public override object Clone()
        {
            return new AIImageAnalysisInstruction
            {
                AnalysisPrompt = AnalysisPrompt
            };
        }

        public override string ToString()
        {
            return $"Category: AI Assistant, Item: AI Image Analysis";
        }
    }
}
