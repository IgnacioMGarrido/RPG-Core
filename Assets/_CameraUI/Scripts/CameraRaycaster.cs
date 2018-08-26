using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using System;

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour
    {
        // INSPECTOR PROPERTIES RENDERED BY CUSTOM EDITOR SCRIPT
        const int WALKABLE_LAYER = 9;
        [SerializeField] int[] layerPriorities;

        float maxRaycastDepth = 100f; // Hard coded value
        int topPriorityLayerLastFrame = -1; // So get ? from start with Default layer terrain
        //two new delegates
        //OnMouseOverEnemy(Enemy enemy)
        public delegate void OnMouseOverPotentiallyWalkable(Vector3 destination);
        public event OnMouseOverPotentiallyWalkable onMouseOverPotentiallyWalkable;

        // Setup delegates for broadcasting layer changes to other classes
        public delegate void OnCursorLayerChange(int newLayer); // declare new delegate type
        public event OnCursorLayerChange notifyLayerChangeObservers; // instantiate an observer set

        public delegate void OnClickPriorityLayer(RaycastHit raycastHit, int layerHit); // declare new delegate type
        public event OnClickPriorityLayer notifyLeftMouseClickObservers; // instantiate an observer set
        public event OnClickPriorityLayer notifyRightMouseClickObservers; // instantiate an observer set


        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D unknownCursor = null;
        [SerializeField] Texture2D targetCursor = null;
        [SerializeField] Texture2D buttonCursor = null;

        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        void Update()
        {
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                NotifyObserversIfLayerChanged(5);
                return; // Stop looking for other objects
            }
            else
            {
                //Specify layer priorities here
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxRaycastDepth);
                if (RaycastForEnemy(ray)) { return; }
                if (RaycastForWalkable(ray)) { return; }
                FarTooComplex();//TODO: remove
            }
        }
        private bool RaycastForEnemy(Ray ray)
        {
            
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


        private void FarTooComplex()
        {
            // Raycast to max depth, every frame as things can move under mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxRaycastDepth);

            RaycastHit? priorityHit = FindTopPriorityHit(raycastHits);
            if (!priorityHit.HasValue) // if hit no priority object
            {
                NotifyObserversIfLayerChanged(0); // broadcast default layer
                return;
            }

            // Notify delegates of layer change
            var layerHit = priorityHit.Value.collider.gameObject.layer;
            NotifyObserversIfLayerChanged(layerHit);

            // Notify delegates of highest priority game object under mouse when clicked
            if (Input.GetMouseButton(0))
            {
                notifyLeftMouseClickObservers(priorityHit.Value, layerHit);
            }

            if (Input.GetMouseButtonDown(1))
            {
                notifyRightMouseClickObservers(priorityHit.Value, layerHit);
            }
        }

        void NotifyObserversIfLayerChanged(int newLayer)
        {
            if (newLayer != topPriorityLayerLastFrame)
            {
                topPriorityLayerLastFrame = newLayer;
                notifyLayerChangeObservers(newLayer);
            }
        }

        RaycastHit? FindTopPriorityHit(RaycastHit[] raycastHits)
        {
            // Form list of layer numbers hit
            List<int> layersOfHitColliders = new List<int>();
            foreach (RaycastHit hit in raycastHits)
            {
                layersOfHitColliders.Add(hit.collider.gameObject.layer);
            }

            // Step through layers in order of priority looking for a gameobject with that layer
            foreach (int layer in layerPriorities)
            {
                foreach (RaycastHit hit in raycastHits)
                {
                    if (hit.collider.gameObject.layer == layer)
                    {
                        return hit; // stop looking
                    }
                }
            }
            return null; // because cannot use GameObject? nullable
        }
    }
}