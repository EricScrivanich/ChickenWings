
using UnityEngine;

[System.Serializable]
    public class PlaceholderRingData
    {
        public int ringIndex;
        public int addOrder;
        public int order;
        public int getsTriggeredInt;
        public int doesTriggerInt;
        public float xCordinateTrigger;
        public float speed;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public PlaceholderRingData(PlaceholderRing placeholder, Transform transform)
        {
            ringIndex = placeholder.ringIndex;
            addOrder = placeholder.addOrder;
            order = placeholder.order;
            getsTriggeredInt = placeholder.getsTriggeredInt;
            doesTriggerInt = placeholder.doesTriggerInt;
            xCordinateTrigger = placeholder.xCordinateTrigger;
            speed = placeholder.speed;
            position = transform.position;
            rotation = transform.rotation;
            scale = transform.localScale;
        }
    }