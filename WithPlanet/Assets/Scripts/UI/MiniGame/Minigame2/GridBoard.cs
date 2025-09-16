using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    public class GridBoard : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup grid;
        [SerializeField] private GridItemView cellPrefab;

        private readonly List<GridItemView> _cells = new();
        public IReadOnlyList<GridItemView> Cells => _cells;

        public void Build(int gridSize)
        {
            Clear();
            int total = gridSize * gridSize;
            for (int i = 0; i < total; i++)
            {
                var cell = Instantiate(cellPrefab, grid.transform);
                _cells.Add(cell);
            }
        }

        public void Clear()
        {
            for (int i = _cells.Count - 1; i >= 0; i--)
                if (_cells[i] != null) Destroy(_cells[i].gameObject);
            _cells.Clear();
        }
    }
}
