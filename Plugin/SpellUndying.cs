using System;
using System.Collections.Generic;

using ThunderRoad;
using UnityEngine;

namespace SpellUndying
{
    public class SpellUndying : SpellCastLightning
    {

        Dictionary<Collider, Creature> collider_map = new Dictionary<Collider, Creature>();
        LinkedList<Creature> undying_list = new LinkedList<Creature>();

        SpellUndying()
        {
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
            EventManager.onCreatureKill += EventManager_onCreatureKill;
        }

        private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            foreach (RagdollPart rp in creature.ragdoll.parts)
            {
                foreach (Collider c in rp.colliderGroup.colliders)
                {
                    collider_map.Remove(c);
                }
            }
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            foreach(RagdollPart rp in creature.ragdoll.parts)
            {
                foreach(Collider c in rp.colliderGroup.colliders)
                {
                    collider_map[c] = creature;
                }
            }
        }

        public override void Fire(bool active)
        {
            base.Fire(active);
            foreach(BoltHit hit in base.boltHits)
            {
                Collider collider = hit.collider;
                if(collider != null && collider_map.ContainsKey(collider))
                {
                    Creature creature = collider_map[collider];
                    creature.OnDamageEvent += Creature_OnDamageEvent;
                    foreach (CreatureData.PartData part in creature.data.ragdollData.parts)
                    {
                        part.sliceForceKill = false;
                    }
                    undying_list.AddLast(creature);
                }
            }
        }

        private void Creature_OnDamageEvent(CollisionInstance collisionInstance)
        {
            if (collisionInstance.targetCollider != null)
            {
                Creature creature = collider_map[collisionInstance.targetCollider];
                if (undying_list.Contains(creature))
                {
                    creature.maxHealth = float.MaxValue;
                    creature.currentHealth = creature.maxHealth;
                }
            }
        }
        public override void OnImbueCollisionStart(CollisionInstance collisionInstance)
        {
            if (collisionInstance.targetCollider != null)
            {
                Creature creature = collider_map[collisionInstance.targetCollider];
                if (undying_list.Contains(creature))
                {
                    if(collisionInstance.damageStruct.penetrationDeepReached)
                    {
                        undying_list.Remove(creature);
                        creature.Kill();
                    }
                }
            }
            base.OnImbueCollisionStart(collisionInstance);
        }
    }
}
