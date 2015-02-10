namespace LIFEViewProtocol.Helper {
	public class Definitions {
		/// <summary>
		///     Known types of agents.
		/// </summary>
		public enum AgentTypes {
			BasicAgent,
			MovingBasicAgent,
			NonMovingBasicAgent,
			AnimalAgent,
			HumanAgent,
			CheetahAgent,
			ImpalaAgent,
			VegetationAgent,
			TreeAgent,
			LionAgent,
			ElephantAgent,
			FourLeggedAgent
		}

		/// <summary>
		///     Known primitive objects that may be used as markers or similar.
		/// </summary>
		public enum Primitives {
			Square,
			Cricle,
			Ring,
			Triangle,
			Sphere,
			Box
		}

		public enum EnvironmentEvents {
			Fire,
			Storm,
			Lightning,
			Flood,
			LightRain,
			Rain,
			HeavyRain,
			LightSnow,
			Snow,
			HeavySnow
		}

		/// <summary>
		///     Known passive objects. There is no interaction with passive objects but they are still relevant for the simulation
		///     (eg. buildings)
		/// </summary>
		public enum PassiveTypes {
			BasicPassiveObject,
			MovingPassiveObject,
			NonMovingPassiveObject,
			VegetationPassiveObject,
			WaterPointPassiveObject,
			PolygonPassiveObject,
			BuildingPassiveObject,
			WallPassiveObject
		}

		/// <summary>
		///     Available server commands
		/// </summary>
		public enum ServerActions {
			StepBack,
			StepForward,
			FirstTick,
			LatestTick,
			PauseTick,
			TransactionalTrue,
			TransactionalFalse
		}

		/// <summary>
		///     Available client commands
		/// </summary>
		public enum VisualizationActions {
			TransactionalTrue,
			TransactionalFalse
		}

		/// <summary>
		///     Predefined color names for easier access to the array.
		/// </summary>
		public enum AgentColors {
			Black,
			White,
			Yellow,
			Green,
			Red,
			Blue,
			Pink,
			Purple,
			Brown,
			Orange,
			Grey
		}


		/// <summary>
		///     Values for the predefined colors.
		/// </summary>
		public static readonly int[][] ColorValues = {
			new int[3] {0, 0, 0}, // black
			new int[3] {255, 255, 255}, // white
			new int[3] {225, 228, 15}, // yellow
			new int[3] {88, 171, 45}, // green
			new int[3] {204, 0, 0}, // red
			new int[3] {3, 86, 155}, // blue
			new int[3] {204, 0, 255}, // pink
			new int[3] {77, 0, 77}, // purple
			new int[3] {77, 0, 0}, // brown
			new int[3] {255, 77, 0}, // orange
			new int[3] {128, 128, 128} // grey
		};
	}
}