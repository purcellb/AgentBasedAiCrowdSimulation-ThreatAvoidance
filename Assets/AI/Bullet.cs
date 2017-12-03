using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.AI
{
    public class Bullet : MonoBehaviour
    {
        public float BulletDamage;

        private void OnCollisionEnter(Collision collision)
        {
            var thingHit = collision.gameObject;
            IEnumerable<Person> personParts = thingHit.GetComponents<Person>();

            if (personParts == null) return;
            personParts = personParts.Where(t => t.IsAlive);
            foreach (var part in personParts)
            {
                part.WoundCount += 1;
                part.CurrentHealth -= BulletDamage;
            }

            DestroyObject(gameObject);
        }
    }
}