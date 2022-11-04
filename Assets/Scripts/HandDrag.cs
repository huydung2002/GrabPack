using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDrag : MonoBehaviour
{
    public GrabHand hand;
    public bool dragging;

    public List<GrabableObject> grabingObjs = new List<GrabableObject>();
    public HandTrigger stickingTrigger;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!dragging && stickingTrigger)
        {
            this.transform.position = stickingTrigger.transform.position;
            this.transform.rotation = Quaternion.identity;
            this.hand.GetComponentInParent<BoneFollower>().enabled = false;
        }
        else
        {
            this.hand.GetComponentInParent<BoneFollower>().enabled = true;
        }
    }

    private void OnMouseDrag()
    {
        if (dragging)
        {
            FindObjectOfType<AudioManager>().Play("Drag");
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
            pos.z = 0;
            this.transform.position = pos;
        }
    }
    private void OnMouseDown()
    {
        FindObjectOfType<AudioManager>().Play("Drag");
        dragging = true;
        stickingTrigger = null;
        this.transform.parent = null;
    }

    private void OnMouseUp()
    {
        FindObjectOfType<AudioManager>().Play("Drag");
        dragging = false;
        this.transform.parent = hand.transform;
        if (GameManager.instance.ended) return;

        if(!hand.wrongLine) { 
            var colliders = Physics2D.OverlapCircleAll(this.transform.position, 1f);
            foreach(var collider in colliders)
            {
                var grabObj = collider.GetComponent<GrabableObject>();
                if (grabObj) {
                    //grabObj.transform.parent = this.transform;
                    //grabObj.transform.localPosition = Vector3.zero;
                    grabObj.SetGrabbed(this);
                    this.grabingObjs.Add(grabObj);
                }

                var handTrigger =  collider.GetComponent<HandTrigger>();
                if(handTrigger && handTrigger.CheckCanTrigger(this.hand) &&  handTrigger.stick)
                {
                    stickingTrigger = handTrigger;
                }
                if(collider.CompareTag("Goal")) { 
                    GameManager.instance.CollectGoal(collider);
                }

                var anchoerObj = collider.GetComponent<AnchorObject>();
                if (anchoerObj)
                {
                    this.hand.StartFlying();
                    return;
                }
            }
        }

        if (!stickingTrigger)
            this.hand.StartCollectHand();

    }
    public void Restart()
    {
       foreach(var obj in this.grabingObjs)
        {
            obj.Restart();
        }
        this.grabingObjs.Clear();
        stickingTrigger = null;
    }
}
