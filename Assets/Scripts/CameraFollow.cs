using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void Start()
    {      
        offset = transform.position - target.position;
       
    }
    private void FixedUpdate()
    {
        if(target)
        {
            Vector3 newPos = target.position + offset;
            transform.position = Vector3.Slerp(transform.position, newPos, .125f);
            transform.LookAt(target);
        }
       
    }
}
