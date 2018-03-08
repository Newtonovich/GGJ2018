using UnityEngine;

public class Connector : MonoBehaviour
{
    [SerializeField] private GameObject jointSpriteObject;
    [SerializeField] private GameObject straightSpriteObject;
    [SerializeField] private GameObject diagonalSpriteObject;

    private InfluenceRadius influenece;

    private Vector3 jointPoint;
    private Vector3 orgPoint;
    private Vector3 currentPoint;
    private SpriteRenderer straightSprite;
    private SpriteRenderer diagonalSprite;

    private GameObject ssgo;
    private GameObject dsgo;

    void OnEnable()
    {
        influenece = GetComponent<InfluenceRadius>();
        orgPoint = Vector3.zero;

        ssgo = Instantiate(straightSpriteObject, orgPoint, Quaternion.identity, GameManager.Instance.ConnectorsParent);
        straightSprite = ssgo.GetComponent<SpriteRenderer>();

        dsgo = Instantiate(diagonalSpriteObject, orgPoint, Quaternion.identity, GameManager.Instance.ConnectorsParent);
        dsgo.transform.Rotate(new Vector3(0, 0, 45));
        diagonalSprite = dsgo.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(influenece != null && influenece.Dragging)
        {
            jointPoint = CalculateJointPosition();
            
            straightSprite.transform.position = (jointPoint + this.transform.position) / 2;
            Vector3 widthVector = this.transform.position - jointPoint;
            float straightWidth = widthVector.x == 0 ? 1 : widthVector.x * 2 + Mathf.Sign(widthVector.x);
            float straightHeight = widthVector.y == 0 ? 1 : widthVector.y * 2 + Mathf.Sign(widthVector.y);
            straightSprite.size = new Vector2(straightWidth , straightHeight);

            // Connect the Diagonal
            diagonalSprite.transform.position = (jointPoint + orgPoint) / 2;
            float tmpValue = 2 * jointPoint.x;
            float diagLength = Mathf.Sqrt(2* (tmpValue * tmpValue)) + 1;

            bool useWidth = Mathf.Sign(jointPoint.x) * Mathf.Sign(jointPoint.y) > 0;
            float diagWidth = useWidth ? diagLength : 1;
            float diagHeight = useWidth ? 1 : diagLength;
            diagonalSprite.size = new Vector2(diagWidth, diagHeight);
        }
    }

    private Vector3 CalculateJointPosition()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        float min = Mathf.Min(Mathf.Abs(x), Mathf.Abs(y));
        return new Vector3(min * Mathf.Sign(x), min * Mathf.Sign(y), 0);
    }

    void OnDestroy()
    {
        Destroy(ssgo);
        Destroy(dsgo);
    }
}
