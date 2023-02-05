using System;
using System.Collections.Generic;
using DefenseNodes.Cursor;
using DefenseNodes.Towers;
using Unity.Mathematics;
using UnityEditor.UIElements;
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

		private CapsuleCollider _capsuleCollider;

		private static LayerMask _treeLayer;
		private static LayerMask _groundLayer;

		private void Awake()
		{
			_capsuleCollider = GetComponent<CapsuleCollider>();
		}

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

			AudioManager.PlayNoOverlap("plant");
		}


		[SerializeField] private float health;
		public float Health => health;

		public void Damage(float amount)
		{
			SetHealth(health - amount);
			
			

			AudioManager.PlayNoOverlap("tree_hurt");
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
			AudioManager.PlayNoOverlap("tree_hurt");

			Destroy(gameObject);
		}


		private void OnDestroy()
		{
			foreach (Node node in Children)
			{
				node.HasParent = false;
				node.Die();
			}

			SetBeingDragged(false, _draggingEventData);
			
			if (HasParent)
				Parent.Children.Remove(this);

			if (_beingDragged)
				NodeCursor.Singleton.gameObject.SetActive(false);

			OnDestroyed.Invoke();
		}

		// Interaction

		public float ReachDistance = 8;

		private Vector3 _dragPosWorld = Vector3.zero;

		private bool _placementValid;

		private bool _beingDragged;
		private PointerEventData _draggingEventData;

		private void SetBeingDragged(bool beingDragged, PointerEventData eventData)
		{
			_draggingEventData = eventData;
			eventData.dragging = beingDragged;
			NodeCursor.Singleton.gameObject.SetActive(beingDragged);

			if (!_beingDragged && beingDragged)
			{
				CameraRef.Raycaster.eventMask &= ~(1 << LayerRefs.TowerBody);
				eventData.pointerDrag = gameObject;
				eventData.selectedObject = gameObject;
			}
			else if(_beingDragged && !beingDragged)
			{
				CameraRef.Raycaster.eventMask |= 1 << LayerRefs.TowerBody;
				eventData.pointerDrag = null;
				eventData.selectedObject = null;
			}
			
			_beingDragged = beingDragged;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			SetBeingDragged(true, eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			_dragPosWorld = eventData.pointerCurrentRaycast.worldPosition;
			NodeCursor.Singleton.transform.position = _dragPosWorld + Vector3.up * 2;
			_placementValid = CheckIfValidPlacement(eventData);
		}

		public void OnUpdateSelected(BaseEventData eventData)
		{
			Color c = _placementValid ? Color.green : Color.red;

			NodeCursor.Singleton.SetColor(c);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			SetBeingDragged(false, eventData);

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
			
			if (Vector3.Distance(transform.position, pointerWorld) > ReachDistance)
				return false;
			
			if (eventData.hovered.Count < 1 || eventData.hovered[0].layer != LayerRefs.Ground)
				return false;

			float distFromCenter = Vector3.Distance(pointerWorld, Vector3.zero);
			if (distFromCenter < 7 || distFromCenter > 22)
				return false;
			
			if (Physics.CheckSphere(pointerWorld, 1, LayerRefs.TowerBodyMask))
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