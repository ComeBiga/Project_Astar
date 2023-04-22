using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileTypeSelect : MonoBehaviour
{
    public GameObject goTileTypeSelect;
    public GameObject goBackground;
    public TextMeshProUGUI txtPaintType;

    public PathFindingSimulator pathFindingSimulator;

    public void OnPointerEnter(BaseEventData baseEventData)
    {
        var pointerEventData = baseEventData as PointerEventData;

        var tileType = (GroundTile.EType)pointerEventData.pointerEnter.GetComponent<TileTypeSelectNode>()?.type;
        pathFindingSimulator.paintType = tileType;
        txtPaintType.text = tileType.ToString();
    }

    public void OnPointerExit(BaseEventData baseEventData)
    {
        var pointerEventData = baseEventData as PointerEventData;
    }

    // Start is called before the first frame update
    private void Start()
    {
        goTileTypeSelect.SetActive(false);
        goBackground.SetActive(false);
        txtPaintType.text = pathFindingSimulator.paintType.ToString();

        InputManager.Instance.onMouseMiddleButtonDown += (screenCenterPos, rayCastHit) =>
        {
            CameraController.Instance.LockCamera(true);
            CameraController.Instance.HideAimPoint(true);
            goTileTypeSelect.SetActive(true);
            goBackground.SetActive(true);
        };
        
        InputManager.Instance.onMouseMiddleButton += (screenCenterPos, rayCastHit) =>
        {

        };
        
        InputManager.Instance.onMouseMiddleButtonUp += (screenCenterPos, rayCastHit) =>
        {
            CameraController.Instance.LockCamera(false);
            CameraController.Instance.HideAimPoint(false);
            goTileTypeSelect.SetActive(false);
            goBackground.SetActive(false);
        };
    }

    private IEnumerator SelectTileType()
    {
        while(true)
        {
            yield return null;
        }
    }
}
