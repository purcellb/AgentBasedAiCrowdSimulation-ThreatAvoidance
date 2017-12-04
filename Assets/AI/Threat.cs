using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.AI
{
    public class Threat : MonoBehaviour
    {
        private bool _isFiring;
        private bool _isReloading;
        private Person[] _people;
        private float _timeSinceLastShot;
        public int AmmoCount;
        public GameObject Bullet;
        public GameObject BulletEmitter;
        public float EffectiveRange = 70.0f;
        public int MagazineSize = 15;
        public float MuzzleVelocity = 40000.0f;
        public float FiringCone = 1.0f;
        public Person Target;
        public LayerMask TargetMask;
        public float TimeBetweenShots = .33f;
        public float TurnRateRadians = .75f * Mathf.PI;
        public float VaryBy = 0.0f;
        public float ViewAngle = 110;
        public List<Person> VisibleTargets = new List<Person>();

        // Use this for initialization
        private void Start()
        {
            AmmoCount = MagazineSize;
            Invoke("EnableObstacle", 5f);
            StartCoroutine("DoProximityCheck");
        }

        // Update is called once per frame
        private void Update()
        {
            Aim();
            Fire();

            //walk toward target at randomized rate
            if (Vector3.Distance(transform.position, Target.transform.position) > 10)
            {
                var r = Random.Range(0.7f, 1.5f);
                transform.position += transform.forward * r * Time.deltaTime;
            }
        }

        private void Aim()
        {
            //if theres no target detected or were reloading, stop
            if (_isReloading || Target == null || Target.CurrentHealth <= 0)
            {
                _isFiring = false;
                return;
            }

            var targetDir = Target.transform.position - transform.position;
            targetDir.y = 0.0f;
            targetDir = targetDir.normalized;

            var aim = transform.forward;

            //rotate aim towards target
            aim = Vector3.RotateTowards(aim, targetDir, TurnRateRadians * Time.deltaTime, 1.0f);

            transform.forward = aim;

            _isFiring = true;
        }

        private void Reload()
        {
            _isReloading = true;
            Invoke("FinishReloading", 3.0f);
        }

        private void FinishReloading()
        {
            AmmoCount += MagazineSize;
            _isReloading = false;
        }

        private void Fire()
        {
            if (_timeSinceLastShot > 0)
                _timeSinceLastShot -= Time.deltaTime;

            //can i fire?
            if (!_isFiring || _isReloading) return;
            if (!(_timeSinceLastShot <= 0)) return;

            //reset counter, maintains RoF with variance
            var shotVariance = Random.Range(-VaryBy, VaryBy);
            _timeSinceLastShot = TimeBetweenShots + shotVariance;

            if (AmmoCount < 1)
            {
                Reload();
                return;
            }
            
            

        float xSpread = Random.Range(-1, 1);
        float ySpread = Random.Range(-1, 1);
        //normalize the spread vector to keep it conical
        Vector3 spread = new Vector3(xSpread, ySpread, 0.0f).normalized * FiringCone;
        Quaternion rotation = Quaternion.Euler(spread) * BulletEmitter.transform.rotation;
        var bulletHandler = Instantiate(Bullet, BulletEmitter.transform.position, rotation);

        var bulletRigidbody = bulletHandler.GetComponent<Rigidbody>();

            //"fires" the bullet
            bulletRigidbody.AddForce(transform.forward * MuzzleVelocity);
            AmmoCount -= 1;
            //make sure bullets aren't laying around. They should destroy on impact, this is just a precauiton.
            Destroy(bulletHandler, 5.0f);
        }

        //proximity check for threats, every half of a second
        private IEnumerator DoProximityCheck()
        {
            for (;;)
            {
                var r = Random.Range(1, 6);
                _people = FindObjectsOfType<Person>();
                //target is a nearby living person that is in range and isn't me... TODO: Morbid thought, shoot self when no more targets?
                var viableTargets = _people.Where(t =>
                    t != gameObject.GetComponent<Person>() && t.IsAlive &&
                    Vector3.Distance(transform.position, t.transform.position) < EffectiveRange);

                FindVisibleTargets(viableTargets);

                Target = (Person) VisibleTargets.Where(t => t.IsAlive)
                    .OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).ToArray().GetValue(r);

                yield return new WaitForSeconds(.5f);
            }
        }

        // I've heavily modified it, but still. Credit for the original bit of code for this function goes to Sebastian Lague https://github.com/SebLague
        private void FindVisibleTargets(IEnumerable<Person> viableTargets)
        {
            VisibleTargets.Clear();

            foreach (var p in viableTargets)
            {
                var target = p.transform;
                var dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < ViewAngle / 2)
                {
                    var dstToTarget = Vector3.Distance(transform.position, target.position);

                    if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, TargetMask))
                        VisibleTargets.Add(p);
                }
            }
        }

        //Credit for this function goes to Sebastian Lague https://github.com/SebLague
        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
                angleInDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private void EnableObstacle()
        {
            gameObject.GetComponent<NavMeshObstacle>().enabled = true;
        }

        private void OnDrawGizmosSelected()
        {
            //displays general threat radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 25.0f);
            //display effective weapon range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, EffectiveRange);
        }
    }
}