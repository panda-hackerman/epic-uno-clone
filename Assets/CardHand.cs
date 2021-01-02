using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvacedMathStuff;

/* Handles positioning of cards, also can show the arc as a line
 * if drawCards is enabled. This script runs in the editor. */
//TODO: Maybe add an option to use a Cubic bezier (4 points)

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class CardHand : MonoBehaviour
{
    //Curve shit
    [Header("CardHand")]
    public bool showArc = true;
    public bool autoCalulateArc = true;
    [Range(0, 3)] public float arcWidth = 2f;
    [Range(0, 3)] public float arcHeight = 0.6f;
    public float cardXRot = 30f;
    public int vertexCount = 50;
    public Transform p0, p1, p2;

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
        arcWidth = cardsInHand / 4.0f;
        arcWidth = Mathf.Clamp(arcWidth, 0.0f, 3.0f);

        arcHeight = cardsInHand / (40.0f / 3.0f);
        arcHeight = Mathf.Clamp(arcHeight, 0.3f, 1.2f);
    }

    void SetCardPos()
    {
        int index = 0;

        foreach(Transform transform in cards)
        {
            if (!transform.TryGetComponent(out Card card)) //There shouldn't be anything here that isn't a card, but just in case
            {
                Debug.Log($"Uh oh, that isn't a card! {transform.name} doesn't have a Card script.", transform);
                transform.SetParent(null); //Get outta here!
                continue;
            }

            float t = (index + 0.5f) / cardsInHand;

            Vector3 position = AdvMath.Bezier(t, p0.position, p1.position, p2.position);
            float rotation = 0;

            if (cardsInHand > 1)
            {
                float totalArc = arcWidth * 10;
                float rotPerCard = totalArc / (cardsInHand - 1);
                float startRot = -1f * (totalArc / 2f);

                rotation = startRot + (index * rotPerCard);
            }

            transform.eulerAngles = new Vector3(cardXRot, 0, -rotation);
            transform.position = position;

            card.spriteRenderer.sortingOrder = index;

            index++;
        }
    }

    void DrawCurve()
    {
        if (!p0 || !p1 || !p2) return; //At least 3 points are required

        p0.localPosition = Vector3.zero.X(-arcWidth);
        p2.localPosition = Vector3.zero.X(arcWidth);
        p1.localPosition = Vector3.zero.Y(arcHeight);

        List<Vector3> pointList = new List<Vector3>();

        for (float t = 0; t <= 1; t += 1.0f / vertexCount) //t is a point on the line and must be >= 0 & <= 1; 0 would be the start of the line and 0.5 would be in the middle
        {
            Vector3 point = AdvMath.Bezier(t, p0.position, p1.position, p2.position); //Bezier calulates a position on the curve given an interpolant (t) and 3-4 control points
            pointList.Add(point);
        }

        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPositions(pointList.ToArray());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(p0.position, p1.position);

        Gizmos.DrawLine(p1.position, p2.position);
    }

    #region READ_ONLY

    public int cardsInHand { get { return cards.childCount; } }

    private Transform _cards;
    public Transform cards
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
