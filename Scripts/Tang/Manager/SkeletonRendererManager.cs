using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TaskList
{
    private List<Action> _actions = new List<Action>();
    private ParallelOptions _options = new ParallelOptions();

    public TaskList()
    {
        _options.MaxDegreeOfParallelism = 12;
    }

    public void AddTask(Action task)
    {
        _actions.Add(task);
    }

    public void DoAllTask()
    {
        Parallel.For(0, _actions.Count, _options, (int i) =>
        {
            var task = _actions[i];
            if(task != null)
                task();
        });
        _actions.Clear();
    }
}

public class SkeletonRendererManager : MonoBehaviour
{
//    public static SkeletonRendererManager Instance;
//    
//    public List<SkeletonRenderer> SkeletonRenderers = new List<SkeletonRenderer>();
//    
//    public TaskList resetTask = new TaskList();
//    public TaskList applyTask = new TaskList();
//    
//    public TaskList updateTransformTask = new TaskList();
//    
//    private void Awake()
//    {
//        Instance = this;
//    }
//
//
//    public void Add(SkeletonRenderer skeletonRenderer)
//    {
//        SkeletonRenderers.Add(skeletonRenderer);
//    }
//
//    public void Remove(SkeletonRenderer skeletonRenderer)
//    {
//        SkeletonRenderers.Remove(skeletonRenderer);
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//        for (int i = 0; i < SkeletonRenderers.Count; i++)
//        {
//            SkeletonRenderer skeletonRenderer = SkeletonRenderers[i];
////            skeletonRenderer.OnUpdate();
//        }
//        resetTask.DoAllTask();
//        applyTask.DoAllTask();
//        updateTransformTask.DoAllTask();
//    }
//
//    private void LateUpdate()
//    {
//        for (int i = 0; i < SkeletonRenderers.Count; i++)
//        {
//            SkeletonRenderer skeletonRenderer = SkeletonRenderers[i];
////            skeletonRenderer.OnLateUpdate();
//        }
//    }
}