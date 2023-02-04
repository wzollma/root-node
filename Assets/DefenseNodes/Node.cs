using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefenseNodes
{
	public class Node : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler,
		IUpdateSelectedHandler
	{
		public float ReachDistance = 5;
		[SerializeField] private GameObject nodePrefab;

		public List<Node> Children { get; private set; } = new List<Node>();
		public bool HasParent { get; private set; } = false;
		public Node Parent { get; private set; }

		public void AddChild(Node node)
		{
			if (Children.Contains(node) || Parent == node || this == node)
			{
				return;
			}

			if (node.Parent != null)
			{
				node.Parent.Children.Remove(node);
			}

			Children.Add(node);
			node.Parent = this;
			node.HasParent = true;
		}

		public float Health { get; private set; }

		public void Damage(float amount)
		{
			SetHealth(Health - amount);
		}

		public void SetHealth(float value)
		{
			Health = value;

			if (Health < 0)
			{
				Kill();
			}
		}

		public event Action OnKilled = delegate { };

		public void Kill()
		{
			foreach (Node node in Children)
			{
				node.Kill();
			}

			OnKilled.Invoke();


			Destroy(gameObject);
		}

		private void OnDrawGizmos()
		{
			if (!HasParent)
				return;

			Debug.DrawLine(transform.position, Parent.transform.position, Color.cyan);
		}

		private void OnDestroy()
		{
			if (!HasParent)
				return;

			Parent.Children.Remove(this);
		}

		// Interaction

		private Vector3 _dragPosWorld = Vector3.zero;
		private bool _placementValid;
		
		private void Start()
		{
			gameObject.tag = "Node";
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			eventData.selectedObject = gameObject;
			_placementValid = false;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			eventData.selectedObject = null;
			
			if (!_placementValid)
				return;

			Node newNode = Instantiate(nodePrefab, eventData.pointerCurrentRaycast.worldPosition, Quaternion.identity)
				.GetComponent<Node>();

			AddChild(newNode);
		}

		public void OnDrag(PointerEventData eventData)
		{
			_dragPosWorld = eventData.pointerCurrentRaycast.worldPosition;
			_placementValid = CheckIfValidPlacement(eventData);
		}

		public void OnUpdateSelected(BaseEventData eventData)
		{
			Color c = _placementValid ? Color.green : Color.red;

			Debug.DrawLine(_dragPosWorld, transform.position, c);
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

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				Kill();
			}
		}
	}
}