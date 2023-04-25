using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    #region Variables
    // Variables.
    [Header("Waypoint Variables"), Space(5)]
    [SerializeField] public List<WaypointData> waypoints;
    [SerializeField, Tooltip("Determins weather the platform will go backward and forward through the points or if it will cycle back to point one")] private bool cyclic;


    [Space(10), Header("Platform Variables"), Space(5)]
    [SerializeField, Min(1)] private int platformAmount = 1;
    [SerializeField] private bool canPush;
    [SerializeField] private float duration = 1;
    [SerializeField] private AnimationCurve easeCurve;
    private List<PlatformData> platforms = new List<PlatformData>();
    private Dictionary<GameObject, PlatformData> platformDict = new Dictionary<GameObject, PlatformData>();
    

    [Space(10), Header("Passenger Variables"), Space(5)]
    [SerializeField] private LayerMask passengerMask;
    private List<PassengerMovement> passengerMovement;
    private Dictionary<Transform, CharacterController> passengerDict = new Dictionary<Transform, CharacterController>();


    private float nextMoveTime;
    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (waypoints.Count == 0)
        {
            // This is awkward.
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < platformAmount; i++)
        {
            var instance = AddNewPlatform();
            instance.name = $"Platform {i}";

            instance.transform.position = waypoints[i].obj.transform.position;
            platforms.Add(new PlatformData(instance, i));

            platformDict.Add(platforms[i].obj, platforms[i]);

            var corotine = CalculateCurvedPlatformMovement(platforms[i]);
            StartCoroutine(corotine);
        }
    }

    private void Update()
    {
        //CalculatePassengerMovement()
    }

    private void OnDrawGizmos()
    {
        // Check if the list is null or only has one item
        if (waypoints.Count == 1) Gizmos.DrawIcon(waypoints[0].obj.transform.position, "Light Gizmo.tiff");

        if (waypoints.Count <= 1) return;

        var prevPoint = waypoints[0];

        foreach (var point in waypoints)
        {
            Gizmos.DrawIcon(point.obj.transform.position, "Light Gizmo.tiff");
            Gizmos.DrawLine(prevPoint.obj.transform.position, point.obj.transform.position);
            prevPoint = point;
        }

        if (cyclic) Gizmos.DrawLine(waypoints[0].obj.transform.position, waypoints[waypoints.Count - 1].obj.transform.position);
    }

    #endregion

    #region Private Methods
    // Private Methods.
    private float Ease(float time)
    {
        return easeCurve.Evaluate(time);
    }


    IEnumerator CalculateCurvedPlatformMovement(PlatformData platform)
    {
        float timePassed = 0;

        while (true)
        {
            // Wair for the wait time to start moving.
            if (Time.time < nextMoveTime)
            {
                yield return null;
                continue;
            }

            // Calculate the waypoint we are coming from.
            platform.fromWaypointIndex %= waypoints.Count;

            // Calculate the waypoint we are going to.
            int toWaypointIndex = (platform.fromWaypointIndex + 1) % waypoints.Count;

            nextMoveTime = Time.time + waypoints[platform.fromWaypointIndex].waitTime;

            while (timePassed <= duration)
            {
                timePassed += Time.deltaTime;
                float percent = Mathf.Clamp01(timePassed / duration);
                float curvePercent = Ease(percent);


                platform.velocity = Vector3.LerpUnclamped(waypoints[platform.fromWaypointIndex].obj.transform.position, waypoints[toWaypointIndex].obj.transform.position, curvePercent) - platform.obj.transform.position;

                //platform.obj.transform.position = Vector3.LerpUnclamped(waypoints[platform.fromWaypointIndex].obj.transform.position, waypoints[toWaypointIndex].obj.transform.position, curvePercent);

                platform.obj.transform.Translate(platform.velocity);

                if (percent >= 1) //|| curPos == waypoints[toWaypointIndex].transform.position)
                {
                    platform.fromWaypointIndex++;

                    if (!cyclic)
                    {
                        if (platform.fromWaypointIndex + 1 >= waypoints.Count)
                        {
                            platform.fromWaypointIndex = 0;
                            waypoints.Reverse();
                        }
                    }

                    timePassed = 0;
                    break;
                }

                yield return null;
            }

            yield return null;
        }
    }

    private void MovePassengers(bool moveBeforePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!passengerDict.ContainsKey(passenger.transform))
            {
                passengerDict.Add(passenger.transform, passenger.transform.GetComponent<CharacterController>());
            }
            if (passenger.moveBeforePlatform == moveBeforePlatform)
            {
                passengerDict[passenger.transform].Move(passenger.velocity);
            }
        }
    }

    private void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        //float directionHorizontal = Mathf.Sign(velocity.x)
        float directionVertical = Mathf.Sign(velocity.y);

        //if(velocity.y != 0)
        //{
        //    float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        //    for (int i = 0; i < verticalRayCount; i++)
        //    {
        //        Vector2 rayOrigin = (directionVertical == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
        //        rayOrigin += Vector2.right * (verticalRaySpacing * i);
        //        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionVertical, rayLength, passengerMask);

        //        if (hit && hit.distance != 0)
        //        {
        //            if (!movedPassengers.Contains(hit.transform))
        //            {
        //                movedPassengers.Add(hit.transform);

        //                float pushX = (directionVertical == 1) ? velocity.x : 0;
        //                float pushY = velocity.y - (hit.distance - skinWidth) * directionVertical;

        //                passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionVertical == 1, true));
        //            }
        //        }
        //    }
        //}
    }

    public void AddNewPassengerMovement(GameObject gameObject, Transform transform, bool moveBeforePlatform) 
    {
        var platform = platformDict[gameObject];
        var velocity = platformDict[gameObject].velocity;

        if (velocity.y != 0)
        {
            // do a check if it was hit from the bottom. maybe a callback from the charactercolliderhit.

            // This needs to check z as well
            float directionHorizontal = Mathf.Sign(velocity.x);
            float directionVertical = Mathf.Sign(velocity.y);

            float pushX = (directionVertical == 1) ? velocity.x : 0;
            float pushY = velocity.y * directionVertical;

            //passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionVertical == 1, true));
        }
    }

    public void RemovePassengerMovement()
    {

    }

    private void CalculatePlatformBounds(BoxCollider collider)
    {

    }


    public GameObject AddNewPlatform()
    {
        // TODO: This may be better to simply have a counter, then a list thats the length of that counter for the models on the platform
        var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.GetComponent<BoxCollider>().isTrigger = true;
        instance.AddComponent<MeshCollider>();
        instance.transform.parent = transform;
        return instance;
        //RegenerateNames();
        //CleanNullFromList(platforms);
    }
    #endregion

    #region Structs
    // Struct for keeping track of all the elements of a passenger
    private struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform transform, Vector3 velocity, bool standingOnPlatform, bool moveBeforePlatform)
        {
            this.transform = transform;
            this.velocity = velocity;
            this.standingOnPlatform = standingOnPlatform;
            this.moveBeforePlatform = moveBeforePlatform;
        }
    }

    [System.Serializable]
    private class PlatformData
    {
        public GameObject obj;
        public int fromWaypointIndex;
        public Vector3 velocity;
        public List<PassengerMovement> passengerMovement;
        public PlatformData(GameObject obj,  int fromWaypointIndex)
        {
            this.obj = obj;
            this.fromWaypointIndex = fromWaypointIndex;
        }
    }
    #endregion Structs
}

[System.Serializable]
public struct WaypointData
{
    public GameObject obj;
    public float waitTime;

    public WaypointData(GameObject obj)
    {
        this.obj = obj;
        this.waitTime = 0;
    }
}
