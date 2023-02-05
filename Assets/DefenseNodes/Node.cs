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
		public event Action OnDestroyed = delegate {  };

		public event Action<float> OnHealthChange = delegate {  };
		
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


		[SerializeField] private float health;
		public float Health => health;

		public void Damage(float amount)
		{
			SetHealth(health - amount);
		}

		public void SetHealth(float value)
		{
			health = value;
			
			OnHealthChange.Invoke(health);

			if (health < 0)
			{
				Die();
			}
		}

		public void Die()
		{
			Destroy(gameObject);
		}


		private void OnDestroy()
		{
			foreach (Node node in Children)
			{
				node.HasParent = false;
				node.Die();
			}
			
			if (HasParent)
				Parent.Children.Remove(this);

			OnDestroyed.Invoke();
		}

		// Interaction

		public float ReachDistance = 8;

		private Vector3 _dragPosWorld = Vector3.zero;

		private bool _placementValid;

		public void OnBeginDrag(PointerEventData eventData)
		{
			CameraRef.Raycaster.eventMask &= ~(1 << LayerMask.NameToLayer("tree"));
			eventData.selectedObject = gameObject;
			_placementValid = false;
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
			newNode.SpawnRootModel();
		}

		[SerializeField] private GameObject rootPrefab;
		private void SpawnRootModel()
		{
			Transform rootTransform = Instantiate(rootPrefab, transform).transform;
			rootTransform.localScale =
				new Vector3(1, 1, Vector3.Distance(transform.position, Parent.transform.position));
			rootTransform.rotation =
				Quaternion.LookRotation(Parent.transform.position - transform.position, Vector3.up);

		}

		private bool CheckIfValidPlacement(PointerEventData eventData)
		{
			Vector3 pointerWorld = eventData.pointerCurrentRaycast.worldPosition;
			pointerWorld.y = 0;

			if (!NodeSpawner.Singleton.CheckIfEnoughMoneyForSelected())
				return false;
			
			if (eventData.hovered.Count < 1 || eventData.hovered[0].layer != LayerMask.NameToLayer("ground"))
				return false;

			if (Vector3.Distance(transform.position, pointerWorld) > ReachDistance)
				return false;


			return NavManager.instance.isPositionValid(pointerWorld, 1f, 100);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				Die();
			}
		}

		// debug


		private void OnDrawGizmos()
		{
			if (!HasParent)
				return;

			Debug.DrawLine(transform.position, Parent.transform.position, Color.cyan);
		}
	}
}