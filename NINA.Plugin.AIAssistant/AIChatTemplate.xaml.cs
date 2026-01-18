using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace NINA.Plugin.AIAssistant
{
    [Export(typeof(ResourceDictionary))]
    public partial class AIChatTemplate : ResourceDictionary
    {
        public AIChatTemplate()
        {
            InitializeComponent();
        }
    }
}
