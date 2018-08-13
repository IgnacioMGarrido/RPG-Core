using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorAffordance : MonoBehaviour {

    CameraRaycaster cameraRaycaster;
    [SerializeField] Texture2D[] cursorTextures;
    [SerializeField] Vector2 hotSpot = new Vector2(0, 0);

	// Use this for initialization
	void Start () {
        cameraRaycaster = GetComponent<CameraRaycaster>();
        cameraRaycaster.onLayerChange += OnDelegateCalled; // register delegates
	}
	// Update is called once per frame
	void OnDelegateCalled(Layer newLayer) {
        switch (newLayer)
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

    //TODO: Consider de-registering OnLayerChanged on leaving game scenes.
}
