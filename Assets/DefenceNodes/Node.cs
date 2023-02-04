using System;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

namespace DefenceNodes
{
	public class Node : MonoBehaviour
	{
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
	}
}