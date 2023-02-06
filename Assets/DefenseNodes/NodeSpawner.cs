using System;
using DefenseNodes.Towers;
using UnityEngine;

namespace DefenseNodes
{

	public class NodeSpawner : MonoBehaviour
	{
		public int money = 10;
		
		public int SelectedTowerIndex { get; private set; }

		public int SetSelectedTowerIndex(int index)
		{
			return SelectedTowerIndex = Math.Clamp(index, 0, towerPrefabs.Length);
		}
		
		[SerializeField] private GameObject nodePrefab;

		[SerializeField] private TowerBase[] towerPrefabs;
		
		public static NodeSpawner Singleton { get; private set; }
		
		// Start is called before the first frame update
		void Awake()
		{
			Singleton = this;
		}

		public bool CheckIfEnoughMoneyForSelected()
		{
			return towerPrefabs[SelectedTowerIndex].Cost <= money;
		}

		public bool TrySpawnNode(Vector3 position, Quaternion rotation, out Node node)
		{
			node = null;
			if (!CheckIfEnoughMoneyForSelected())
            {
				AudioManager.instance.Play("not_enough_money");
				return false;
			}				

			money -= towerPrefabs[SelectedTowerIndex].Cost;
			
			node = Instantiate(nodePrefab, position, rotation).GetComponent<Node>();
			TowerBase tower = SpawnTower(node.transform);
			node.SetHealth(tower.InitialHealth);
			node.OnHealthChange += tower.OnNodeTakesDamage;
			node.thisTree = tower;

			node.GetComponent<HoverHighlight>().MeshRenderer = tower.meshFilter.GetComponent<MeshRenderer>();

			return true;
		}

		public void addMoney(int value)
        {
			money += value;
        }

		private TowerBase SpawnTower(Transform parent)
		{
			return Instantiate(towerPrefabs[SelectedTowerIndex].gameObject, parent).GetComponent<TowerBase>();
		}

		private void OnGUI()
		{
			Rect r = new Rect(50, 50, 150, Screen.height - 100);
			GUI.Box(r, "");
			GUILayout.BeginArea(r);
			GUILayout.Label("Seeds: " + money);
			GUILayout.Label("Tree health: " + TreeBase.instance.health);
			for (int i = 0; i < towerPrefabs.Length; i++)
			{
				TowerBase tower = towerPrefabs[i];
				if (GUILayout.Button(tower.TowerName + "\nCost: " + tower.Cost + "\nHealth: " + tower.InitialHealth))
				{
					SetSelectedTowerIndex(i);
				}
			}
			GUILayout.EndArea();
		}
	}
}