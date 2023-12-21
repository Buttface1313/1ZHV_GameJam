using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {
    private Vector3 _camOffset;
    public void AddVerticalOffset(float verticalOffset) {
        _camOffset += new Vector3(verticalOffset,0, 0);
    }

    public void AddHorizontalOffset(float horizontalOffset) {
        _camOffset += new Vector3(0, horizontalOffset, 0);
    }

    public void AddOffset(Vector3 offset) {
        _camOffset += offset;
    }

    void LateUpdate() {
        transform.localEulerAngles = _camOffset;

        _camOffset = Vector3.zero;
    }
}
