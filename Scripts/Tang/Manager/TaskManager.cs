using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class TaskManager : MonoBehaviour
    {
        public class Task
        {
            public Task(string id, Action action)
            {
                this.id = id;
                this.action = action;
            }

            public string id;
            public Action action;

            public void Execute()
            {
                if (action != null)
                    action();
            }
        }

        private static TaskManager instance;
        public static TaskManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<TaskManager>();
                }
                return instance;
            }
        }

        private List<Task> tasks;

        private void Awake()
        {
            tasks = new List<Task>();
        }
        
        public void AddTask(string id, Action action)
        {
            RemoveTask(id);
            tasks.Add(new Task(id, action));
        }

        public void RemoveTask(string id)
        {
            int index = FindTaskIndex(id);
            if (index>=0)
            {
                tasks.RemoveAt(index); 
            }
        }

        public Task FindTask(string id)
        {
            return tasks.Find((Task t) => { return t.id == id; });
        }

        public int FindTaskIndex(string id)
        {
            return tasks.FindIndex((Task t) => { return t.id == id; });
        }

        private IEnumerator ienumerator;

        IEnumerator ExecuteTask()
        {
            while (true)
            {
//                for (int i = 0; i < tasks.Count; i++)
//                {
//                    Task task = tasks[i];
//                    task.Execute();
//                    yield return null;
//                }
//                tasks.Clear();

                if (tasks.Count > 0)
                {
                    try
                    {
                        tasks[0].Execute();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }

                    tasks.RemoveAt(0);
                }
                yield return null;
            }
        }

        private void Update()
        {
            if (ienumerator == null)
            {
                ienumerator = ExecuteTask();
            }

            if (ienumerator.MoveNext())
            {
            }
            else
            {
                ienumerator = null;
            }
        }
    }
}