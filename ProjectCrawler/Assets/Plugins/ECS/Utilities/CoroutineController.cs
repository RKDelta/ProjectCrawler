namespace ECS.Utilities
{
    using UnityEngine;

    public class CoroutineController : MonoBehaviour
    {
        public static CoroutineController Instance { get; protected set; }

        public void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("There can only be one CoroutineController in the Scene at a time");

                Object.Destroy(this);
                return;
            }
        }
    }
}
