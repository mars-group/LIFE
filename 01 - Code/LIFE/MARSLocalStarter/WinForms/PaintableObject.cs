using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageWrappers;

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
                var vegetationPassivObject = passivObject as VegetationPassiveObject;
                if (basicPassiveObject == null) continue;

                var agentPoint = new PaintableObject()
                {
                    Rectangle = new RectangleF(
                        new PointF(basicPassiveObject.X, basicPassiveObject.Y),
                        new SizeF(vegetationPassivObject != null ? vegetationPassivObject.Width : 1, vegetationPassivObject != null ? vegetationPassivObject.Height : 1)),
                    Color = Color.FromArgb(200, Color.FromArgb(basicPassiveObject.Description.GetHashCode())),
                };
                result.Add(agentPoint);
            }

            foreach (var agent in agentData.Where(agent => agent != null))
            {
                var basicAgent = agent as BasicAgent;
                var movingBasicAgent = agent as MovingBasicAgent;
                if (basicAgent == null) continue;

                var agentPoint = new PaintableObject()
                {
                    Rectangle = new RectangleF(new PointF(basicAgent.X, basicAgent.Y),
                        new SizeF(movingBasicAgent != null ? movingBasicAgent.Width : 1, movingBasicAgent != null ? movingBasicAgent.Height : 1)),
                    Color = Color.FromArgb(255, 204, 134, 29)
                };
                result.Add(agentPoint);
            }

            return result;
        }
    }
}
