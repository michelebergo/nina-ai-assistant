using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Core.Utility.Notification;
using NINA.Sequencer.SequenceItem;
using NINA.Equipment.Interfaces.Mediator;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NINA.Plugin.AIAssistant.AI;
using NINA.Core.Utility;

namespace NINA.Plugin.AIAssistant.SequenceItems
{
    [ExportMetadata("Name", "AI Suggest Exposure")]
    [ExportMetadata("Description", "Get AI recommendations for optimal exposure time based on target and conditions")]
    [ExportMetadata("Icon", "LightbulbSVG")]
    [ExportMetadata("Category", "AI Assistant")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class AISuggestExposureInstruction : SequenceItem
    {
        private readonly AIService _aiService;
        private readonly ICameraMediator _cameraMediator;

        [ImportingConstructor]
        public AISuggestExposureInstruction(ICameraMediator cameraMediator)
        {
            _cameraMediator = cameraMediator;
            
            var plugin = (AIAssistantPlugin?)Utility.GetPluginInstance("NINA.Plugin.AIAssistant");
            _aiService = plugin?.GetAIService() ?? throw new InvalidOperationException("AI Assistant plugin not loaded");
        }

        [JsonProperty]
        public string TargetName { get; set; } = "M42";

        [JsonProperty]
        public string FilterType { get; set; } = "Luminance";

        [JsonProperty]
        public double CurrentSkyBrightness { get; set; } = 20.5; // mag/arcsec²

        public override async Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token)
        {
            try
            {
                var status = new ApplicationStatus
                {
                    Status = "Getting AI exposure recommendations..."
                };
                progress.Report(status);

                var cameraInfo = _cameraMediator.GetInfo();
                var prompt = $@"As an astrophotography expert, suggest optimal exposure time for:

Target: {TargetName}
Filter: {FilterType}
Sky Brightness: {CurrentSkyBrightness} mag/arcsec²
Camera: {cameraInfo.Name}
Camera Gain: {cameraInfo.Gain}
Camera Offset: {cameraInfo.Offset}

Provide:
1. Recommended exposure time (in seconds)
2. Recommended number of sub-exposures
3. Brief explanation of the reasoning
4. Any additional tips for this target

Be concise and practical.";

                var request = new AIRequest
                {
                    Prompt = prompt,
                    Temperature = 0.5,
                    MaxTokens = 400
                };

                var response = await _aiService.SendRequestAsync(request, token);

                if (response.Success && !string.IsNullOrEmpty(response.Content))
                {
                    Logger.Info($"AI Exposure Suggestion: {response.Content}");
                    
                    Notification.ShowInformation($"AI Exposure Recommendations:\n\n{response.Content}");
                    
                    status.Status = "AI recommendations received";
                    progress.Report(status);
                }
                else
                {
                    Logger.Error($"AI suggestion failed: {response.Error}");
                    Notification.ShowError($"AI suggestion failed: {response.Error}");
                    throw new Exception(response.Error ?? "Unknown error");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting AI exposure suggestion: {ex.Message}");
                Notification.ShowError($"AI suggestion error: {ex.Message}");
                throw;
            }
        }

        public override object Clone()
        {
            return new AISuggestExposureInstruction(_cameraMediator)
            {
                TargetName = TargetName,
                FilterType = FilterType,
                CurrentSkyBrightness = CurrentSkyBrightness
            };
        }

        public override string ToString()
        {
            return $"Category: AI Assistant, Item: AI Suggest Exposure for {TargetName}";
        }
    }
}
