using UnityEditor;
using UnityEngine;

namespace Assets.AI
{
    public class ThreatBehavior : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 18.0f);
        }
    }
}