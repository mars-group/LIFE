using UnityEngine;
using Random = UnityEngine.Random;

namespace ApplicationCore
{
    public sealed class SimulationObject : MonoBehaviour
    {
        private float walkingTimeTarget { get; set; }

        public Vector3 Target;

        private float xMax;

        private float zMax;

        public void Start()
        {
            DistributionManager.RegisterSimulationObject(this);
            xMax = 100;
            zMax = 100;

            Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        }

        public void Update()
        {
            if (Time.time < walkingTimeTarget)
            {
                transform.Translate(Target*Time.deltaTime*0.01f);
            }
            else
            {
                Vector3 tmp = new Vector3(Random.Range(-xMax, xMax), 1, Random.Range(-zMax, zMax));
                Target = tmp - transform.position;
                walkingTimeTarget = Time.time + Random.Range(5.0f, 10.0f);
            }

            if (transform.position.x < DistributionManager.bounds.UpperLeft.x
            || transform.position.x > DistributionManager.bounds.LowerRight.x
            || transform.position.z < DistributionManager.bounds.LowerRight.z
            || transform.position.z > DistributionManager.bounds.UpperLeft.z)
            {
                DistributionManager.ReportOutOfBounds(this);
            }
        }

    }
}
