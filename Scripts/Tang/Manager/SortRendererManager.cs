




using System.Threading;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace Tang
{
    public class SortRendererManager
    {
        static private SortRendererManager _sortRendererManager;
        public static SortRendererManager Instance
        {
            get
            {
                if (_sortRendererManager == null)
                {
                    _sortRendererManager = new SortRendererManager();
                }
                return _sortRendererManager;
            }
        }

        ThreadWork threadWork = new ThreadWork();

        private List<SortRenderer> sortRendererList = new List<SortRenderer>();
        private List<SortRenderer> tmpSortRendererList = new List<SortRenderer>();

        IEnumerator enumeratorSort;

        bool apply = false;

        Thread thread;

        public void AddSortRenderer(SortRenderer sortRenderer)
        {
            RemoveSortRenderer(sortRenderer);

            sortRendererList.Add(sortRenderer);
        }

        public void RemoveSortRenderer(SortRenderer sortRenderer)
        {
            sortRendererList.Remove(sortRenderer);
        }

        public void Update()
        {
            
        }

        public void Sort()
        {
            for (int i = sortRendererList.Count - 1; i >= 0; i--)
            {
                SortRenderer sortRenderer = sortRendererList[i];
                try
                {
                    if (sortRenderer)
                    {
                    }
                    else
                    {
                        sortRendererList.RemoveAt(i);
                    }
                }
                catch
                {
                    sortRendererList.RemoveAt(i);
                }

            }

            tmpSortRendererList.Clear();
            tmpSortRendererList.AddRange(sortRendererList);

            tmpSortRendererList.Sort((SortRenderer a, SortRenderer b) =>
            {

                if (a.PosZ < b.PosZ) // 在前面的条件
                {
                    return 1;
                }
                else if (a.PosZ > b.PosZ)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
        }

        public void Apply()
        {
            for (int i = 0; i < tmpSortRendererList.Count; i++)
            {
                SortRenderer sortRenderer = tmpSortRendererList[i];
                if (sortRenderer)
                {
                    int currZOrder = (int)ZOrder.ObjectMin + i;
                    sortRenderer.SetZorder(currZOrder);
                }
            }
        }

        public void UpdateState()
        {
            if (Application.isPlaying == false)
            {
                foreach (var sortRenderer in sortRendererList)
                {
                    if (sortRenderer)
                    {
                        sortRenderer.UpdateState();
                    }
                }
            }
        }
    }
}