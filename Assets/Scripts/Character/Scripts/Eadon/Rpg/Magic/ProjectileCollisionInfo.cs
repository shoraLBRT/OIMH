#if EADON_RPG_INVECTOR
using System;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    public class ProjectileCollisionInfo : EventArgs
    {
        //public ContactPoint ContactPoint;
        public Vector3 hitPoint;
        public Collider hitCollider;
        public GameObject hitGameObject;
    }
}
#endif
