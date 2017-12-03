using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.AI
{
    public class Person : MonoBehaviour
    {
        public NavMeshAgent Agent;
        public float Agility;
        public float CurrentHealth;
        public float DangerDistance;
        public bool IsAlive = true;
        public float MaximumHealth;
        public int MentalBreakThreshold;
        public NavMeshObstacle Obstacle;
        public float ReactionTime;
        public float SensorLength = .5f;
        public float SpeedModifier;
        public int WoundCount = 0;
        public int WoundHandicap;

        protected virtual void Start()
        {
            MaximumHealth = 100.0f;
            CurrentHealth = MaximumHealth;
            Agent = GetComponent<NavMeshAgent>();
            Obstacle = GetComponent<NavMeshObstacle>();
            SetupRandomizedAgent();
        }

        private void SetupRandomizedAgent()
        {
            //gives variety to the crowd
            SpeedModifier = Random.Range(.5f, 1.2f);
            ReactionTime = Random.Range(0.6f, 1.75f);
            Agility = Random.Range(2.0f, 3.0f);
            MentalBreakThreshold = Random.Range(3, 12); 
            DangerDistance = 25.0f / SpeedModifier; //if im slower, I percieve danger farther away
            Agent.acceleration *= SpeedModifier;
            Agent.speed *= SpeedModifier;
            Agent.angularSpeed *= SpeedModifier;
        }

        [UsedImplicitly]
        private void Update()
        {
            //todo: in this and other places i could have an interface that logs whenever a person takes damage 
            //todo: this would allow a postmortem
            //Gunshot wounds slow people down
            if (CurrentHealth > 0.0f && WoundCount > WoundHandicap)
            {
                WoundHandicap += 1;
                var mat = gameObject.GetComponent<MeshRenderer>().material;

                mat.color = Color.Lerp(mat.color, Color.red, (MaximumHealth - CurrentHealth) * .01f);
                Agent.speed *= WoundHandicap * .5f;
                Agent.angularSpeed *= WoundHandicap * .5f;
                Agent.acceleration *= WoundHandicap * .5f;
            }

            //randomly bleeding if badly wounded from gunshots
            var r = Random.Range(1, 100);
            if (CurrentHealth < 33.3f && WoundCount >= 2 && r > 25)
                CurrentHealth -= .25f;

            if (CurrentHealth < 0.0f) Die();
        }

        private void Die()
        {
            //todo: record death on ui
            //turns off all behavior scripts and cancels invoke rather than deleting the object
            //Dead people stop moving and thinking, they dont disappear

            IEnumerable<Person> personParts = gameObject.GetComponents<Person>();
            
            //tell all person components that this person is dead.
            if (personParts == null) return;
            personParts = personParts.Where(t => t.IsAlive);
            foreach (var part in personParts)
            {
                part.IsAlive = false;
                part.CancelInvoke();
                part.Agent.enabled = false;
                part.enabled = false;
            }
            Console.WriteLine(gameObject.name + " died.");
            IsAlive = false;
            var mat = gameObject.GetComponent<MeshRenderer>().material;

            mat.color = Color.red;
            Obstacle.enabled = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            //for simulating being trampled by other people
            var thingHit = collision.gameObject;
            var thingType = thingHit.GetComponent<Person>();

            if (!IsAlive || thingType == null) return;
            IEnumerable<Person> personParts = gameObject.GetComponents<Person>();
            var r = Random.Range(1, 100);
            if (personParts == null || r < 50) return;
            personParts = personParts.Where(t => t.IsAlive);
            foreach (var part in personParts)
                part.CurrentHealth -= collision.relativeVelocity.magnitude * .04f;
        }

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            //Gizmos.DrawRay(transform.position, transform.forward * SensorLength);
            //Gizmos.color = Color.blue;
            //Gizmos.DrawRay(transform.position, transform.right * Range);
            //Gizmos.color = Color.green;
            //Gizmos.DrawRay(transform.position, -transform.right * Range);
        }
    }
}