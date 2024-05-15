using UnityEngine;

namespace House.Windows
{
    public class ObjectInteraction : MonoBehaviour
    {

        public bool inUse;

        public void Interact()
        {
            inUse = !inUse;
            Debug.Log("Interaction");
        }

    }
}