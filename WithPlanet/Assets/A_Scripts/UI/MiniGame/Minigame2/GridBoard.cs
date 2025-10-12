using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Minigames.ToxicCleanser
{
    public class GridBoard : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup grid;
        [SerializeField] private GridItemView cellPrefab;

        [Header("Grid Settings (고정)")]
        [SerializeField, Range(2, 10)] private int columns = 3;
        [SerializeField, Range(2, 10)] private int rows = 3;

        private readonly List<GridItemView> _cells = new();
        public IReadOnlyList<GridItemView> Cells => _cells;

        /// <summary>
        /// 게시물을 생성한다. 
        /// </summary>
        /// <param name="gridSize">게시물 행렬의 행과 열의 크기</param>
        public void Build(int gridSize)
        {
            columns = gridSize;
            rows = gridSize;

            Clear();
            int total = columns * rows;
            for (int i = 0; i < total; i++)
            {
                var cell = Instantiate(cellPrefab, grid.transform);
                _cells.Add(cell);
            }
        }

        /// <summary>
        /// 모든 게시물을 삭제한다. 
        /// </summary>
        public void Clear()
        {
            for (int i = _cells.Count - 1; i >= 0; i--)
                if (_cells[i] != null)
                {
                    Destroy(_cells[i].gameObject);
                }
            _cells.Clear();
        }
    }
}
