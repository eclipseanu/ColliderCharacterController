using System.Collections.Generic;
using UnityEngine;

public class ColliderCharacterController : MonoBehaviour {
    //Fields
    [Header("General:")]
    [SerializeField] protected Transform        _trans;
    [SerializeField] protected CapsuleCollider  _coll;
    [SerializeField] protected float            _speed;
    [SerializeField] protected bool             _useGravity;
    [Header("Sleep:")]
    [SerializeField] protected bool             _autoSleep;
    [SerializeField] protected int              _autoSleepAfterFixedUpdates;
    [SerializeField] protected bool             _autoWakeUpOnColliderEvent;
    [Header("Slopes:")]
    [SerializeField] protected float            _slopeLimit;
    [SerializeField] protected bool             _autoSlideDownOnSlopeLimit;
    [Header("Collision:")]
    [SerializeField] protected int              _maxCollisions;
    [SerializeField] protected float            _innerBias;
    [SerializeField] protected float            _groundBias;
    [SerializeField] protected float            _forwardBias;
    [SerializeField] protected LayerMask        _collideWith;
    [SerializeField] protected LayerMask        _depenetrateWith;
    [Header("Events:")]
    [SerializeField] protected bool             _enableColliderEvents;
    [SerializeField] protected LayerMask        _enableColliderEventsWith;

    protected bool                              _isGrounded;
    protected bool                              _isSleeping;
    protected RaycastHit[]                      _lastCollHits;
    protected int                               _lastCollHitAmount;
    protected int                               _highestPointIndex;
    protected float                             _highestPointAngle;
    protected int                               _highestSlopeIndex;
    protected float                             _highestSlopeAngle;
    protected float                             _highestPoint;
    protected int                               _lowestPointIndex;
    protected float                             _lowestPointAngle;
    protected int                               _lowestSlopeIndex;
    protected float                             _lowestSlopeAngle;
    protected float                             _lowestPoint;

    protected int                               _collideWithLayerMask;
    protected int                               _depenetrateWithLayerMask;
    protected int                               _raiseCollisionEventWithLayerMask;
    protected Vector3                           _moveVelocity;
    protected int                               _isSleepingCounter;
    protected Vector3                           _sleepPosition;
    protected Vector3                           _capsuleCastPoint1;
    protected Vector3                           _capsuleCastPoint2;
    protected Vector3                           _capsuleCastHeight;
    protected Vector3                           _groundBiasVector;
    protected float                             _scaledCollRadius;
    protected Vector3                           _gravity;
    protected Quaternion                        _crossVectorModifier;
    protected Dictionary<int, Collider>         _collidedInstanceIds;
    protected HashSet<int>                      _itemsToExit;
    protected Vector3                           _tmpCapsuleCastForwardBiased;
    protected Vector3                           _tmpCapsuleCastDirectionAndLength;
    protected Vector3                           _tmpPenetrationDirection;
    protected float                             _tmpPenetrationDistance;
    protected Vector3                           _tmpWallNormal;
    protected Vector3                           _tmpCrossMove;
    protected Vector3                           _tmpCrossProduct;
    protected float                             _tmpDotProduct;
    protected int                               _tmpCurrentCollidedInstanceId;
    protected bool                              _tmpDetectedCollision;

    public Transform                            Trans                                   { get { return _trans; } set { _trans = value; } }
    public CapsuleCollider                      Coll                                    { get { return _coll; } set { _coll = value; } }
    public float                                Speed                                   { get { return _speed; } set { _speed = value; } }
    public bool                                 UseGravity                              { get { return _useGravity; } set { _useGravity = value; } }
    public bool                                 AutoSleep                               { get { return _autoSleep; } set { _autoSleep = value; } }
    public int                                  AutoSleepAfterFixedUpdates              { get { return _autoSleepAfterFixedUpdates; } set { _autoSleepAfterFixedUpdates = value; } }
    public bool                                 AutoWakeUpOnColliderEvent               { get { return _autoWakeUpOnColliderEvent; } set { _autoWakeUpOnColliderEvent = value; } }
    public float                                SlopeLimit                              { get { return _slopeLimit; } set { _slopeLimit = value; } }
    public bool                                 AutoSlideDownOnSlopeLimit               { get { return _autoSlideDownOnSlopeLimit; } set { _autoSlideDownOnSlopeLimit = value; } }
    public int                                  MaxCollisions                           { get { return _maxCollisions; } set { _maxCollisions = value; } }
    public float                                InnerBias                               { get { return _innerBias; } set { _innerBias = value; } }
    public float                                GroundBias                              { get { return _groundBias; } set { _groundBias = value; } }
    public float                                ForwardBias                             { get { return _forwardBias; } set { _forwardBias = value; } }
    public LayerMask                            CollideWith                             { get { return _collideWith; } set { _collideWith = value; } }
    public LayerMask                            DepenetrateWith                         { get { return _depenetrateWith; } set { _depenetrateWith = value; } }
    public bool                                 EnableColliderEvents                    { get { return _enableColliderEvents; } set { _enableColliderEvents = value; } }
    public LayerMask                            EnableColliderEventsWith                { get { return _enableColliderEventsWith; } }
    public bool                                 IsGrounded                              { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool                                 IsSleeping                              { get { return _isSleeping; } set { _isSleeping = value; } }
    public RaycastHit[]                         LastCollHits                            { get { return _lastCollHits; } }
    public int                                  LastCollHitAmount                       { get { return _lastCollHitAmount; } }
    public int                                  HighestPointIndex                       { get { return _highestPointIndex; } }
    public float                                HighestPointAngle                       { get { return _highestPointAngle; } }
    public int                                  HighestSlopeIndex                       { get { return _highestSlopeIndex; } }
    public float                                HighestSlopeAngle                       { get { return _highestSlopeAngle; } }
    public float                                HighestPoint                            { get { return _highestPoint; } }
    public int                                  LowestPointIndex                        { get { return _lowestPointIndex; } }
    public float                                LowestPointAngle                        { get { return _lowestPointAngle; } }
    public int                                  LowestSlopeIndex                        { get { return _lowestSlopeIndex; } }
    public float                                LowestSlopeAngle                        { get { return _lowestSlopeAngle; } }
    public float                                LowestPoint                             { get { return _lowestPoint; } }

    //Events
    public delegate void ColliderEnterEvent (Collider pCollider);
    public event ColliderEnterEvent OnColliderEnter;
    public delegate void ColliderStayEvent (Collider pCollider);
    public event ColliderStayEvent OnColliderStay;
    public delegate void ColliderExitEvent (Collider pCollider);
    public event ColliderEnterEvent OnColliderExit;
    public delegate void SleepEvent ();
    public event SleepEvent OnSleep;
    public delegate void WakeUpEvent ();
    public event WakeUpEvent OnWakeUp;


    //Functions
    #region Unity
    protected virtual void Awake () {
        Rebuild();
    }

    protected virtual void FixedUpdate () {
        if (!_isGrounded && _useGravity) {
            _trans.position += _gravity;
        }

        if (_autoSlideDownOnSlopeLimit && _isGrounded && _lowestSlopeAngle > _slopeLimit) {
            _trans.position += (Quaternion.LookRotation(_lastCollHits[_lowestSlopeIndex].normal) * Vector3.down).normalized * _speed * Time.fixedDeltaTime;
        }

        if (_autoSleep) {
            if (_isSleeping) {
                if (_sleepPosition != _trans.position) {
                    WakeUp();
                }
            } else {
                if (_sleepPosition == _trans.position) {
                    if (++_isSleepingCounter >= _autoSleepAfterFixedUpdates) {
                        Sleep();
                    }
                } else {
                    _isSleepingCounter = 0;
                    _sleepPosition = _trans.position;
                }
            }
        }
    }

    #endregion

    #region Helpers
    public virtual void Sleep () {
        _isSleeping = true;
        if (_isSleepingCounter < _autoSleepAfterFixedUpdates) {
            _isSleepingCounter = _autoSleepAfterFixedUpdates;
        }

        _sleepPosition = _trans.position;
        RaiseSleep();
    }

    public virtual void WakeUp () {
        _isSleeping = false;
        _isSleepingCounter = 0;
        _sleepPosition = _trans.position;
        RaiseWakeUp();
    }

    public virtual void Move (ref Vector3 pDirection, float pTime) {
        if (pDirection == Vector3.zero) {
            _moveVelocity = pDirection;
            return;
        }

        _moveVelocity = pDirection.normalized;
        if (_isGrounded) {
            if (_highestSlopeAngle <= _slopeLimit) {
                _trans.position += Vector3.Cross(_crossVectorModifier * _moveVelocity, _lastCollHits[_highestSlopeIndex].normal).normalized * _speed * pTime;
            } else {
                _tmpWallNormal = _lastCollHits[_highestSlopeIndex].normal;
                _tmpWallNormal.y = 0f;
                _tmpWallNormal = _tmpWallNormal.normalized;
                if (Vector3.Angle(-_tmpWallNormal, _moveVelocity) > 90f) {
                    _trans.position += Vector3.Cross(_crossVectorModifier * _moveVelocity, _lastCollHits[_lowestSlopeIndex].normal).normalized * _speed * pTime;
                } else {
                    _trans.position += Vector3.Cross(_crossVectorModifier * (_moveVelocity - _tmpWallNormal * (Vector3.Dot(_moveVelocity, _tmpWallNormal))), _lastCollHits[_lowestSlopeIndex].normal) * _speed * pTime;
                }
            }
        } else {
            _trans.position += _moveVelocity * _speed * pTime;
        }
    }

    public virtual void Rebuild() {
        _isGrounded = false;
        _collideWithLayerMask = _collideWith.value;
        _depenetrateWithLayerMask = _depenetrateWith.value;
        _raiseCollisionEventWithLayerMask = _enableColliderEventsWith.value;
        _lastCollHits = new RaycastHit[_maxCollisions];
        _lastCollHitAmount = 0;
        _highestPointIndex = -1;
        _highestSlopeIndex = -1;
        _highestPointAngle = 180f;
        _highestSlopeAngle = 180f;
        _highestPoint = float.MaxValue;
        _lowestPointIndex = -1;
        _lowestSlopeIndex = -1;
        _lowestPointAngle = 180f;
        _lowestSlopeAngle = 180f;
        _lowestPoint = float.MinValue;
        _scaledCollRadius = (_trans.localScale.x >= _trans.localScale.z) ? _trans.localScale.x * _coll.radius : _trans.localScale.z * _coll.radius;
        _capsuleCastPoint1 = _coll.center + Vector3.down * (_coll.height * 0.5f * _trans.localScale.y - _scaledCollRadius - _innerBias);
        _capsuleCastPoint2 = _coll.center + Vector3.up * (_coll.height * 0.5f * _trans.localScale.y - _scaledCollRadius);
        _capsuleCastHeight = Vector3.down * (_innerBias + _groundBias);
        _groundBiasVector = new Vector3(0f, _groundBias, 0f);
        _gravity = new Vector3(0f, Physics.gravity.y, 0f) * Time.fixedDeltaTime;
        _crossVectorModifier = Quaternion.AngleAxis(90f, Vector3.up);
        _collidedInstanceIds = new Dictionary<int, Collider>(_maxCollisions * 2);
        _itemsToExit = new HashSet<int>();
    }

    public virtual void UpdateColliders () {
        if (_isSleeping) {
            if (_autoWakeUpOnColliderEvent) {
                GetColliders();
                if (HasColliderForEvents()) {
                    WakeUp();
                } else {
                    return;
                }
            } else {
                return;
            }
        } else {
            GetColliders();
        }

        AssignHighestAndLowestValues();
        _isGrounded = _lowestPointIndex > -1 && _lastCollHits[_lowestPointIndex].point.y <= _trans.position.y + _capsuleCastPoint1.y - _innerBias;
        Depenetrate();
        RaiseColliderEvents();
    }

    protected virtual void GetColliders () {
        _tmpCapsuleCastForwardBiased = _moveVelocity * _forwardBias;
        _tmpCapsuleCastDirectionAndLength = _capsuleCastHeight + _tmpCapsuleCastForwardBiased * 2f;
        _lastCollHitAmount = Physics.CapsuleCastNonAlloc(_trans.position + _capsuleCastPoint1 - _tmpCapsuleCastForwardBiased, _trans.position + _capsuleCastPoint2 - _tmpCapsuleCastForwardBiased, _scaledCollRadius, _tmpCapsuleCastDirectionAndLength.normalized, _lastCollHits, _tmpCapsuleCastDirectionAndLength.magnitude, _collideWithLayerMask);
    }

    protected virtual bool HasColliderForEvents () {
        for (int i = 0; i < _lastCollHitAmount; i++) {
            if (_raiseCollisionEventWithLayerMask == (_raiseCollisionEventWithLayerMask | (1 << _lastCollHits[i].collider.gameObject.layer))) {
                return true;
            }
        }

        return false;
    }

    protected virtual void RaiseColliderEvents () {
        if (!_enableColliderEvents) {
            return;
        }

        if (_itemsToExit.Count > 0) {
            _itemsToExit.Clear();
        }

        if (_collidedInstanceIds.Count > 0) {
            foreach (var item in _collidedInstanceIds.Keys) {
                _itemsToExit.Add(item);
            }
        }

        _tmpDetectedCollision = false;
        for (int i = 0; i < _lastCollHitAmount; i++) {
            if (_raiseCollisionEventWithLayerMask == (_raiseCollisionEventWithLayerMask | (1 << _lastCollHits[i].collider.gameObject.layer))) {
                _tmpCurrentCollidedInstanceId = _lastCollHits[i].collider.GetInstanceID();
                if (!_collidedInstanceIds.ContainsKey(_tmpCurrentCollidedInstanceId)) {
                    _collidedInstanceIds.Add(_tmpCurrentCollidedInstanceId, _lastCollHits[i].collider);
                    RaiseColliderEnter(_lastCollHits[i].collider);
                } else {
                    RaiseColliderStay(_lastCollHits[i].collider);
                    _itemsToExit.Remove(_tmpCurrentCollidedInstanceId);
                }

                if (!_tmpDetectedCollision) {
                    _tmpDetectedCollision = true;
                    _isSleepingCounter = 0;
                }
            }
        }

        foreach (var item in _itemsToExit) {
            RaiseColliderExit(_collidedInstanceIds[item]);
            _collidedInstanceIds.Remove(item);
        }
    }

    protected virtual void AssignHighestAndLowestValues () {
        _highestPointIndex = -1;
        _lowestPointIndex = -1;
        if (_lastCollHitAmount == 0) {
            return;
        }

        _highestPoint = float.MinValue;
        _lowestPoint = float.MaxValue;
        for (int i = 0; i < _lastCollHitAmount; i++) {
            if (_lastCollHits[i].distance > 0f 
                && _lastCollHits[i].point.y >= _coll.bounds.min.y - _groundBias 
                && _depenetrateWithLayerMask == (_depenetrateWithLayerMask | (1 << _lastCollHits[i].collider.gameObject.layer))) {
                if (_lastCollHits[i].point.y > _highestPoint) {
                    _highestPoint = _lastCollHits[i].point.y;
                    _highestPointIndex = i;
                }

                if (_lastCollHits[i].point.y < _lowestPoint) {
                    _lowestPoint = _lastCollHits[i].point.y;
                    _lowestPointIndex = i;
                }
            }
        }

        if (_highestPointIndex > -1) {
            _highestPointAngle = Vector3.Angle(Vector3.up, _lastCollHits[_highestPointIndex].normal);
        } else {
            _highestPointAngle = 180f;
        }

        if (_lowestPointIndex > -1) {
            _lowestPointAngle = Vector3.Angle(Vector3.up, _lastCollHits[_lowestPointIndex].normal);
        } else {
            _lowestPointAngle = 180f;
        }

        _highestSlopeIndex = (_highestPointAngle >= _lowestPointAngle) ? _highestPointIndex : _lowestPointIndex;
        _highestSlopeAngle = (_highestPointAngle >= _lowestPointAngle) ? _highestPointAngle : _lowestPointAngle;
        _lowestSlopeIndex = (_lowestPointAngle <= _highestPointAngle) ? _lowestPointIndex : _highestPointIndex;
        _lowestSlopeAngle = (_lowestPointAngle <= _highestPointAngle) ? _lowestPointAngle : _highestPointAngle;
    }

    protected virtual void Depenetrate () {
        for (int i = 0; i < _lastCollHitAmount; i++) {
            if (_depenetrateWithLayerMask == (_depenetrateWithLayerMask | (1 << _lastCollHits[i].collider.gameObject.layer))
                && Physics.ComputePenetration(_coll, _trans.position, _trans.rotation, _lastCollHits[i].collider, _lastCollHits[i].collider.transform.position, _lastCollHits[i].collider.transform.rotation, out _tmpPenetrationDirection, out _tmpPenetrationDistance)
                && _tmpPenetrationDistance > 0f) {
                _trans.position += (_tmpPenetrationDirection * _tmpPenetrationDistance);
            }
        }
    }

    #endregion

    #region Events
    protected virtual void RaiseColliderEnter(Collider pCollider) {
        if (OnColliderEnter != null) {
            OnColliderEnter(pCollider);
        }
    }

    protected virtual void RaiseColliderStay (Collider pCollider) {
        if (OnColliderStay != null) {
            OnColliderStay(pCollider);
        }
    }

    protected virtual void RaiseColliderExit (Collider pCollider) {
        if (OnColliderExit != null) {
            OnColliderExit(pCollider);
        }
    }

    protected virtual void RaiseSleep () {
        if (OnSleep != null) {
            OnSleep();
        }
    }

    protected virtual void RaiseWakeUp () {
        if (OnWakeUp != null) {
            OnWakeUp();
        }
    }

    #endregion
}
