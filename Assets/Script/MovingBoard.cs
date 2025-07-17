using UnityEngine;

public class MovingBoard : MonoBehaviour
{
    [SerializeField] float movingSpeed;
    [SerializeField] float movingRange;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * movingSpeed, movingRange*2)-movingRange;
        Debug.Log(offset);
        transform.localPosition = startPos + new Vector3(offset, 0, 0);
    }
}
