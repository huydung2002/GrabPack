using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    public SpriteRenderer sprite;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Grap")
        {
            sprite.color = Color.red;
        }
        
        Debug.Log("cham");
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Grap")
        {
            sprite.color = Color.white;
        }
        Debug.Log("exit");
    }


}
