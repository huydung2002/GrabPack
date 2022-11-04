using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum HandColor { 
    RED,
    BLUE,
    ALL
}

public class GrabHand : MonoBehaviour
{
    public HandColor color;
    public PolygonPathFinding pathFinding;
    public HandDrag handDrag;
    public List<Vector2> points = new List<Vector2>();
    public LineRenderer lineRenderer;
    public bool collecting = false;
    public bool flying = false;
    public float collectSpeedStart = 10;
    public float collectSpeedAcc = 10;
    private float crCollectSpeed;
    private bool lastLineOfSight;
    public bool wrongLine;
    private Character character;
    private Vector3 characterOffset;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponentInParent<Character>();
        characterOffset = this.transform.InverseTransformPoint(character.transform.position);
        UpdateLineColor(true);
        //this.handDrag.GetComponent<SpriteRenderer>().color = color == HandColor.RED ? Color.red : Color.green;
        lineRenderer.sortingLayerName = "Foreground";
    }
    private void UpdateLineColor(bool wrong)
    {
        var c = wrong ? Color.grey : Color.white;
        lineRenderer.startColor = lineRenderer.endColor = c;
    }
    // Update is called once per frame
    void Update()
    {
        this.points[0] = this.transform.position;
        if(handDrag.dragging) 
        {
            FindObjectOfType<AudioManager>().Play("Drag");
            UpdateDraggingHand();
        }
        if(collecting)
        {
            UpdateCollectHand();
        }
        if (flying)
            UpdateFlyToHand();

        if (points != null) {
            lineRenderer.positionCount = this.points.Count + 1;
            for(int i = 0; i < this.points.Count; i ++)
            {
                lineRenderer.SetPosition(i, new Vector3(this.points[i].x, this.points[i].y));
            }
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, handDrag.transform.position);
        //for (int i = 0; i < points.Count - 1; i ++)
        //{
        //    Debug.DrawLine(points[i], points[i + 1], Color.green);
        //}
        //    Debug.DrawLine(points[points.Count - 1], to.position, Color.green);
        }
        //Debug.DrawLine(startPoint, endPoint, Color.blue);



    }
    private void UpdateDraggingHand()
    {
        Vector3 startPoint = this.points[this.points.Count - 1];
        Vector3 endPoint = handDrag.transform.position;
        //check trigger
        var triggers = Physics2D.LinecastAll(startPoint, endPoint);
        foreach(var trigger in triggers) { 
            if (trigger.collider && trigger.collider.CompareTag("Hazard"))
            {
                GameManager.instance.loseStatus = LoseStatus.Lose;
                GameManager.instance.EndGame(false);
                return;
            }
            if (trigger.collider && trigger.collider.CompareTag("Saw"))
            {
                GameManager.instance.loseStatus = LoseStatus.Lose1;
                GameManager.instance.EndGame(false);
                return;
            }
            if (trigger.collider && trigger.collider.CompareTag("Electric"))
            {
                GameManager.instance.loseStatus = LoseStatus.Lose2;
                GameManager.instance.EndGame(false);
                return;
            }
        }
        //if (lastLineOfSight)
        {
            //   Debug.DrawLine(startPoint, endPoint, Color.red);
            var addPoints = pathFinding.FindPath(startPoint, endPoint);
            var lastPoint = this.points[this.points.Count - 1];
            if (addPoints != null)
            {
                if (addPoints.Count > 2)
                {
                    for (int i = 0; i < addPoints.Count - 1; i++)
                    {
                        Debug.DrawLine(addPoints[i], addPoints[i + 1], Color.white, 10);
                    }

                    addPoints.RemoveAt(addPoints.Count - 1); //remove hand point

                    if(this.points.Count > 2) {
                        if (this.points[this.points.Count - 1] == addPoints[addPoints.Count - 2] && this.points[this.points.Count - 2] == addPoints[addPoints.Count - 1])
                        {
                            return;
                        }
                    }

                    this.points.RemoveAt(this.points.Count - 1);
                    this.points.AddRange(addPoints);
                }
                else if (this.points.Count > 1)
                {
                    var semiLastPoint = this.points[this.points.Count - 2];
                    addPoints = pathFinding.FindPath(semiLastPoint, endPoint);
                    if (addPoints != null && addPoints.Count == 2)
                    {
                        this.points.RemoveAt(this.points.Count - 1);
                    }
                }
            }
        }
        //else
        //{
        //    Debug.DrawLine(startPoint, endPoint, Color.red);
        //}
        wrongLine = !pathFinding.InLineOfSight(this.points[this.points.Count - 1], endPoint);
        UpdateLineColor(wrongLine);
    }
    private void UpdateCollectHand()
    {
        Vector3 lastPoint = this.points[this.points.Count - 1];
        Vector3 handPos = handDrag.transform.position;

        Vector3 dir = handPos - lastPoint;
        float moveDist = this.crCollectSpeed * Time.deltaTime;
        float dist = Vector2.Distance(lastPoint, handPos);

        if (moveDist >= dist)
        {
            moveDist = dist;
            if (this.points.Count > 1)
            {
                this.points.RemoveAt(this.points.Count - 1);
            }
            else collecting = false;
        }
        handPos -= dir.normalized * moveDist;
        handDrag.transform.position = handPos;
        this.crCollectSpeed += Time.deltaTime * this.collectSpeedAcc;
    }

    private void UpdateFlyToHand()
    {
        Vector3 startPoint = this.points[0];
        Vector3 handPos = handDrag.transform.position;
        Vector3 nextPoint = this.points.Count > 1 ? this.points[1] : handPos;

        Vector3 dir = nextPoint - startPoint;
        float moveDist = this.crCollectSpeed * Time.deltaTime;
        float dist = Vector2.Distance(startPoint, nextPoint);

        if (moveDist >= dist)
        {
            moveDist = dist;
            if (this.points.Count > 1)
            {
                this.points.RemoveAt(1);
            }
            else flying = false;
        }
        Vector3 moveVector = dir.normalized * moveDist;
        this.character.transform.position += moveVector;
        float angle = Vector3.Angle(dir, this.transform.up);
        this.character.transform.RotateAround(this.transform.position, Vector3.forward, angle);
        this.crCollectSpeed += Time.deltaTime * this.collectSpeedAcc;
        handDrag.transform.position = handPos;
    }


    public void StartCollectHand()
    {
        this.collecting = true;
        this.crCollectSpeed = collectSpeedStart;
    }
    public void StartFlying()
    {
        this.flying = true;
        this.crCollectSpeed = collectSpeedStart;
    }
    private void OnDisable()
    {
        this.handDrag.enabled = false;
        this.handDrag.dragging = false;
        this.handDrag.GetComponent<Collider2D>().enabled = false;
    }
    public void Restart()
    {
        this.flying = false;
        this.collecting = false;
        this.handDrag.enabled = true;
        this.handDrag.GetComponent<Collider2D>().enabled = true;
        this.points = new List<Vector2>() { this.transform.position };
        this.handDrag.transform.position = this.transform.position;
        handDrag.Restart();
    }
}
