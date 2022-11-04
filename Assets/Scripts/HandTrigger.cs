using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrigger : MonoBehaviour
{
    public HandColor color;
    [HideInInspector]
    public GameObjectBase obj;

    public bool stick;
    // Start is called before the first frame update
    void Start()
    {
        obj = GetComponent<GameObjectBase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var hand = collision.GetComponent<HandDrag>().hand;
        if (CheckCanTrigger(hand) && Input.GetMouseButtonUp(0))
        {
            SetObjectStatus(ObjectStatus.ON);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var hand = collision.GetComponent<HandDrag>().hand;
        if (CheckCanTrigger(hand) && Input.GetMouseButtonUp(0))
        {
            SetObjectStatus(ObjectStatus.OFF);
        }
    }
    private void SetObjectStatus(ObjectStatus status)
    {
        obj.SetStatus(status);
    }

    public bool CheckCanTrigger(GrabHand hand)
    {
        return hand && (this.color == HandColor.ALL || hand.color == this.color);
    }
}
