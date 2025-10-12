using System.Collections.Generic;
using UnityEngine;

namespace Project.Minigames.ToxicCleanser
{
    public class CommentPool : MonoBehaviour
    {
        [SerializeField] private CommentItem prefab;
        [SerializeField] private RectTransform parent;
        [SerializeField] private int prewarm = 16;

        private readonly Queue<CommentItem> pool = new();

        private void Awake()
        {
            for (int i = 0; i < prewarm; i++)
                pool.Enqueue(Create());
        }

        private CommentItem Create()
        {
            var go = Instantiate(prefab, parent);
            go.gameObject.SetActive(false);
            return go;
        }

        public CommentItem Get()
        {
            var item = pool.Count > 0 ? pool.Dequeue() : Create();
            item.gameObject.SetActive(true);
            return item;
        }

        public void Release(CommentItem item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(parent);
            pool.Enqueue(item);
        }

        public CommentItem PeekPrefab()
        {
            return prefab; // 풀에서 사용하는 원본 프리팹 반환
        }
    }
}