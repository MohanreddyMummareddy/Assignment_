using UnityEngine;
using UnityEngine.UIElements;

public class LineDrawingAndCollision : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private void Update()
    {
        if(Task_2_Manager.Instance.currentState != Task_2_Manager.state.Done) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                lineRenderer.positionCount = 0;
                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, GetMouseWorldPosition());
            }
            else if (Input.GetMouseButton(0))
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, GetMouseWorldPosition());
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector3[] linePositions = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(linePositions);

                Task_2_Manager.Instance.isLineDrawingCompleted?.Invoke(linePositions);

                lineRenderer.positionCount = 0;
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
