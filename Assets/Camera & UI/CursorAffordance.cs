using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAffordance : MonoBehaviour {

    CameraRaycaster cameraRaycaster;
    [SerializeField] Texture2D[] cursorTextures;
    Vector2 hotSpot = new Vector2(96, 96);

	// Use this for initialization
	void Start () {
        cameraRaycaster = GetComponent<CameraRaycaster>();
	}
	
	// Update is called once per frame
	void Update () {
        switch (cameraRaycaster.layerHit)
        {
            case Layer.Walkable:
                Cursor.SetCursor(cursorTextures[0], hotSpot, CursorMode.Auto);
                break;
            case Layer.Enemy:
                Cursor.SetCursor(cursorTextures[1], hotSpot, CursorMode.Auto);
                break;
            case Layer.RaycastEndStop:
                Cursor.SetCursor(cursorTextures[2], hotSpot, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(cursorTextures[2], hotSpot, CursorMode.Auto);
                Debug.LogError("Don't know what cursor to show");
                break;
        }
    }
}
