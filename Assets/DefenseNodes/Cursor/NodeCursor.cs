using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace DefenseNodes.Cursor
{
    public class NodeCursor : MonoBehaviour
    {
        public static NodeCursor Singleton { get; private set; }

        private DecalProjector _decalProjector;
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");

        private void Awake()
        {
            _decalProjector = GetComponent<DecalProjector>();
            
            Singleton = this;
            SetColor(Color.red);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void SetColor(Color color)
        {
            _decalProjector.material.SetColor(ColorProperty, color);
        }
    }
}