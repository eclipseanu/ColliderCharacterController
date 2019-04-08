using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMov : MonoBehaviour {
    public bool rotate;
    public float rotateSpeed;
    public bool moveLeftRight;
    public float moveSpeed;
    public float moveDistance;
    float _movedDistance;

    void FixedUpdate() {
        if (rotate) {
            transform.Rotate(Vector3.up, rotateSpeed);
        }

        if (moveLeftRight) {
            if (_movedDistance > moveDistance) {
                moveSpeed = -moveSpeed;
            } else if (_movedDistance < 0f) {
                moveSpeed = Mathf.Abs(moveSpeed);
            }
            _movedDistance += moveSpeed * Time.deltaTime;
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        //transform.position += new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * 5f * Time.fixedDeltaTime;
    }
}
