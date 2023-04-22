using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileTypeSelect : MonoBehaviour
{
    public PathFindingSimulator pathFindingSimulator;
    public GameObject goTileTypeSelect;
    public GameObject goBackground;
    public TextMeshProUGUI txtPaintType;
    public RawImage imgPaintType;
    public Texture texturePaintType_Start;
    public Texture texturePaintType_Goal;
    public Texture texturePaintType_Ground;
    public Texture texturePaintType_Wall;

    public void OnPointerEnter(BaseEventData baseEventData)
    {
        var pointerEventData = baseEventData as PointerEventData;

        var tileType = (GroundTile.EType)pointerEventData.pointerEnter.GetComponent<TileTypeSelectNode>()?.type;
        pathFindingSimulator.paintType = tileType;
        txtPaintType.text = tileType.ToString();
        imgPaintType.texture = GetPaintTypeTexture(tileType);
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
        imgPaintType.texture = GetPaintTypeTexture(pathFindingSimulator.paintType);

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

    private Texture GetPaintTypeTexture(GroundTile.EType type)
    {
        Texture resultTexture;

        switch(type)
        {
            case GroundTile.EType.Start:
                resultTexture = texturePaintType_Start;
                break;
            case GroundTile.EType.Goal:
                resultTexture = texturePaintType_Goal;
                break;
            case GroundTile.EType.Ground:
                resultTexture = texturePaintType_Ground;
                break;
            case GroundTile.EType.Wall:
                resultTexture = texturePaintType_Wall;
                break;
            default:
                resultTexture = null;
                break;
        }

        return resultTexture;
    }
}
