#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Eadon.Rpg.Invector.Magic
{
    public class EadonProjectile : MonoBehaviour
    {
        public float maxDistance = 30;
        public float speed = 1;
        public GameObject target;
        public bool destroyOnCollision = true;
        public float destroyDelay = 0.5f;
        
        private Vector3 _startPosition;
        private Transform _t;
        private Transform _targetT;
        private Vector3 _oldPos;
        private bool _isCollided;
        private bool _isOutDistance;

        private Quaternion _startQuaternion;

        //private float currentSpeed;
        private bool _dropFirstFrameForFixUnityBugWithParticles;

        private void Start()
        {
            _t = transform;
            if (target != null) _targetT = target.transform;
            _startPosition = _t.position;
            _oldPos = _t.position;
            _isCollided = false;
            _isOutDistance = false;
            _startQuaternion = _t.rotation;
        }

        private void Update()
        {
            if (_isOutDistance)
            {
                Destroy(this.gameObject);
            }
            if (!_dropFirstFrameForFixUnityBugWithParticles)
            {
                UpdateWorldPosition();
            }
            else _dropFirstFrameForFixUnityBugWithParticles = false;
        }

        private void UpdateWorldPosition()
        {
            var frameMoveOffset = Vector3.zero;
            var frameMoveOffsetWorld = Vector3.zero;
            if (!_isCollided && !_isOutDistance)
            {
                if (target == null)
                {
                    var currentForwardVector = (Vector3.forward) * (speed * Time.deltaTime);
                    frameMoveOffset = _t.localRotation * currentForwardVector;
                    frameMoveOffsetWorld = _startQuaternion * currentForwardVector;
                }
                else
                {
                    var forwardVec = (_targetT.position - _t.position).normalized;
                    var currentForwardVector = (forwardVec) * (speed * Time.deltaTime);
                    frameMoveOffset = currentForwardVector;
                    frameMoveOffsetWorld = currentForwardVector;
                }
            }

            var currentDistance = (_t.position + frameMoveOffset - _startPosition).magnitude;
            if (!_isOutDistance && currentDistance > maxDistance)
            {
                _isOutDistance = true;

                if (target == null)
                    _t.position = _startPosition +
                                      _t.localRotation * (Vector3.forward) * maxDistance;
                else
                {
                    var forwardVec = (_targetT.position - _t.position).normalized;
                    _t.position = _startPosition + forwardVec * maxDistance;
                }
                Destroy(gameObject);

                _oldPos = _t.position;
                return;
            }

            _t.position = _oldPos + frameMoveOffsetWorld;
            _oldPos = _t.position;
        }
    }
}
#endif
