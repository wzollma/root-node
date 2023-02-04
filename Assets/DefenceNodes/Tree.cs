using System;
using System.Security.Cryptography;
using DefenceNodes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace DefenceNodes
{
	[RequireComponent(typeof(Node))]
	public class Tree : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
	{
		public float ReachDistance = 8;
		[SerializeField] private GameObject treePrefab;

		private Node _node;
		public Node Node => _node;

		private void Awake()
		{
			_node = GetComponent<Node>();
		}

		private void Start()
		{
			gameObject.tag = "Tree";
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			
		}

		public void OnEndDrag(PointerEventData eventData)
		{

			Vector3 pointerPos = eventData.pointerCurrentRaycast.worldPosition;

			if (!CheckIfValidPlacement(eventData))
				return;

			GameObject newTreeGo = Instantiate(treePrefab, pointerPos, quaternion.identity);
			Tree newTree = newTreeGo.GetComponent<Tree>();

			_node.AddChild(newTree.Node);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				_node.Kill();
			}
		}

		public void OnDrag(PointerEventData eventData)
		{

			Color c = CheckIfValidPlacement(eventData) ? Color.green : Color.red;
				
			Debug.DrawLine(eventData.pointerCurrentRaycast.worldPosition, transform.position, c);
		}

		private bool CheckIfValidPlacement(PointerEventData eventData)
		{
			if (!eventData.hovered[0].CompareTag("Ground"))
				return false;
			
			Vector2 transformXZ = new Vector2(transform.position.x, transform.position.z);
			Vector3 wp = eventData.pointerCurrentRaycast.worldPosition;
			Vector2 pointXZ = new Vector2(wp.x, wp.z);

			return Vector2.Distance(transformXZ, pointXZ) < ReachDistance;
		}
	}
}