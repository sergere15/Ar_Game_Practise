using UnityEngine;
using System.Collections.Generic;

public class SafeAreaHandler : MonoBehaviour
{
    public List<RectTransform> UITop;
    public List<RectTransform> UIFullWidth;
    public List<RectTransform> UIBottom;

    private ScreenOrientation _lastScreenOrientation;
    private float _maxSafeAreaOffset;

    private void Start() {
        _lastScreenOrientation = Screen.orientation;
        UpdateSafeArea();
    }

    private void Update() {
        // Checks if orientation of the device changed and triggers update of UI elements
        if (_lastScreenOrientation != Screen.orientation) {
            _lastScreenOrientation = Screen.orientation;
            UpdateSafeArea();
        }
    }

    private void UpdateSafeArea () {
        Rect safeArea = Screen.safeArea;

        // Skip calculations beforehand, if the safe area is equal to the full screen.
        if (safeArea == new Rect(0f, 0f, Screen.width, Screen.height)) {
            return;
        }

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchorMin = safeArea.position / screenSize;
        Vector2 anchorMax = (safeArea.position + safeArea.size) / screenSize;

        Vector2 offset = safeArea.size - new Vector2(Screen.width, Screen.height);
        _maxSafeAreaOffset = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y));

        RectTransform rectTransform = GetComponent<RectTransform> ();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        // Update top and bottom elements only if the safe area restricts in height.
        if (safeArea.height != Screen.height) {
            UpdateTopElements();
            UpdateBottomElements();
        }

        // Update full width elements only if the safe area restricts in width.
        if (safeArea.width != Screen.width) {
            UpdateFullWidthElements();
        }
    }

    private void UpdateTopElements() {
        foreach(RectTransform element in UITop) {
            element.sizeDelta = new Vector2(element.sizeDelta.x, _maxSafeAreaOffset);
            element.anchoredPosition = new Vector2(element.anchoredPosition.x, _maxSafeAreaOffset);
        }
    }

    private void UpdateFullWidthElements() {
        foreach(RectTransform element in UIFullWidth) {
            element.sizeDelta = new Vector2(_maxSafeAreaOffset * 2, element.sizeDelta.y);
        }
    }

    private void UpdateBottomElements() {
        foreach(RectTransform element in UIBottom) {
            if (UITop.Contains(element)) {
                element.sizeDelta = new Vector2(element.sizeDelta.x, _maxSafeAreaOffset * 2);
                element.anchoredPosition = new Vector2(element.anchoredPosition.x, 0f);  
            } else {
                element.sizeDelta = new Vector2(element.sizeDelta.x, _maxSafeAreaOffset);
                element.anchoredPosition = new Vector2(element.anchoredPosition.x, -_maxSafeAreaOffset);        
            }
        }
    }
}
