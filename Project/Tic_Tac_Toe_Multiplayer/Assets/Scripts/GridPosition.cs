using UnityEngine;
// using UnityEngine.EventSystems;

public class GridPosition : MonoBehaviour/*, IPointerDownHandler*/
{
    [SerializeField]
    private int x;
    [SerializeField]
    private int y;


    // 检测鼠标点击

    // 使用需要 在Hierarchy中添加EventSystem，并在Camrea中添加PhysicsRaycaster/PhysicsRaycaster2D组件
    // public void OnPointerDown(PointerEventData eventData) {
    //     Debug.Log("OnPointerDown");
    // }

    // 配合BoxCollision使用
    private void OnMouseDown() {
        Debug.Log("Click!" + x + "," + y);
        GameManager.Instance.ClickedOnGridPositionRpc(x, y, GameManager.Instance.GetLocalPlayerType());
    }
}
