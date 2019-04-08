using UnityEngine;

public class PlayerInputToController : MonoBehaviour {
    //Fields
    public ColliderCharacterController _controller;
    Vector3 _direction;


    //Functions
    void Awake () {
        _controller.OnColliderEnter += Controller_OnColliderEnter;
        _controller.OnColliderExit += Controller_OnColliderExit;
    }

    void FixedUpdate() {
        // Set our input.
        _direction.x = Input.GetAxis("Horizontal");
        _direction.z = Input.GetAxis("Vertical");

        // Make the ColliderCharacterController move according to input.
        _controller.Move(ref _direction, Time.fixedDeltaTime);

        // Update colliders, physics and raise events.
        _controller.UpdateColliders();
    }

    void Controller_OnColliderEnter (Collider pCollider) {
        Debug.Log("Hey I found " + pCollider.name + "!");
    }

    void Controller_OnColliderExit (Collider pCollider) {
        Debug.Log("Goodbye " + pCollider.name + "!");
    }
}
