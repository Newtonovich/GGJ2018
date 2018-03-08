using UnityEngine;

public class SubStationCreator : MonoBehaviour
{
    [SerializeField] private GameObject influenceRadiusObject;

    private InfluenceRadius influenceRadius;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.AvailableStation > 0)
        {
            GameObject irgo = Instantiate(influenceRadiusObject, Vector3Int.zero, Quaternion.identity, GameManager.Instance.ConnectorsParent);
            influenceRadius = irgo.GetComponent<InfluenceRadius>();
        }
    }

    void OnMouseUp()
    {
        if(GameManager.Instance.AvailableStation > 0 && influenceRadius != null)
        {
            influenceRadius.MouseUp(GameManager.Instance.InfluencesCounter);
            audioSource.Play();
        }
    }
}
