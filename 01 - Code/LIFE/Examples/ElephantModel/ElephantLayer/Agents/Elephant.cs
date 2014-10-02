using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpQuadTree;
using LayerAPI.Interfaces;

namespace ElephantLayer.Agents
{
public class Elephant : IAgent, IQuadObject{
	private const double DEFAULT_ELEPHANT_SIZE = 1;
    private const double DEFAULT_ELEPHANT_DIRECTION = 0;
    private const double DEFAULT_VISION_DISTANCE = 20;
    private const double DEFAULT_STEP_SIZE = 




	private static int totalElephants=0;
	
	private int rank=1; //This will matter later
	private int elephantNumber;
	private bool isThirsty;
    private double radius;
    private double direction;
    private int visionDistance;
    private double stepLength;
    private int steps;

    public Elephant() {
		this(DEFAULT_ELEPHANT_SIZE, new Random().NextDouble()*Math.PI*2,);
	}

	public Elephant(double size, double direction, int visionDistance, double stepLength){
        this.radius=radius;
        this.direction = direction;
	    this.steps = 0;
	    this.visionDistance = visionDistance;
		this.stepLength=stepLength;
		isThirsty=true;
		elephantNumber=++totalElephants;
	}

	 
	public void Tick(){
		super.act();
		if(canMove() && getGrid().contains(Water.class, getLocation()))
			isThirsty=false;
	}
	 
	public String toString() {
		return "Elephant #"+elephantNumber+". Total steps = "+steps;
	}
	 
	protected double getRandomTurnDirection() {
		double angle=Math.random()*getMaxTurningRadius()*2-getMaxTurningRadius();
		return angle;
	}
	 
	protected double getResourceDirection(Resource actor) {
		Location loc=actor.getLocation();
		double angle=getTurnAngleTo(loc);
		angle=fixTurnAngle(angle);
		return angle;
	}
	 
	protected double getRepulsionDirection(Elephant actor) {
		Location loc=actor.getLocation();
		double angle=getTurnAngleTo(loc);
		/*
		if(angle>Math.PI/2 || angle < -Math.PI/2)
			return 0;
			*/
		angle=fixTurnAngle(angle+Math.PI);
		return angle;
	}
	 
	protected double getAttractionDirection(Elephant actor) {
		Location loc=actor.getLocation();
		double angle=getTurnAngleTo(loc);
		angle=fixTurnAngle(angle);
		return angle;
	}
	 
	protected double getAlignmentDirection(Elephant actor) {
		double angle=actor.getDirection()-getDirection();
		angle=fixTurnAngle(angle);
		return angle;
	}
	 
	protected double getRandomTurnFactorWeight() {
		return 1;
	}
	 
	protected double getResourceFactorWeight(Resource actor) {
		if(!isThirsty)
			return 0;
		double distance=getLocation().distanceTo(actor.getLocation());
		return 1/Math.sqrt(distance);
	}
	 
	protected double getRepulsionFactorWeight(Elephant actor) {
		double distance=getLocation().distanceTo(actor.getLocation());
		if(distance>idealSeperation)
			return 0;
		else
			return (-distance/idealSeperation)+1;
	}

	protected double getAttractionFactorWeight(Elephant actor) {
		double distance=getLocation().distanceTo(actor.getLocation());
		if(distance<idealSeperation)
			return 0;
		else
			return ((distance-idealSeperation)/idealSeperation);
			
	}
	 
	protected double getAlignmentFactorWeight(Elephant actor) {
		return 1;
//		return 1/3 + 1/rank;
	}

    public Rect Bounds
    {
		get { return _bounds; }
		set { _bounds = value; }
    }

    public event EventHandler BoundsChanged;
}

}
