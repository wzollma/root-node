using UnityEngine;

namespace DefenseNodes
{
    public class NodePlacer : MonoBehaviour
    {
        private int _numCollision;
        
        private void OnCollisionEnter(Collision collision)
        {
            _numCollision++;
        }

        private void OnCollisionExit(Collision other)
        {
            _numCollision--;
        }
        
        
    }
}
