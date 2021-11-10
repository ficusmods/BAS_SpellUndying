using System;
using System.Collections.Generic;

using ThunderRoad;
using UnityEngine;

namespace SpellUndying
{
    public class SpellUndying : SpellCastLightning
    {
        public bool dieOnHeadChop = false;

        public override void OnSprayLoop()
        {
            base.OnSprayLoop();

            if (base.boltHits[0].collider)
            {
                ColliderGroup BoltHitColliderGroup = base.boltHits[0].collider.GetComponentInParent<ColliderGroup>();
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
                                uc.dieOnHeadChop = dieOnHeadChop;
                            }
                        }
                    }
                }
            }
        }
    }
}
