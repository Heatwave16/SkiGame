using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform camPosition;

    private void Update()
    {
        transform.position = camPosition.position;
    }
}
