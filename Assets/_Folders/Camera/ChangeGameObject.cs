using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGameObject : MonoBehaviour
{
    [SerializeField] private bool setObjectActive;
    [SerializeField] private GameEvent gameEvent;
    [SerializeField] private GameObject targetObject;
    public void TriggerAction()
   {
    if (gameEvent != null)
    {
            gameEvent.TriggerEvent();
        }
        // targetObject.SetActive(setObjectActive);

    }
}
