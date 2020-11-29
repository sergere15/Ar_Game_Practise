using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Wikitude;

[RequireComponent(typeof(Camera))]
public class AlignmentInteractionController : MonoBehaviour
{
    public AlignmentDrawable Drawable;

    public GameObject UIInteractionHint;
    public Slider ZoomSlider;
    public bool ZoomSliderIsDragged { get; set; }

    public GameObject LoadingIndicator;

    private Camera _sceneCamera;
    private WikitudeCamera _wikitudeCamera;
    private Camera _alignmentCamera;

    /* The last mouse position has to be stored to calculate drag gestures. */
    private Vector2 _lastMousePosition;

    /* This value is used to restore the previous culling setting after rendering for the camera is done. */
    private bool _wasCullingInverted;

    private bool _alignmentInactive = false;

    private float _initialFieldOfView;

    void Start() {
        _sceneCamera = Camera.main;
        _sceneCamera.enabled = false;

        Drawable.Initialize();

        _alignmentCamera = GetComponent<Camera>();
        _alignmentCamera.clearFlags = CameraClearFlags.Depth;
        _initialFieldOfView = _alignmentCamera.fieldOfView;

        /* The alignment camera has to be drawn on top of the scene camera. */
        _alignmentCamera.depth = _sceneCamera.depth + 1;

        /* Sets the zoom range. */
        ZoomSlider.minValue = 0.25f;
        ZoomSlider.maxValue = 1.75f;
        Drawable.SetZoomRange(ZoomSlider.minValue, ZoomSlider.maxValue);
        ZoomSlider.value = 1f;

        ZoomSlider.onValueChanged.AddListener(OnZoomSliderValueChanged);

#if !UNITY_EDITOR
        /* Remove zoom slider if outside of the Unity Editor. */
        DestroyImmediate(ZoomSlider.transform.parent.gameObject);
#endif
        LoadingIndicator.SetActive(true);
    }

    private void Update() {
        if (_wikitudeCamera == null) {
            _wikitudeCamera = _sceneCamera.GetComponent<WikitudeCamera>();
        }

        if (LoadingIndicator.activeSelf) {
            /* The tracker is currently loading the target. */
            return;
        }

        _alignmentCamera.transform.position = _sceneCamera.transform.position;
        _alignmentCamera.transform.rotation = _sceneCamera.transform.rotation;

        if (Drawable.AlignmentDrawableAlignedWithTarget) {
            if (_alignmentInactive != true) {
                _alignmentCamera.enabled = false;
                _sceneCamera.enabled = true;

                UIInteractionHint.SetActive(false);
                if (ZoomSlider != null) {
                    ZoomSlider.transform.parent.gameObject.SetActive(false);
                }
                _alignmentInactive = true;
            }

            return;
        }

        /* The alignment drawables have to be notified, if the scene camera FOV changes. */
        if (!Mathf.Approximately(_alignmentCamera.fieldOfView, _sceneCamera.fieldOfView)) {
            _alignmentCamera.fieldOfView = _sceneCamera.fieldOfView;
            float changeInFOV = _initialFieldOfView / _alignmentCamera.fieldOfView;
            Drawable.AdjustZoom(changeInFOV);
        }

        if (_alignmentInactive) {
            _alignmentCamera.enabled = true;
            _sceneCamera.enabled = false;

            UIInteractionHint.SetActive(true);
            if (ZoomSlider != null) {
                ZoomSlider.transform.parent.gameObject.SetActive(true);
            }
            _alignmentInactive = false;
        }


        /* If the view is mirrored in case of using a mirrored webcam or the remote front camera
           for live preview, the rotation gestures also have to be mirrored correctly. */
        float flipHorizontalValue = _wikitudeCamera.FlipHorizontal ? -1f : 1f;

        /* Skip gestures if the zoom slider is interacted with. */
        if (ZoomSliderIsDragged == false) {
            /* Interaction logic for handling two-finger scale and rotation gestures. */
            if (Input.touchCount >= 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)) {
                Touch touchIdZero = Input.GetTouch(0);
                Touch touchIdOne = Input.GetTouch(1);

                Vector2 prevTouchIdZero = touchIdZero.position - touchIdZero.deltaPosition;
                Vector2 prevTouchIdOne = touchIdOne.position - touchIdOne.deltaPosition;

                float prevTouchDistance = (prevTouchIdZero - prevTouchIdOne).magnitude;
                float touchDistance = (touchIdZero.position - touchIdOne.position).magnitude;
                float touchDistancesDelta = touchDistance - prevTouchDistance;

                Drawable.AddZoom(touchDistancesDelta / Mathf.Min(Screen.width, Screen.height));

                if (ZoomSlider != null) {
                    ZoomSlider.value = Drawable.GetZoom();
                }

                float rotation = Vector2.SignedAngle(prevTouchIdZero - prevTouchIdOne, touchIdZero.position - touchIdOne.position);
                float rotationMultiplier =  180f / Mathf.Min(Screen.width, Screen.height);
                Drawable.AddRotation(new Vector3(0f, 0f, flipHorizontalValue * rotation * rotationMultiplier));

                /* In case one finger gets lifted, the last mouse position has to be invalidated. */
                _lastMousePosition = Vector2.zero;
            } else if (Input.touchCount < 2 ) {
                /* The mouse input works for both, the mouse input and single finger input. */
                if (Input.GetMouseButtonDown(0)){
                    _lastMousePosition = Input.mousePosition;
                } else if (Input.GetMouseButton(0)) {
                    /* This condition is met if a finger during two-finger gestures is lifted. */
                    if (_lastMousePosition.Equals(Vector2.zero)) {
                        _lastMousePosition = Input.mousePosition;
                    } else {
                        Vector2 mousePosition = Input.mousePosition;
                        Vector2 deltaMousePosition = mousePosition - _lastMousePosition;
                        _lastMousePosition = mousePosition;

                        float rotationMultiplier =  180f / Mathf.Min(Screen.width, Screen.height);

                        Drawable.AddRotation(new Vector3(deltaMousePosition.y * rotationMultiplier, -flipHorizontalValue * deltaMousePosition.x  * rotationMultiplier, 0f));
                    }
                }
            }
        }
    }

    public void OnTargetFinishedLoading() {
        Drawable.gameObject.SetActive(true);
        LoadingIndicator.SetActive(false);
    }

    public void OnErrorLoadingTargets(Error error) {
        /* The separate error callback will display the error, so we just hide the LoadingIndicator here. */
        LoadingIndicator.SetActive(false);
    }

    private void OnZoomSliderValueChanged(float value) {
        Drawable.SetZoom(value);
    }

    /* If the scene renderer has a mirrored view or inverted culling, the settings are also applied to this camera. */
    private void OnPreRender() {
        _alignmentCamera.ResetWorldToCameraMatrix();
        _alignmentCamera.ResetProjectionMatrix();
        _alignmentCamera.projectionMatrix = _alignmentCamera.projectionMatrix * Matrix4x4.Scale(_wikitudeCamera.FlipHorizontal ? new Vector3(-1,1,1) : Vector3.one);

        _wasCullingInverted = GL.invertCulling;
        GL.invertCulling = _wikitudeCamera.InvertCulling || _wikitudeCamera.FlipHorizontal;
    }

    private void OnPostRender() {
        GL.invertCulling = _wasCullingInverted;
    }
}
