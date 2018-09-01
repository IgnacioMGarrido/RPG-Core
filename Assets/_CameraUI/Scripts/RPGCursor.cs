using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using RPG.Characters;
namespace RPG.CameraUI
{
    public class RPGCursor : MonoBehaviour
    {
        const int WALKABLE_LAYER = 9;

        float maxRaycastDepth = 100f; // Hard coded value

        public delegate void OnMouseOverPotentiallyWalkable(Vector3 destination);
        public event OnMouseOverPotentiallyWalkable onMouseOverPotentiallyWalkable;

        public delegate void OnMouseOverEnemy(Enemy enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;

        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D targetCursor = null;
        [SerializeField] Texture2D buttonCursor = null;

        Vector2 cursorHotspot = new Vector2(0, 0);


        Rect currentScreenRect;
        void Update()
        {
            currentScreenRect = new Rect(0, 0, Screen.width, Screen.height);
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Stop looking for other objects
            }
            else
            {
                PerformRaycast();
            }
        }
        void PerformRaycast() {
            if (currentScreenRect.Contains(Input.mousePosition))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Priority Order
                if (RaycastForEnemy(ray)) { return; }
                if (RaycastForWalkable(ray)) { return; }
            }

        }
        private bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxRaycastDepth))
            {

                GameObject gameObjectHit = hitInfo.collider.gameObject;
                Enemy enemyHit = gameObjectHit.GetComponent<Enemy>();
                if (enemyHit)
                {
                    Cursor.SetCursor(targetCursor, cursorHotspot, CursorMode.Auto);
                    onMouseOverEnemy(enemyHit);
                    return true;
                }
            }
            return false;
        }

        private bool RaycastForWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask potentiallyWalkableLayer = 1 << WALKABLE_LAYER;
            bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer);
            if (potentiallyWalkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverPotentiallyWalkable(hitInfo.point);
                return true;
            }
            return false;
        }

        private void CursorOverUI() {

        }
    }
}