using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvacedMathStuff;

/* Handles positioning of cards, also can show the arc as a line
 * if drawCards is enabled. This script runs in the editor. */

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class CardHand : MonoBehaviour
{
    //Curve shit
    [Header("CardHand")]
    public bool showArc = true; //Visual representation of the bezier (enables/ disables line renderer)
    public bool autoCalulateArc = true; //Automatically calulate the width and height based on card count
    [Range(0, 3)] public float arcWidth = 2f; //Distance from the center to the side of the arc
    [Range(0, 3)] public float arcHeight = 0.6f; //Distance from the center to the top of the arc
    public float cardXRot = 30f;
    public int vertexCount = 50; //Detail of the line renderer arc
    public Transform p0, p1, p2; //These are important

    //My refereces
    private LineRenderer lineRenderer;

    private void Start()
    {
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        lineRenderer.enabled = showArc;
        if (autoCalulateArc) CalcCurve();
        DrawCurve();
        SetCardPos();
    }

    void CalcCurve()
    {
        arcWidth = cardNumber / 4.0f;
        arcWidth = Mathf.Clamp(arcWidth, 0.0f, 3.0f);

        arcHeight = cardNumber / (40.0f / 3.0f);
        arcHeight = Mathf.Clamp(arcHeight, 0.3f, 1.2f);
    }

    void SetCardPos()
    {
        int index = 0;

        foreach(Transform transform in Cards)
        {
            if (!transform.TryGetComponent(out Card card)) //There shouldn't be anything here that isn't a card, but just in case
            {
                Debug.Log($"Uh oh, that isn't a card! {transform.name} doesn't have a Card script.", transform);
                transform.SetParent(null); //Get outta here!
                continue;
            } //Check for card script

            float t = (index + 0.5f) / cardNumber; //Position in the curve

            Vector3 position = AdvMath.Bezier(t, p0.position, p1.position, p2.position);
            float rotation = 0;

            if (cardNumber > 1) //To prevent division by 0 errors when there is only one card
            {
                float totalArc = arcWidth * 10;
                float rotPerCard = totalArc / (cardNumber - 1);
                float startRot = -1f * (totalArc / 2f);

                rotation = startRot + (index * rotPerCard);
            }

            transform.eulerAngles = new Vector3(cardXRot, 0, -rotation);
            transform.position = position;

            card.sortingOrder = index;

            index++;
        }
    }

    void DrawCurve()
    {
        if (!p0 || !p1 || !p2) return; //At least 3 points are required

        p0.localPosition = Vector3.zero.X(-arcWidth); //Move to the left by the width
        p2.localPosition = Vector3.zero.X(arcWidth); //Move to the right by the width
        p1.localPosition = Vector3.zero.Y(arcHeight); //Move up by the height

        if (showArc)
        {
            List<Vector3> pointList = new List<Vector3>();

            for (float t = 0; t <= 1; t += 1.0f / vertexCount) //t is a point on the line and must be >= 0 & <= 1; 0 would be the start of the line and 0.5 would be in the middle
            {
                Vector3 point = AdvMath.Bezier(t, p0.position, p1.position, p2.position); //Bezier calulates a position on the curve given an interpolant (t) and 3-4 (3 here) control points
                pointList.Add(point);
            }

            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPositions(pointList.ToArray()); //Sets every point at once (pretty handy, eh?)
        }

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(p0.position, p1.position);

        Gizmos.DrawLine(p1.position, p2.position);
    }

    #region READ_ONLY

    public int cardNumber { get { return Cards.childCount; } }

    private Transform _cards;
    public Transform Cards
    {
        get
        {
            if (!_cards)
                _cards = transform.Find("Cards");
            if (!_cards)
            {
                _cards = new GameObject("Cards").transform;
                _cards.localPosition = Vector3.zero;
                _cards.SetParent(transform);
            }
            return _cards;
        }
    }

    #endregion
}
