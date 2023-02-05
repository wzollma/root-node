using System;
using System.Collections.Generic;
using DefenseNodes.Towers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace DefenseNodes
{
	public class Node : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler,
		IUpdateSelectedHandler
	{
		public float ReachDistance = 5;

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

		// Interaction

		private Vector3 _dragPosWorld = Vector3.zero;
		private bool _placementValid;

		private void Start()
		{
			gameObject.tag = "Node";
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			CameraRef.Raycaster.eventMask &= ~(1 << LayerMask.NameToLayer("tree"));
			eventData.selectedObject = gameObject;
			_placementValid = false;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			CameraRef.Raycaster.eventMask |= 1 << LayerMask.NameToLayer("tree");
			
			eventData.selectedObject = null;

			if (!_placementValid)
				return;


			if (!NodeSpawner.Singleton.TrySpawnNode(eventData.pointerCurrentRaycast.worldPosition,
				    quaternion.identity, out Node newNode))
			{
				return;
			}
			
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
			Vector3 pointerWorld = eventData.pointerCurrentRaycast.worldPosition;
			pointerWorld.y = 0;

			if (!NodeSpawner.Singleton.CheckIfEnoughMoneyForSelected())
				return false;
			
			if (eventData.hovered.Count < 1 || !eventData.hovered[0].CompareTag("Ground"))
				return false;

			if (Vector3.Distance(transform.position, pointerWorld) > ReachDistance)
				return false;


			// float pathDist = NavManager.instance.getShortestDistanceToPath(pointerWorld);
			// Debug.Log(pathDist);
			// if (pathDist < 0.5f)
			// 	return false;

			return true;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				Kill();
			}
		}
		
		public Transform GetTowerContainer()
		{
			return transform.GetChild(0);
		}
		
		// debug
		
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
	}
}