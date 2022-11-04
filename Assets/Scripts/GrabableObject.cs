using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    private Vector3 startPos;
    private HandDrag hand;
    // Start is called before the first frame update
    void Start()
    {
        startPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (hand) this.transform.position = hand.transform.position;
    }

    public void SetGrabbed(HandDrag hand)
    {
        this.hand = hand;
    }
    public void Restart()
    {
        this.hand = null;
        this.transform.position = startPos;

    }
}
