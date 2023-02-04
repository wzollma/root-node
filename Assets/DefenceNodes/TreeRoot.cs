using System;
using UnityEngine;

namespace DefenceNodes
{
	[RequireComponent(typeof(Node))]
	public class TreeRoot : MonoBehaviour
	{
		private Node _node;
		public Node Node => _node;
		
		private void Awake()
		{
			_node = GetComponent<Node>();
		}
	}
}