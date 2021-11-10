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
                        Logger.Detailed("Adding Undying effect to {0} ({1}, {2})", creature.name, creature.creatureId, creature.GetInstanceID());
                        if (!creature.isPlayer)
                        {
                            if (!creature.gameObject.TryGetComponent<UndyingCreature>(out UndyingCreature ur))
                            {
                                var uc = creature.gameObject.AddComponent<UndyingCreature>();
                                uc.dieOnHeadChop = dieOnHeadChop;
                            }
                        }
                    }
                }
            }
        }
    }
}
