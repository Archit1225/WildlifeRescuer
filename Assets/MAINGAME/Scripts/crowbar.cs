using UnityEngine;

public class crowbar : MonoBehaviour
{
    public GameObject grabObject;

    public void EnableLever()
    {
        grabObject.SetActive(true);
    }
    public void DisableLever()
    {
        grabObject.SetActive(false);
    }
}
