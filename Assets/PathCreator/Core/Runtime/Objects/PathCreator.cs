using System.Collections.Generic;
using UnityEngine;

namespace PathCreation
{
    public class PathCreator : MonoBehaviour
    {

        /// This class stores data for the path editor, and provides accessors to get the current vertex and bezier path.
        /// Attach to a GameObject to create a new path editor.

        public event System.Action pathUpdated;

        [SerializeField, HideInInspector]
        PathCreatorData editorData;
        [SerializeField, HideInInspector]
        bool initialized;

        GlobalDisplaySettings globalEditorDisplaySettings;

        // Vertex path created from the current bezier path
        public VertexPath path
        {
            get
            {
                if (!initialized)
                {
                    InitializeEditorData(false);
                }
                return editorData.GetVertexPath(transform);
            }
        }

        // The bezier path created in the editor
        public BezierPath bezierPath
        {
            get
            {
                if (!initialized)
                {
                    InitializeEditorData(false);
                }
                return editorData.bezierPath;
            }
            set
            {
                if (!initialized)
                {
                    InitializeEditorData(false);
                }
                editorData.bezierPath = value;
            }
        }

        #region Internal methods

        /// Used by the path editor to initialise some data
        public void InitializeEditorData(bool in2DMode)
        {
            if (editorData == null)
            {
                editorData = new PathCreatorData();
            }
            editorData.bezierOrVertexPathModified -= TriggerPathUpdate;
            editorData.bezierOrVertexPathModified += TriggerPathUpdate;

            editorData.Initialize(in2DMode);
            initialized = true;
        }
        private int currentLayer = 0;
        public float GetCustomPointDistanceAtPercent(float t, int direction = 1)
        {


            if (editorData.customPoints == null || editorData.customPoints.Count == 0)
            {
                return 0;
            }

            if (t >= editorData.customPoints[editorData.customPoints.Count - 1].value)
            {
                return editorData.customPoints[editorData.customPoints.Count - 1].distance;
            }
            else if (t <= editorData.customPoints[0].value)
            {
                return editorData.customPoints[0].distance;
            }

           
            for (int i = 0; i < editorData.customPoints.Count - 1; i++)
            {
                if (editorData.customPoints[i].value <= t && t <= editorData.customPoints[i + 1].value)
                {
                    float p = Mathf.InverseLerp(editorData.customPoints[i].value, editorData.customPoints[i + 1].value, t);
                    if (editorData.customPoints[i].layerChanges != Vector2.zero && direction == 1)
                    {
                        currentLayer = editorData.customPoints[i].layerChanges.y;



                    }
                    else if (editorData.customPoints[i + 1].layerChanges != Vector2.zero && direction == -1) currentLayer = editorData.customPoints[i + 1].layerChanges.x;
                    return Mathf.Lerp(editorData.customPoints[i].distance, editorData.customPoints[i + 1].distance, p);
                }
            }

            return 0;



        }
        private float currentMinLayerDistanceCheck;
        private float currentMaxLayerDistanceCheck;
        private void SetCurrentLayer(int direction, float distance)
        {


        }

        public int ReturnLayer()
        {
            return currentLayer;
        }

        public int ReturnLayerForStart(float distance)
        {
            int layer = 0;
            for (int i = 0; i < editorData.customPoints.Count - 1; i++)
            {
                if (distance < editorData.customPoints[i].value && editorData.customPoints[i].layerChanges != Vector2.zero)
                {
                    layer = editorData.customPoints[i].layerChanges.x;
                    break;

                }

            }
            currentLayer = layer;
            return layer;

        }


        public PathCreatorData EditorData
        {
            get
            {
                return editorData;
            }

        }

        public void TriggerPathUpdate()
        {
            if (pathUpdated != null)
            {
                pathUpdated();
            }
        }

#if UNITY_EDITOR

        // Draw the path when path objected is not selected (if enabled in settings)
        void OnDrawGizmos()
        {

            // Only draw path gizmo if the path object is not selected
            // (editor script is resposible for drawing when selected)
            GameObject selectedObj = UnityEditor.Selection.activeGameObject;
            if (selectedObj != gameObject)
            {

                if (path != null)
                {
                    path.UpdateTransform(transform);

                    if (globalEditorDisplaySettings == null)
                    {
                        globalEditorDisplaySettings = GlobalDisplaySettings.Load();
                    }

                    if (globalEditorDisplaySettings.visibleWhenNotSelected)
                    {

                        Gizmos.color = globalEditorDisplaySettings.bezierPath;

                        for (int i = 0; i < path.NumPoints; i++)
                        {
                            int nextI = i + 1;
                            if (nextI >= path.NumPoints)
                            {
                                if (path.isClosedLoop)
                                {
                                    nextI %= path.NumPoints;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            Gizmos.DrawLine(path.GetPoint(i), path.GetPoint(nextI));
                        }
                    }
                }
            }
        }
#endif

        #endregion
    }
}