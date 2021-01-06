using UnityEngine;

[System.Obsolete]
public class RandomPan : MonoBehaviour
{
    [SerializeField] private Vector2 _maxRange = Vector2.zero;
    [SerializeField] private Vector2 _minRange = Vector3.zero;
    [SerializeField] private float _panSpeed = 1f;
    private Vector3 _startPos;
    private Vector2 _newPos;

    private void Awake() => _startPos = transform.position;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _newPos, _panSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, _newPos) < 0.5f)
        {
            GetNewPosition();
        }
    }

    private void GetNewPosition()
    {
        float panX = Random.Range(_minRange.x, _maxRange.x);
        float panY = Random.Range(_minRange.y, _maxRange.y);
        _newPos = new Vector2(_startPos.x + panX, _startPos.y + panY);
    }
}
