using UnityEngine;
using UnityEngine.AI;

namespace Assets.AI
{
    public class RandomMovement : MonoBehaviour
    {
        private int _directionValue = 1;
        private float _turnValue;
        public Collider Collider;
        public float SensorLength = .5f;
        public float Speed = 10.0f;
        public float TurnSpeed = 50.0f;

        // Use this for initialization
        private void Start()
        {
            Collider = transform.GetComponent<Collider>();
        }

        // Update is called once per frame
        private void Update()
        {
            RaycastHit hit;
            var hitFlag = 0;
            var forwardBlocked = false;
            var backwardBlocked = false;

            //right
            if (Physics.Raycast(transform.position, transform.right, out hit, SensorLength * 1.3f))
            {
                if (hit.collider.tag == "Exit")
                    Destroy(gameObject);
                if (hit.collider.tag != "Obstacle" || hit.collider == Collider) return;
                _turnValue -= 1;
                hitFlag++;
            }

            //left
            if (Physics.Raycast(transform.position, -transform.right, out hit, SensorLength * 1.3f))
            {
                if (hit.collider.tag == "Exit")
                    Destroy(gameObject);
                if (hit.collider.tag != "Obstacle" || hit.collider == Collider) return;
                _turnValue += 1;
                hitFlag++;
            }

            //front
            if (Physics.Raycast(transform.position, transform.forward, out hit, SensorLength * 1.6f))
            {
                if (hit.collider.tag == "Exit")
                    Destroy(gameObject);
                if (hit.collider.tag != "Obstacle" || hit.collider == Collider) return;
                if (_directionValue >= 0) _directionValue = -1;
                hitFlag++;
                forwardBlocked = true;
            }

            //back
            if (Physics.Raycast(transform.position, -transform.forward, out hit, SensorLength))
            {
                if (hit.collider.tag == "Exit")
                    Destroy(gameObject);
                if (hit.collider.tag != "Obstacle" || hit.collider == Collider) return;
                if (_directionValue <= 0) _directionValue = 1;
                hitFlag++;
                backwardBlocked = true;
            }
            if (forwardBlocked && backwardBlocked)
                _directionValue = 0;

            if (hitFlag == 0)
                _turnValue = 0;

            transform.Rotate(Vector3.up * (TurnSpeed * _turnValue) * Time.deltaTime);

            transform.position += transform.forward * Speed * _directionValue * Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, transform.forward * SensorLength);
            Gizmos.DrawRay(transform.position, -transform.forward * SensorLength);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.right * (SensorLength * 1.3f));
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, -transform.right * (SensorLength * 1.3f));
        }
    }
}