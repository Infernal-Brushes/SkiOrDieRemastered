using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI), typeof(CanvasGroup))]
public class FadeTextController : MonoBehaviour {
    [SerializeField] private float _showTime = 1;
    [SerializeField] private float _hideTime = 1;
    [SerializeField] private float _moveTime = 1;
    [SerializeField] private float _yOffset = 10;

    [SerializeField]
    private TextMeshProUGUI _textMeshProUGUI;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private float _currentShowTime;
    private float _currentHideTime;
    private float _currentMoveTime;
    private float _currentMoveValue;

    private void Awake()
    {
        _endPosition = transform.localPosition;
    }

    public void Show(string text) {
        SetDefaultValue();
        StopAllCoroutines();
        _textMeshProUGUI.text = text;
        StartCoroutine(Appear());
        StartCoroutine(MoveAndDisappear());
    }

    private IEnumerator Appear()
    {
        while (_currentShowTime < _showTime) {
            _canvasGroup.alpha = _currentShowTime / _showTime;
            _currentShowTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveAndDisappear()
    {
        _startPosition = new Vector3(_endPosition.x, _endPosition.y - _yOffset, _endPosition.z);
        _textMeshProUGUI.rectTransform.anchoredPosition3D = _startPosition;
        while (_currentMoveTime < _moveTime)
        {
            _textMeshProUGUI.rectTransform.anchoredPosition3D = Vector3.Lerp(_startPosition, _endPosition, _currentMoveValue);
            _currentMoveValue += Time.deltaTime / _moveTime;
            _currentMoveTime += Time.deltaTime;
            yield return null;
        }

        while (_currentHideTime < _hideTime)
        {
            _canvasGroup.alpha = Mathf.Abs(_currentHideTime / _hideTime - 1);
            _currentHideTime += Time.deltaTime;
            yield return null;
        }
    }

    private void SetDefaultValue() {
        _currentShowTime = 0;
        _currentHideTime = 0;
        _currentMoveTime = 0;
        _currentMoveValue = 0;
        _canvasGroup.alpha = 0;
    }
}