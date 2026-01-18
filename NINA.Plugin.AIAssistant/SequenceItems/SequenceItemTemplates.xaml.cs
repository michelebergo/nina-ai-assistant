using System.ComponentModel.Composition;
using System.Windows;

namespace NINA.Plugin.AIAssistant.SequenceItems
{
    [Export(typeof(ResourceDictionary))]
    public partial class SequenceItemTemplates : ResourceDictionary
    {
        public SequenceItemTemplates()
        {
            InitializeComponent();
        }
    }
}
