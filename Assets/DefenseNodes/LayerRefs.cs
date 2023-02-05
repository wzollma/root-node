using UnityEngine;

namespace DefenseNodes
{
    public class LayerRefs : MonoBehaviour
    {
        public static int TowerBody { get; private set; }
        public static int TowerBodyMask { get; private set; }
        public static int Ground { get; private set; }

        private void Awake()
        {
            TowerBody = LayerMask.NameToLayer("tower body");
            TowerBodyMask = LayerMask.GetMask("tower body");
            Ground = LayerMask.NameToLayer("ground");
        }
    }
}