using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.AI
{
    public class PersonSeekExit : Person
    {
        private GameObject[] _exits;
        private GameObject _nearestExit;
        private Threat[] _threats;

        protected override void Start()
        {
            base.Start();
            _threats = FindObjectsOfType<Threat>();
            _exits = GameObject.FindGameObjectsWithTag("Exit");
            InvokeRepeating("SetDestinationNearestExit", ReactionTime, Agility);
        }

        [UsedImplicitly]
        private void SetDestinationNearestExit()
        {
            if (!IsAlive)
            {
                CancelInvoke();
                return;
            }
            var currentShortest = Mathf.Infinity;
            var actorPosition = transform.position;

            foreach (var e in _exits)
            {
                //if this exit is unsafe, skip it
                if (_threats.Any(t =>
                    Vector3.Distance(t.transform.position, e.transform.position) < t.EffectiveRange)) continue;

                var exitPosition = e.transform.position;

                var currentExitDistance = Vector3.Distance(actorPosition, exitPosition);

                if (currentExitDistance > currentShortest) continue;

                _nearestExit = e;
                currentShortest = currentExitDistance;
                if (!IsAlive) CancelInvoke();
            }
            try
            {
                if (_nearestExit == null) throw new NullReferenceException();
                Agent.destination = _nearestExit.transform.position;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("SetDestinationNearestExit failed to find an exit\n" + e);
            }
        }

        private void Leave(Object exit)
        {
            Debug.Log(gameObject.name + " escaped through exit " + exit.name);
            Destroy(gameObject);
        }


        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            if (!IsAlive)
            {
                return;
            }
            if (_nearestExit != null &&
                Vector3.Distance(Agent.transform.position, _nearestExit.transform.position) < 2.0f)
                Leave(_nearestExit);
            
        }
    }
}