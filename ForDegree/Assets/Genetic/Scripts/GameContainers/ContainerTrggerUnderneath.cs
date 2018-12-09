using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerTrggerUnderneath : MonoBehaviour
{

    public NodeMono ontheFloor;

    //Detect collisions between the GameObjects with Colliders attached
    private void OnTriggerStay(Collider other)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (other.gameObject.name.StartsWith("Floor"))
        {
            ontheFloor = other.gameObject.GetComponent<NodeMono>();

        }

    }

}
