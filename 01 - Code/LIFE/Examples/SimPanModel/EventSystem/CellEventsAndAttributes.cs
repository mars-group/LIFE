using System;
using System.Collections.Generic;

namespace EventSystem {
    public class CellEventsAndAttributes
    {
        public delegate void CellTypeChangeHandler(int cellId, EventArgs e);
        public event CellTypeChangeHandler StatusChange;

        public Dictionary<CellType, System.Drawing.Color> CellColors = new Dictionary<CellType, Color> {
            {CellType.Neutral, Color.WhiteSmoke},
            {CellType.Obstacle, Color.DarkGray },
            {CellType.Panic, Color.Crimson},
            {CellType.Chaos, Color.OrangeRed},
            {CellType.Sacrifice, Color.Purple}
        };

        public enum CellType
        {
            Neutral,
            Obstacle,
            Panic,
            Chaos,
            Sacrifice
        }
    }

     public class CellIdAndStatus : EventArgs {
        public int CellId { get; set; }
        public CellLayerImpl.CellType CellType { get; set; }
    }

    
}