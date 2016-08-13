
namespace LifeAPI
{
	/// <summary>
	/// Marks a layer as disposable.
	/// Results in the SimulationManager to trigger
	/// a clean up on the layer, after simulation
	/// has finished. Implementation is optional.
	/// </summary>
	public interface IDisposableLayer
	{
		/// <summary>
		/// Disposes the layer after all ticks were executed. Use this to clean up, 
		/// or await the completion of longer running tasks (e.g. 
		/// output of results)
		/// </summary>
		void DisposeLayer ();
	}
}

