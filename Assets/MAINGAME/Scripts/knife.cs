using UnityEngine;

public class knife : MonoBehaviour
{
    public Material netAnchor_Det;
    private int durability = 4; // Knife durability, can be adjusted as needed
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NetAnchors"))
        {
            Debug.Log("NetTrap Deployed");
            other.GetComponent<MeshRenderer>().material = netAnchor_Det;
            durability--;
            if (durability <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
