using UnityEngine;

public class knife : MonoBehaviour
{
    public Material netAnchor_Det;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NetAnchors"))
        {
            Debug.Log("NetTrap Deployed");
            other.GetComponent<MeshRenderer>().material = netAnchor_Det;
        }
    }
}
