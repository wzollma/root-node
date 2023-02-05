using DefenseNodes.Towers;
using UnityEngine;

namespace DefenseNodes
{

	public class NodeSpawner : MonoBehaviour
	{

		[SerializeField] private GameObject nodePrefab;

		[SerializeField] private GameObject[] treePrefabs;
		
		public static NodeSpawner Singleton { get; private set; }
		
		// Start is called before the first frame update
		void Awake()
		{
			Singleton = this;
		}

		public Node SpawnNode(Vector3 position, Quaternion rotation)
		{
			return Instantiate(nodePrefab, position, rotation).GetComponent<Node>();
		}

		public TowerBase SpawnTower(Transform parent, int treeIndex)
		{
			return Instantiate(treePrefabs[treeIndex], parent).GetComponent<Towers.TowerBase>();
		}
	}
}