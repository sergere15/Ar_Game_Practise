using UnityEngine;
using Wikitude;

public class AlignmentDrawable : MonoBehaviour
{
    public ObjectTracker TargetObjectTracker;

    private Vector3 _positionInitial;
    private Vector3 _positionLast;
    private Quaternion _rotationInitial;
    private Quaternion _rotationLast;

    private float _zoomMin = 0.5f;
    private float _zoomMax = 1.5f;

    /* This is used to rotate around a different pivot point. */
    public Vector3 Offset;

    /* If the smooth transition from the drawable's position to the target's is done, this flag is set to true. */
    [HideInInspector]
    public bool AlignmentDrawableAlignedWithTarget = false;

    /* The recognized target is set to visualize a smooth transition from the drawable to the target. */
    private GameObject _recognizedTargetObject = null;

    private float _zoomAdjustment = 1f;

    public void Initialize() {
        _positionInitial = transform.localPosition;
        _positionLast = _positionInitial;
        _rotationInitial = transform.localRotation;
        _rotationLast = _rotationInitial;

        /* Callbacks are set up to disable or re-enable the alignment initializer. */
        TargetObjectTracker.GetComponentInChildren<ObjectTrackable>(true).OnObjectRecognized.AddListener(OnObjectRecognized);
        TargetObjectTracker.GetComponentInChildren<ObjectTrackable>(true).OnObjectLost.AddListener(OnObjectLost);
    }

    public void SetZoomRange(float min, float max) {
        _zoomMin = min;
        _zoomMax = max;
    }

    public void AddZoom(float value) {
        Vector3 position = transform.localPosition;
        Vector3 adjustedPosition = GetAdjustedPosition();
        position.z = Mathf.Clamp(transform.localPosition.z * (1 - value ), _zoomMin * adjustedPosition.z , _zoomMax * adjustedPosition.z);
        transform.localPosition = position;
        _positionLast = position;
    }

    public void SetZoom(float value) {
        Vector3 position = transform.localPosition;
        Vector3 adjustedPosition = GetAdjustedPosition();
        position.z = Mathf.Clamp(adjustedPosition.z * value, _zoomMin * adjustedPosition.z, _zoomMax * adjustedPosition.z);
        transform.localPosition = position;
        _positionLast = position;
    }

    public float GetZoom() {
        return transform.localPosition.z / (_positionInitial.z * _zoomAdjustment);
    }

    public void AdjustZoom(float value){
        float zoom = GetZoom();
        _zoomAdjustment = value;
        SetZoom(zoom);
    }

    private Vector3 GetAdjustedPosition() {
        return new Vector3(_positionInitial.x, _positionInitial.y, _positionInitial.z * _zoomAdjustment);
    }

    public void AddRotation(Vector3 value) {
        transform.localPosition += Offset;
        transform.Rotate(value, Space.World);
        _rotationLast = transform.localRotation;
        transform.localPosition -= Offset;
    }

    private void Update() {
        if (_recognizedTargetObject != null) {
            if (AlignmentDrawableAlignedWithTarget == false) {
                /* The drawable is smoothly moved, rotated and scaled to the pose of a recognized target. */
                transform.position = Vector3.Lerp(transform.position , _recognizedTargetObject.transform.position, 10f * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation , _recognizedTargetObject.transform.rotation, 10f * Time.deltaTime);
                transform.localScale = Vector3.Lerp(transform.localScale , _recognizedTargetObject.transform.localScale, 10f * Time.deltaTime);

                /* If the transition of the drawable is close to the recognized target's position, the drawable will be considered aligned with the target. */
                if ((transform.position - _recognizedTargetObject.transform.position).magnitude < 0.01f) {
                    AlignmentDrawableAlignedWithTarget = true;
                }

            } else {
                /* The alignment drawable is fully aligned here. Additional modifications like disabling the alignment visualization can be done here. */

                transform.position = _recognizedTargetObject.transform.position;
                transform.rotation = _recognizedTargetObject.transform.rotation;
                transform.localScale = _recognizedTargetObject.transform.localScale;
            }
        } else {
            /* The pose of the alignment drawable is sent to the object tracker, to help it find the object in the desired pose. */
            TargetObjectTracker.UpdateAlignmentPose(Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale));
        }
    }

    private void OnObjectRecognized(ObjectTarget target) {
        _recognizedTargetObject = target.Drawable.transform.parent.gameObject;
    }

    private void OnObjectLost(ObjectTarget target) {
        _recognizedTargetObject = null;
        AlignmentDrawableAlignedWithTarget = false;
        transform.localPosition = _positionLast;
        transform.localRotation = _rotationLast;
        transform.localScale = Vector3.one;
    }
}