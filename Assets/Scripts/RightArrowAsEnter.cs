using UnityEngine;
using UnityEngine.EventSystems;

public class RightArrowAsEnter : MonoBehaviour
{
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
        }
    }
}

