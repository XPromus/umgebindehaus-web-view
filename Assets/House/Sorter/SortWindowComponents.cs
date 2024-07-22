using System;
using System.Collections.Generic;
using System.Linq;
using Lean.Touch;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace House.Sorter
{
    
    public class SortWindowComponents
    {
        private const string TextSeperator = "_";

        public void SortObjectsInWindow(GameObject windowParent)
        {
            var wing1Children = new List<Transform>();
            var wing2Children = new List<Transform>();
            var frameChildren = new List<Transform>();
            
            Transform wing1Pivot = null;
            Transform wing2Pivot = null;
            
            for (var i = 0; i < windowParent.transform.childCount; i++)
            {
                var child = windowParent.transform.GetChild(i);
                var windowPartValue = CalcElementPlacementInWindow(child);

                switch (windowPartValue)
                {
                    case WindowParentComponent.Wing1:
                        wing1Children.Add(child);
                        break;
                    case WindowParentComponent.Wing2:
                        wing2Children.Add(child);
                        break;
                    case WindowParentComponent.Wing1Center:
                        Debug.Log("Found Wing 1 Pivot");
                        wing1Pivot = child;
                        break;
                    case WindowParentComponent.Wing2Center:
                        Debug.Log("Found Wing 2 Pivot");
                        wing2Pivot = child;
                        break;
                    case WindowParentComponent.Frame:
                        frameChildren.Add(child);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
            
            Debug.Log("Creating Wing 1");
            var wing1Parent = new GameObject
            {
                name = "Wing 1",
                transform =
                {
                    parent = windowParent.transform,
                    position = GetObjectCenter(wing1Pivot!)
                }
            };
            
            Debug.Log("Creating Wing 2");
            var wing2Parent = new GameObject
            {
                name = "Wing 2",
                transform =
                {
                    parent = windowParent.transform,
                    position = GetObjectCenter(wing2Pivot!)
                }
            };
            
            Debug.Log("Creating Frame");
            var windowFrameParent = new GameObject
            {
                name = "Window Frame",
                transform =
                {
                    parent = windowParent.transform
                }
            };
            
            AddChildrenToParent(windowFrameParent, frameChildren);
            AddChildrenToParent(wing1Parent, wing1Children);
            AddChildrenToParent(wing2Parent, wing2Children);
            
            wing1Pivot!.gameObject.SetActive(false);
            wing2Pivot!.gameObject.SetActive(false);
            
            AddWindowFunctionality(windowParent, wing1Parent, wing2Parent);
        }

        private void AddChildrenToParent(GameObject parent, List<Transform> children)
        {
            foreach (var child in children)
            {
                child.GetComponent<MeshCollider>().enabled = false;
                child.parent = parent.transform;
            }
        }

        private void AddWindowFunctionality(GameObject windowParent, GameObject leftPivot, GameObject rightPivot)
        {
            var controller = windowParent.AddComponent<WindowController>();
            controller.LeftPane = leftPivot;
            controller.RightPane = rightPivot;
            controller.PaneOpenAngle = 90f;
            controller.RotationAxisValue = WindowController.RotationAxis.Y;
            controller.OpeningDirectionValue = WindowController.OpeningDirection.INWARDS;
            controller.OpeningTime = 3f;
            controller.ShowDebugUI = false;
            
            UnityAction<GameObject> action = controller.UseWindow;
            
            var leftSelectable = leftPivot.AddComponent<LeanSelectableByFinger>();
            UnityEventTools.AddObjectPersistentListener(leftSelectable.OnSelected, action, leftPivot);
            UnityEventTools.AddObjectPersistentListener(leftSelectable.OnDeselected, action, leftPivot);
            
            var rightSelectable = rightPivot.AddComponent<LeanSelectableByFinger>();
            UnityEventTools.AddObjectPersistentListener(rightSelectable.OnSelected, action, rightPivot);
            UnityEventTools.AddObjectPersistentListener(rightSelectable.OnDeselected, action, rightPivot);

            var leftCollider = leftPivot.AddComponent<BoxCollider>();
            leftCollider.center = new Vector3(0f, -0.36f, -0.18f);
            leftCollider.size = new Vector3(0.07f, 1f, 0.34f);
            
            var rightCollider = rightPivot.AddComponent<BoxCollider>();
            rightCollider.center = new Vector3(0f, -0.36f, 0.18f);
            rightCollider.size = new Vector3(0.07f, 1f, 0.34f);
        }
        
        private WindowParentComponent CalcElementPlacementInWindow(Transform element)
        {
            var objectName = element.gameObject.name;

            if (CheckTextForFrame(objectName))
            {
                Debug.Log("Found Frame: " + objectName);
                return WindowParentComponent.Frame;
            }
            
            if (CheckTextForWing(objectName))
            {
                Debug.Log("Found Wing: " + objectName);
                return CheckTextForWingDetails(objectName);
            }

            return WindowParentComponent.Frame;

            //Objekte mit Namen
            // Flügel 1 - geht zuerst auf
            // Flügel 2 - geht als zweites auf
            // Rahmen im Namen - Bewegt sich nicht 

            //Berechnung: y am längsten, x zweite länge, z dritte länge
            //Berechnung Öffnungsrichtung: Zentrum vom Haus. Fenster in die Richtung des Zentrums
        }

        private WindowParentComponent CheckTextForWingDetails(string text)
        {
            var textElement = text.Split(TextSeperator).Last().Split(" ")[0];
            return textElement switch
            {
                "1" => WindowParentComponent.Wing1,
                "2" => WindowParentComponent.Wing2,
                "Drehpunkt" => GetWingCenterComponent(text),
                _ => WindowParentComponent.Wing1
            };
        }

        private bool CheckTextForFrame(string text)
        {
            var textElement = text.Split(TextSeperator).Last().Split(" ")[0];
            if (textElement.Equals("Rahmen")) return true;
            return false;
        }
        
        private bool CheckTextForWing(string text)
        {
            var textElements = text.Split(" ").First().Split(TextSeperator);
            return textElements[^2].Equals("Flügel") || textElements[^1].Equals("Drehpunkt");
        }
        
        private WindowParentComponent GetWingCenterComponent(string text)
        {
            var textElement = text.Split(TextSeperator)[^2];
            return textElement switch
            {
                "1" => WindowParentComponent.Wing1Center,
                "2" => WindowParentComponent.Wing2Center,
                _ => WindowParentComponent.Wing1Center
            };
        }

        private Vector3 GetObjectCenter(Transform target)
        {
            return target.gameObject.GetComponent<Renderer>().bounds.center;
        }

        private enum WindowParentComponent
        {
            Wing1,
            Wing1Center,
            Wing2,
            Wing2Center,
            Frame
        }
        
        /*
        private Bounds GetMaxBounds(GameObject g)
        {
            var renderers = g.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(g.transform.position, Vector3.zero);
            var b = renderers[0].bounds;
            foreach (Renderer r in renderers)
            {
                b.Encapsulate(r.bounds);
            }

            return b;
        }
        */
        
    }
    
}