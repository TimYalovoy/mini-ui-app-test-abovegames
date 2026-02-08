using MainApp;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutGroupDeviceTypeDepender : MonoBehaviour, IDeviceTypeDepender
{
    private const float cellWidth = 640f;
    private const float cellHeight = 640f;

    private GridLayoutGroup _gridLayoutGroup;
    private EDeviceType _deviceType = EDeviceType.UnityEditor;

    private Vector2 defaultSize = new Vector2(cellWidth, cellHeight);

    private void Awake()
    {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    public void Set(EDeviceType deviceType)
    {
        _deviceType = deviceType;

        switch (deviceType)
        {
            case EDeviceType.Phone:
                _gridLayoutGroup.cellSize = defaultSize;
                _gridLayoutGroup.constraintCount = 3;
            break;
            
            case EDeviceType.UnityEditor:
            case EDeviceType.Tablet:
            default:
                _gridLayoutGroup.cellSize = defaultSize * 2f / 3f;
                _gridLayoutGroup.constraintCount = 3;
            break;
        }
    }
}
