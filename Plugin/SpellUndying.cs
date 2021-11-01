using System;
using System.Collections.Generic;

using ThunderRoad;
using UnityEngine;

namespace SpellUndying
{
    public class SpellUndying : SpellCastLightning
    {

        Dictionary<Collider, Creature> collider_map = new Dictionary<Collider, Creature>();
        Dictionary<Creature, RagdollPart> torso_rpart_map = new Dictionary<Creature, RagdollPart>();
        LinkedList<Creature> undying_list = new LinkedList<Creature>();

        SpellUndying()
        {
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            foreach (RagdollPart rp in creature.ragdoll.parts)
            {
                foreach (Collider c in rp.colliderGroup.colliders)
                {
                    collider_map[c] = creature;
                }

                if (rp.data.partTypes.Contains(RagdollPart.Type.Torso))
                {
                    torso_rpart_map[creature] = rp;
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
                    creature_to_max_health(creature);
                }
            }
        }

        public override void OnImbueCollisionStart(CollisionInstance collisionInstance)
        {
            base.OnImbueCollisionStart(collisionInstance);
            Creature creature = collider_map[collisionInstance.targetCollider];

            if (undying_list.Contains(creature))
            {
                if (check_torso_stab(creature, collisionInstance))
                {
                    kill_creature(creature);
                }
            }
        }


        private void Creature_OnDamageEvent(CollisionInstance collisionInstance)
        {
            if (collisionInstance.targetCollider != null)
            {
                if (collider_map.ContainsKey(collisionInstance.targetCollider))
                {
                    Creature creature = collider_map[collisionInstance.targetCollider];

                    if (undying_list.Contains(creature))
                    {
                        creature_to_max_health(creature);
                    }
                }
            }
        }

        private bool check_torso_stab(Creature c, CollisionInstance ci)
        {
            DamageStruct ds = ci.damageStruct;
            
            return (ds.hitRagdollPart == torso_rpart_map[c]);
        }

        private void kill_creature(Creature creature)
        {
            undying_list.Remove(creature);

            foreach (RagdollPart rp in creature.ragdoll.parts)
            {
                foreach (Collider c in rp.colliderGroup.colliders)
                {
                    collider_map.Remove(c);
                }
            }

            creature.Kill();
        }

        private void creature_to_max_health(Creature c)
        {
            c.maxHealth = float.MaxValue;
            c.currentHealth = c.maxHealth;
        }
    }
}
