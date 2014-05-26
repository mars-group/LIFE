using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapActionsStock {

  internal class GoapActionBuyIce : GoapAction {

    public GoapActionBuyIce(Goap goap)
      : base(goap) {
      Preconditions.Add(new GoapWorldStateSunIsShining(true));
      Preconditions.Add(new GoapWorldStateGotIce(false));

      Postconditions.Add(new GoapWorldStateGotIce(true));
    }

      public override string ToString() {
          return "GoapActionBuyIce" ;
      }
  }
}