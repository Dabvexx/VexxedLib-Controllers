using UnityEngine;
using UnityEngine.Events;

public class PlatformTriggerEvent : MonoBehaviour
{
    #region Variables
    // Variables.
    #endregion

    #region Unity Methods

    private void OnCollisionEnter(Collision collision)
    {
        // If the dot product of the angle the collision hit is greater than .5
        // we are on the side and should remove the Y component

        // return the collider and a bool saying if it was from the side or if its from the top or bottom
        // passengerdata(collider.transform, ?, directionVertical == 1, true;
        // Above needs to be sent to something in the platform class to mke this
        // if its from the bottom, ignore collision(?)
    }

    private void OnCollisionExit(Collision collision)
    {
        // remove from the hashset if in hashset
    }

    #endregion
}
