﻿using System;
using System.Collections.Generic;

using ThunderRoad;
using UnityEngine;

namespace SpellUndying
{
    public class SpellUndying : SpellCastLightning
    {

        public override void OnSprayLoop()
        {
            base.OnSprayLoop();

            int count = HitLookup(this.coneAngle, this.currentCharge * this.boltMaxRange);
            if (count <= 0)
                return;
            SpellCastLightning.BoltHit boltHit = count > 1 ? PickBoltHit(count) : this.boltHits[0];
            if (boltHit.collider != null)
            {
                ColliderGroup BoltHitColliderGroup = boltHit.collider.GetComponentInParent<ColliderGroup>();
                if (BoltHitColliderGroup)
                {
                    RagdollPart rp = BoltHitColliderGroup.collisionHandler.ragdollPart;
                    if (rp)
                    {
                        Creature creature = rp.ragdoll.creature;
                        if (!creature.isPlayer)
                        {
                            if (!creature.gameObject.TryGetComponent<UndyingCreatureModule>(out UndyingCreatureModule ur))
                            {
                                Logger.Detailed("Adding Undying effect to {0} ({1}, {2})", creature.name, creature.creatureId, creature.GetInstanceID());
                                var uc = creature.gameObject.AddComponent<UndyingCreatureModule>();
                                creature.OnKillEvent += (CollisionInstance collisionInstance, EventTime eventTime) => { GameObject.Destroy(uc); };
                                creature.OnDespawnEvent += (EventTime eventTime) => { GameObject.Destroy(uc); };
                            }
                        }
                    }
                }
            }
        }
    }
}
