using System;
using System.Collections.Generic;

using ThunderRoad;
using UnityEngine;

namespace SpellUndying
{
    public class SpellUndying : SpellCastLightning
    {

        LinkedList<Creature> undying_list = new LinkedList<Creature>();

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
                        if (!undying_list.Contains(creature))
                        {
                            creature.OnDamageEvent += Creature_OnDamageEvent;
                            undying_list.AddLast(creature);
                            creature_to_max_health(creature);
                            foreach(RagdollPart part in creature.ragdoll.parts)
                            {
                                part.data.sliceForceKill = false;
                            }
                        }
                    }
                }
            }
        }


        private void Creature_OnDamageEvent(CollisionInstance collisionInstance)
        {
            RagdollPart rp = collisionInstance.damageStruct.hitRagdollPart;
            if (!rp) return;
            Creature creature = rp.ragdoll.creature;
            if (!undying_list.Contains(creature)) return;

            if (collisionInstance.damageStruct.baseDamage != 0xDEAD2BAD)
            {
                creature_to_max_health(creature);
            }

            if (!collisionInstance.sourceColliderGroup) return;
            Item item = collisionInstance.sourceColliderGroup.collisionHandler.item;
            if (!item) return;

            foreach (Imbue imbue in item.imbues)
            {
                if (imbue.spellCastBase != null)
                {
                    if (imbue.spellCastBase.id == this.id)
                    {
                        if (check_valid_stab(collisionInstance))
                        {
                            kill_creature(creature, collisionInstance);
                            break;
                        }
                    }
                }
            }
        }

        private bool check_valid_stab(CollisionInstance ci)
        {
            return (ci.damageStruct.damageType == DamageType.Pierce
                && (ci.damageStruct.hitRagdollPart.type == RagdollPart.Type.Neck
                    || ci.damageStruct.hitRagdollPart.type == RagdollPart.Type.Head)
                && ci.damageStruct.penetrationDeepReached);
        }

        private void kill_creature(Creature creature, CollisionInstance ci)
        {
            undying_list.Remove(creature);
            CollisionInstance collisionInstance = new CollisionInstance(new DamageStruct(DamageType.Energy, 0xDEAD2BAD));

            creature.maxHealth = 0xDEAD2BAD - 1.0f;
            creature.currentHealth = creature.maxHealth;
            creature.Damage(collisionInstance);
        }

        private void creature_to_max_health(Creature c)
        {
            c.maxHealth = float.PositiveInfinity;
            c.currentHealth = c.maxHealth;
        }
    }
}
