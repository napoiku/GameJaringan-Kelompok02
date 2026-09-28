using UnityEngine;

public class FixUIFlip : MonoBehaviour
{
    void LateUpdate()
    {
        float parentScaleX = transform.parent.localScale.x;

        Vector3 currentScale = transform.localScale;
        
        currentScale.x = Mathf.Abs(currentScale.x) * Mathf.Sign(parentScaleX);
        
        transform.localScale = currentScale;
    }
}