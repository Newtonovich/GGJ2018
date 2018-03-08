using UnityEngine;

public class Legend : MonoBehaviour
{
    private Animator animator;
    private bool isClosed;

    void Awake()
    {
        animator = GetComponent<Animator>();
        isClosed = true;
    }

    public void Transition()
    {
        animator.SetTrigger(isClosed ? "Open" : "Close");
        isClosed = !isClosed;
    }
}
