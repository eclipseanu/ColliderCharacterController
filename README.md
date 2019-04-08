# ColliderCharacterController
A CharacterController that works without a RigidBody attached. Exposes optional custom-events like `OnColliderEnter` or `OnColliderExit` for a streamlined integration. Works for `GameObjects` that move at a low to medium speed.

## The Good
* Gets everything done in a singular `Physics.CapsuleCastNonAlloc()`.
* Can sleep like RigidBodies to free up resources.
* Performance spikes due to excessive collision can be avoided as you can set the max-amount of Colliders that can be processed at a time.
* Can be completely used without a RigidBody as it exposes events for detecting and handling Colliders internally (ex.: `OnColliderEnter`).
* Remains compatible with RigidBodies if they should be used for pushing stuff around via the Physics-Engine.
* Remains compatible Unity-Events like `OnTriggerEnter` etc. should a kinematic RigidBody with a Collider set to `IsTrigger` be used.

## The Bad
* You need to call `Rebuild` when the scale of the Transform or its Collider-values change.
* Hard edges that face upward might cause light jittering.
* Made for simple games with simple surfaces (like Zelda). Complex or very detailed terrain might cause jittering.

# Parameter Explanation
### Sleep
* `AutoSleep` - Similar to a RigidBody this will allow the Controller to sleep and stop checking for collisions until it is woken up.
* `AutoSleepAfterFixedUpdates` - The amount of `FixedUpdates()` until the Controller automatically should go to sleep.
* `AutoWakeUpOnColliderEvent` - This will make the Controller continue to check for Colliders even when sleeping. When a Collider is detected that would raise `OnColliderEnter` the Controller is automatically being woken up.

### Slopes
* `SlopeLimit` - The angle of the slope the Controller is allowed to move on. The Controller can't move up a slope that is above the limit and is only able to walk it down.
* `AutoSlideDownOnSlopeLimit` - If the Controller is on a surface that is above the `SlopeLimit` he will automatically be pushed down the slope.

### Collision
* `MaxCollisions` - Limits the amount of collisions and/or Colliders that can be detected at once.
* `InnerBias` - The higher this value the higher the startingpoint of the `Physics.CapsuleCastNonAlloc()` will be and is essential for ground-detection. `0.25f` is usually a good value to be able to detect the ground below you.
* `GroundBias` - The higher this value the more the `Physics.CapsuleCastNonAlloc()` will cast downwards beyond the CapsuleCollider itself. This helps avoiding jittering on hard edges and `0.15f` is usually a good value.
* `ForwardBias` - The higher this value the more the CapsuleCollider will cast forward to anticipate any walls or slopes in his direction and not run into them. `0.1f` is usually a good value but you will need to set this higher when moving very quickly.
* `CollideWith` - A `LayerMask` that describes the layers from where colliders should be detected. Layers that are not included here can not fire any events(!!!) nor can the Controller collide with them physically(!!!).
* `DepenetrateWith` -  A `LayerMask` that specifically states what layers the Controller should _physically_ be able to collide with.

### Events
* `EnableColliderEvents` - Enables `OnColliderEnter`, `OnColliderStay` and `OnColliderExit` to be fired.
* `EnableColliderEventsWith` -  A `LayerMask` that specifically states what layers should raise the `OnCollider`-events.
