using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using log4net;
using SMConnector.TransportTypes;

namespace MARSLocalStarter.WinForms
{
    public partial class MarsMC : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MarsMC));

        private readonly SimulationManagerFacade.Interface.ISimulationManagerApplicationCore _core;
        private readonly LayerContainerFacade.Interfaces.ILayerContainerFacade _layerContainer;
        private ICollection<TModelDescription> _availableModels;
        private TModelDescription _selectedModel;

        public MarsMC(SimulationManagerFacade.Interface.ISimulationManagerApplicationCore core, LayerContainerFacade.Interfaces.ILayerContainerFacade layerContainer)
        {
            InitializeComponent();
            _core = core;
            _layerContainer = layerContainer;
            this.Shown += MarsMC_Shown;
            numericUpDown.Maximum = long.MaxValue;
        }

        void MarsMC_Shown(object sender, EventArgs e)
        {
            _availableModels = _core.GetAllModels();
            foreach (var modelDescription in _availableModels)
            {
                listBoxModels.Items.Add(modelDescription.Name);
            }
        }

        private void listBoxModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedModel = _availableModels.ElementAt(listBoxModels.SelectedIndex);
            lblDescription.Text = string.Format("{0}\n\r{1}", _selectedModel.Name, _selectedModel.Description);
            btnRun.Enabled = _selectedModel != null;
            btnRunVis2d.Enabled = _selectedModel != null;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.Hide();
            _core.StartSimulationWithModel(_selectedModel, (int)numericUpDown.Value);
            this.Close();
        }

        private void btnRunVis2d_Click(object sender, EventArgs e)
        {
            this.Hide();
            new MarsVis2D(_core, _layerContainer, _selectedModel, (int)numericUpDown.Value).ShowDialog();
            this.Close();
        }

        private void listBoxModels_DoubleClick(object sender, EventArgs e)
        {
            if (_selectedModel != null) btnRunVis2d.PerformClick();
        }
    }
}
