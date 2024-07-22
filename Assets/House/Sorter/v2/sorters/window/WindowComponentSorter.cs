using System;
using System.Text.RegularExpressions;
using Lean.Touch;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace House.Sorter.v2.sorters.window
{
    public class WindowComponentSorter
    {
        public void SortWindowComponents(Transform floorWindowParent)
        {
            for (var i = 0; i < floorWindowParent.childCount; i++)
            {
                var windowParent = floorWindowParent.GetChild(i);
                if (CheckIfWindowIsMoveable(windowParent))
                {

                    var wing1Parent = new GameObject
                    {
                        name = "Wing 1",
                        transform =
                        {
                            position = GetWingPivot(windowParent, "Flügel_1")
                        }
                    };

                    var wing2Parent = new GameObject
                    {
                        name = "Wing 2",
                        transform =
                        {
                            position = GetWingPivot(windowParent, "Flügel_2")
                        }
                    };

                    var staticParent = new GameObject
                    {
                        name = "Static"
                    };

                    var pointer = 0;
                    while (pointer < windowParent.childCount)
                    {
                        var windowChild = windowParent.GetChild(pointer);
                        if (CheckGameObjectName(windowChild.name, "Flügel_1"))
                        {
                            windowChild.parent = wing1Parent.transform;
                        } 
                        else if (CheckGameObjectName(windowChild.name, "Flügel_2"))
                        {
                            windowChild.parent = wing2Parent.transform;
                        }
                        else
                        {
                            windowChild.parent = staticParent.transform;
                        }
                    }
                    
                    wing1Parent.transform.parent = windowParent;
                    wing2Parent.transform.parent = windowParent;
                    staticParent.transform.parent = windowParent;
                    
                    AddWindowFunctionality(windowParent.gameObject, wing1Parent, wing2Parent);
                }
            }
        }

        private bool CheckIfWindowIsMoveable(Transform window)
        {
            var moveableRegex = new Regex(@"\b\w*" + "Drehpunkt" + @"\w*\b");
            for (var i = 0; i < window.childCount; i++)
            {
                var child = window.GetChild(i);
                if (moveableRegex.Match(child.name).Success) return true;
            }

            return false;
        }

        private bool CheckGameObjectName(string name, string key)
        {
            return new Regex(@"\b\w*" + key + @"\w*\b").Match(name).Success;
        }

        private Vector3 GetWingPivot(Transform windowParent, string wingName)
        {
            for (var i = 0; i < windowParent.childCount; i++)
            {
                var child = windowParent.GetChild(i);
                if (CheckGameObjectName(child.name, "Drehpunkt") && CheckGameObjectName(child.name, wingName))
                {
                    child.gameObject.SetActive(false);
                    return GetObjectCenter(child);
                }
            }

            return Vector3.zero;
        }
        
        private Vector3 GetObjectCenter(Transform target)
        {
            return target.gameObject.GetComponent<Renderer>().bounds.center;
        }
        
        private void AddWindowFunctionality(GameObject windowParent, GameObject wing1, GameObject wing2)
        {
            FaceDirection windowFaceDirection = GetFaceDirection(windowParent.transform);
            
            var controller = windowParent.AddComponent<WindowController>();
            controller.LeftPane = wing1;
            controller.RightPane = wing2;
            controller.PaneOpenAngle = 90f;
            controller.RotationAxisValue = WindowController.RotationAxis.Y;
            controller.OpeningDirectionValue = WindowController.OpeningDirection.INWARDS;
            controller.OpeningTime = 3f;
            controller.ShowDebugUI = false;
            
            UnityAction<GameObject> action = controller.UseWindow;
            
            var wing1Selectable = wing1.AddComponent<LeanSelectableByFinger>();
            UnityEventTools.AddObjectPersistentListener(wing1Selectable.OnSelected, action, wing1);
            UnityEventTools.AddObjectPersistentListener(wing1Selectable.OnDeselected, action, wing1);
            
            var wing2Selectable = wing2.AddComponent<LeanSelectableByFinger>();
            UnityEventTools.AddObjectPersistentListener(wing2Selectable.OnSelected, action, wing2);
            UnityEventTools.AddObjectPersistentListener(wing2Selectable.OnDeselected, action, wing2);

            var colliderCenter = GetColliderCenterPoint(windowFaceDirection);
            var colliderSize = GetColliderSize(windowFaceDirection);
            
            var wing1Collider = wing1.AddComponent<BoxCollider>();
            wing1Collider.center = colliderCenter.wing1;
            wing1Collider.size = colliderSize.wing1;
            /*
            wing1Collider.center = new Vector3(0f, -0.36f, -0.18f);
            wing1Collider.size = new Vector3(0.07f, 1f, 0.34f);
            */
            
            var wing2Collider = wing2.AddComponent<BoxCollider>();
            wing2Collider.center = colliderCenter.wing2;
            wing2Collider.size = colliderSize.wing2;
            /*
            wing2Collider.center = new Vector3(0f, -0.36f, 0.18f);
            wing2Collider.size = new Vector3(0.07f, 1f, 0.34f);
            */
        }

        private FaceDirection GetFaceDirection(Transform windowParent)
        {
            Vector3 rotationPoint = default(Vector3);
            Vector3 directionPoint = default(Vector3);

            for (var i = 0; i < windowParent.GetChild(0).childCount; i++)
            {
                var child = windowParent.GetChild(0).GetChild(i);
                if (CheckGameObjectName(child.name, "Drehpunkt"))
                {
                    rotationPoint = GetObjectCenter(child);
                } 
                else if (CheckGameObjectName(child.name, "Richtung"))
                {
                    directionPoint = GetObjectCenter(child);
                }
            }
            
            if (rotationPoint == default || directionPoint == default)
            {
                throw new Exception();
            }
            
            if (rotationPoint.x < directionPoint.x)
            {
                return FaceDirection.X_NEGATIVE;
            }

            if (rotationPoint.x > directionPoint.x)
            {
                return FaceDirection.X_POSITIVE;
            }
            
            if (rotationPoint.z < directionPoint.z)
            {
                return FaceDirection.Z_NEGATIVE;
            }

            if (rotationPoint.z > directionPoint.z)
            {
                return FaceDirection.Z_POSITIVE;
            }

            throw new Exception();
        }

        private (Vector3 wing1, Vector3 wing2) GetColliderCenterPoint(FaceDirection faceDirection)
        {
            return faceDirection switch
            {
                FaceDirection.X_POSITIVE => (
                    wing1: new Vector3(0f, -0.11f, -0.18f),
                    wing2: new Vector3(0f, -0.11f, 0.18f)
                ),
                FaceDirection.X_NEGATIVE => (
                    wing1: new Vector3(0f, -0.11f, -0.18f),
                    wing2: new Vector3(0f, -0.11f, 0.18f)
                ),
                FaceDirection.Z_POSITIVE => (
                    wing1: new Vector3(-0.18f, -0.11f, 0f),
                    wing2: new Vector3(0.18f, -0.11f, 0f)
                ),
                FaceDirection.Z_NEGATIVE => (
                    wing1: new Vector3(-0.18f, -0.11f, 0f),
                    wing2: new Vector3(0.18f, -0.11f, 0f)
                ),
                _ => (Vector3.zero, Vector3.zero)
            };
        }

        private (Vector3 wing1, Vector3 wing2) GetColliderSize(FaceDirection faceDirection)
        {
            return faceDirection switch
            {
                FaceDirection.X_POSITIVE => (
                    wing1: new Vector3(0.07f, 1f, 0.34f),
                    wing2: new Vector3(0.07f, 1f, 0.34f)
                ),
                FaceDirection.X_NEGATIVE => (
                    wing1: new Vector3(0.07f, 1f, 0.34f),
                    wing2: new Vector3(0.07f, 1f, 0.34f)
                ),
                FaceDirection.Z_POSITIVE => (
                    wing1: new Vector3(0.34f, 1f, 0.07f),
                    wing2: new Vector3(0.34f, 1f, 0.07f)
                ),
                FaceDirection.Z_NEGATIVE => (
                    wing1: new Vector3(0.34f, 1f, 0.07f),
                    wing2: new Vector3(0.34f, 1f, 0.07f)
                ),
                _ => (Vector3.zero, Vector3.zero)
            };
        }

        private enum FaceDirection
        {
            X_POSITIVE,
            X_NEGATIVE,
            Z_POSITIVE,
            Z_NEGATIVE,
        }
        
    }
}