using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LIFEViewProtocol.AgentsAndEvents;
using LIFEViewProtocol.Basics;


namespace MARSLocalStarter.WinForms
{
    internal class PaintableObject : IDisposable
    {
        public Color Color { get; set; }
        public RectangleF Rectangle { get; set; }
        public Image Sprite { get; set; }

        public void Dispose()
        {
            using (Sprite) Sprite = null;
        }

        internal static HashSet<PaintableObject> Translate(List<BasicVisualizationMessage> visualizationMessages)
        {
            var result = new HashSet<PaintableObject>();

            var agentData = visualizationMessages.Where(msg => msg is BasicAgent);
            var passivObjectData = visualizationMessages.Where(msg => msg is BasicPassiveObject);

            foreach (var passivObject in passivObjectData)
            {
                var basicPassiveObject = passivObject as BasicPassiveObject;
                if (basicPassiveObject == null) continue;


            }



            return result;
        }
    }
}
