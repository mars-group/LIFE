using LIFE.API.Layer.PotentialField;

namespace LIFE.Components.GridPotentialFieldLayer
{
    public abstract class GridPotentialFieldLayer : AbstractPotentialFieldLayer<PotentialField>,
        IGridPotentialFieldLayer
    {
        public new int ExploreClosestWithEndlessSight(int cell)
        {
            return base.ExploreClosestWithEndlessSight(cell);
        }

        public new int ExploreClosestFullPotentialField(int cell)
        {
            return base.ExploreClosestFullPotentialField(cell);
        }

        public new bool HasFullPotential(int cell)
        {
            return base.HasFullPotential(cell);
        }

        protected override IFieldLoader<PotentialField> GetPotentialFieldLoader()
        {
            return new GridFieldLoader();
        }
    }
}