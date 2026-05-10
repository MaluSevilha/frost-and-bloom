using UnityEngine;

public class WaterFloat : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * amplitude + Mathf.Sin(Time.time * speed * 0.5f) * (amplitude * 0.5f);
        transform.position = startPos + new Vector3(0, y, 0);
    }
}