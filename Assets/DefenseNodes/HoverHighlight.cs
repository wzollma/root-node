using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefenseNodes
{
	public class HoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public MeshRenderer MeshRenderer;
		[SerializeField] private Material normal;
		[SerializeField] private Material highlight;

		private void Awake()
		{
			MeshRenderer = GetComponentInChildren<MeshRenderer>();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			MeshRenderer.material = highlight;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			MeshRenderer.material = normal;
		}
	}
}