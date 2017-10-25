using UnityEditor;
using UnityEngine;

namespace Assets.AI
{
    public class Threat : MonoBehaviour, IThreaten
    {
        // Use this for initialization
        private void Start()
        {
            
        }

        // Update is called once per frame
        private void Update()
        {

        }

        //todo: display threat radius somehow     
        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, 18.0f);
        //}

        public void DamageObject(GameObject g)
        {
        }
    }
}