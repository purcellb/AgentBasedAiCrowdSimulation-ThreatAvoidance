using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.AI
{
    public class Person : MonoBehaviour
    {
        public NavMeshAgent Agent;
        public bool IsAlive = true; 
        public float Agility;
        public float CurrentHealth;
        public float DangerDistance;
        public float MaximumHealth;
        public float ReactionTime;
        public float SensorLength = 1.1f;
        public float SpeedModifier;

        protected virtual void Start()
        {
            MaximumHealth = 100.0f;
            CurrentHealth = MaximumHealth;
            Agent = GetComponent<NavMeshAgent>();
            SetupRandomizedAgent();
        }

        private void SetupRandomizedAgent()
        {
            //gives some flavor to the agents, some faster some slower
            //todo: normal distributions?
            SpeedModifier = Random.Range(.4f, 1.2f);
            ReactionTime = Random.Range(0.6f, 2.0f);
            Agility = Random.Range(1.4f, 2.0f);
            DangerDistance = 15.0f / SpeedModifier; //if im slower, I percieve danger farther away
            Agent.acceleration *= SpeedModifier;
            Agent.speed *= SpeedModifier;
            Agent.angularSpeed *= SpeedModifier;
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            //todo: some sort of stuck detection? perhaps every X seconds check if im in roughly the same spot as last check

            if (CurrentHealth < 0.0f) Die();
        }

        private void Die()
        {
            //todo: record death on ui
            //turns off all behavior scripts and cancels invoke rather than deleting the object
            //Dead people stop moving and thinking, they dont disappear
            IsAlive = false;
            var behaviors = gameObject.GetComponents<Behaviour>();
            CancelInvoke();
            foreach (var behavior in behaviors)
                behavior.enabled = false;
            Debug.Log(gameObject.name + " died.");
        }

        void OnCollisionEnter(Collision collision)
        {
            var thingHit = collision.gameObject;
            var thingType = thingHit.GetComponent<Person>();

            if (IsAlive && thingType !=null)
            CurrentHealth -= collision.relativeVelocity.magnitude * .15f;
        }

        [UsedImplicitly]
        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, transform.forward * SensorLength);
            //Gizmos.color = Color.blue;
            //Gizmos.DrawRay(transform.position, transform.right * SensorLength);
            //Gizmos.color = Color.green;
            //Gizmos.DrawRay(transform.position, -transform.right * SensorLength);
        }
    }
}