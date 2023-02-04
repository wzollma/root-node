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
		[SerializeField] private GameObject treePrefab;
		[SerializeField] private GameObject treeRootPrefab;

		private Node _node;
		public Node Node => _node;

		private PhysicsRaycaster _physicsRaycaster;
		
		private void Awake()
		{
			_node = GetComponent<Node>();

			_physicsRaycaster = Camera.main.GetComponent<PhysicsRaycaster>();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			
		}

		public void OnEndDrag(PointerEventData eventData)
		{

			Vector3 pointerPosWorld = eventData.pointerCurrentRaycast.worldPosition;

			Vector3 midpoint = (transform.position + pointerPosWorld) / 2;
			

			GameObject newTreeGo = Instantiate(treePrefab, pointerPosWorld, quaternion.identity);
			Tree newTree = newTreeGo.GetComponent<Tree>();

			GameObject newTreeRootGo = Instantiate(treeRootPrefab, midpoint, quaternion.identity);
			TreeRoot newTreeRoot = newTreeRootGo.GetComponent<TreeRoot>();

			_node.AddChild(newTreeRoot.Node);
			newTreeRoot.Node.AddChild(newTree._node);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			
			
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				if(_node.HasParent)
					_node.Parent.Kill();
				else
					_node.Kill();
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			Debug.DrawLine(eventData.pointerCurrentRaycast.worldPosition, transform.position, Color.green);
		}
	}
}