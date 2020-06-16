using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Tang
{
    public class RoomConfig
    {
        public string id;
        public string rawId;

        [JsonIgnore]
        public string RawId
        {
            get
            {
                return string.IsNullOrEmpty(rawId) == true ? id : rawId;
            }
        }
    }

    public class PathConfig
    {
        public string pathName;
        public string beginRoom;
        public List<RoomConnectionConfig> connections;
    }

    public class RoomConnectionConfig
    {
        public string fromScene;
        public string fromPortal;
        public string toScene;
        public string toPortal;
    }

    public class RoomObjectConfig
    {
        public string areaId;
        public string objectId;
        public int count = 1;
    }

    public class RoomObjectesConfig
    {
        public string roomId;
        public List<RoomObjectConfig> objects;
    }

    public class SceneEvents
    {
        public class RoomEvent
        {
            public string roomId;
            public List<SceneEvent> events;
        }

        public List<Parameter> parameters;
        public List<RoomEvent> rooms;
        public List<SceneEvent> events;

        [JsonIgnore] private Dictionary<string, Parameter> parameterDic;
        [JsonIgnore]
        public Dictionary<string, Parameter> ParameterDic
        {
            get
            {
                if (parameterDic == null)
                {
                    parameterDic = new Dictionary<string, Parameter>();

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        Parameter parameter = parameters[i];
                        parameterDic.Add(parameter.name, parameter);
                    }
                }
                return parameterDic;
            }
            set
            {
                parameterDic = value;
            }
        }

        [JsonIgnore] private Dictionary<SceneEventType, List<SceneEvent>> eventDic;
        [JsonIgnore]
        public Dictionary<SceneEventType, List<SceneEvent>> EventDic
        {
            get
            {
                if (eventDic == null)
                {
                    eventDic = new Dictionary<SceneEventType, List<SceneEvent>>();

                    // 添加event到eventDic
                    if (events != null)
                    {
                        for (int i = 0; i < events.Count; i++)
                        {
                            SceneEvent sceneEvent = events[i];
                            List<SceneEvent> events_;
                            if (eventDic.TryGetValue(sceneEvent.eventType, out events_))
                            {
                                events_.Add(sceneEvent);
                            }
                            else
                            {
                                eventDic.Add(sceneEvent.eventType, new List<SceneEvent>() { sceneEvent });
                            }
                        }
                    }

                    // 添加房间市间到eventDic
                    if (rooms != null)
                    {
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            RoomEvent roomEvent = rooms[i];

                            for (int j = 0; j < roomEvent.events.Count; j++)
                            {
                                SceneEvent sceneEvent = roomEvent.events[j];

//                                sceneEvent.conditions.Add(new Condition() { parameter = "roomId", conditionMode = ConditionMode.Equal, value = roomEvent.roomId });

                                List<SceneEvent> events_;
                                if (eventDic.TryGetValue(sceneEvent.eventType, out events_))
                                {
                                    events_.Add(sceneEvent);
                                }
                                else
                                {
                                    eventDic.Add(sceneEvent.eventType, new List<SceneEvent>() { sceneEvent });
                                }
                            }
                        }
                    }
                }
                return eventDic;
            }
        }

        public List<SceneEvent> GetEventsByType(SceneEventType eventType)
        {
            List<SceneEvent> sceneEvents;
            if (EventDic.TryGetValue(eventType, out sceneEvents))
            {
                return sceneEvents;
            }
            return null;
        }

        public Parameter GetParameter(string name)
        {
            Parameter parameter;
            if (parameterDic.TryGetValue(name, out parameter))
            {
                return parameter;
            }
            return null;
        }

        public int GetInt(string name)
        {
            Parameter parameter;
            if (ParameterDic.TryGetValue(name, out parameter))
            {
                if (parameter.type == ParameterType.Int)
                {
                    return (int)parameter.value;
                }
            }

            Debug.LogError("没有找到Int: " + name);
            return -1;
        }

        public float GetFloat(string name)
        {
            Parameter parameter;
            if (ParameterDic.TryGetValue(name, out parameter))
            {
                if (parameter.type == ParameterType.Float)
                {
                    return (float)parameter.value;
                }
            }

            Debug.LogError("没有找到float: " + name);
            return -1;
        }

        public string GetString(string name)
        {
            Parameter parameter;
            if (ParameterDic.TryGetValue(name, out parameter))
            {
                if (parameter.type == ParameterType.String)
                {
                    return parameter.value as string;
                }
            }

            Debug.LogError("没有找到string: " + name);
            return null;
        }

        public void SetInt(string name, int value)
        {
            Parameter parameter;
            if (ParameterDic.TryGetValue(name, out parameter))
            {
                if (parameter.type == ParameterType.Int)
                {
                    parameter.value = value;
                    return;
                }
            }

            Debug.LogError("没有找到Int: " + name);
        }

        public void SetFloat(string name, float value)
        {
            Parameter parameter;
            if (ParameterDic.TryGetValue(name, out parameter))
            {
                if (parameter.type == ParameterType.Float)
                {
                    parameter.value = value;
                    return;
                }
            }

            Debug.LogError("没有找到float: " + name);
        }

        public void SetString(string name, string value)
        {
            Parameter parameter;
            if (ParameterDic.TryGetValue(name, out parameter))
            {
                if (parameter.type == ParameterType.String)
                {
                    parameter.value = value;
                    return;
                }
            }

            Debug.LogError("没有找到string: " + name);
        }

        public bool DoCondition(Condition condition)
        {
            Parameter parameter = GetParameter(condition.parameter);
            if (parameter == null)
            {
                Debug.Log("找不到Parameter:" + condition.parameter);
                return false;
            }
            
            var value = condition.value;
            var conditionMode = condition.conditionMode;
            
            switch (parameter.type)
            {
                case ParameterType.Int:
                {
                    int a = Convert.ToInt32(parameter.value);
                    int b = Convert.ToInt32(value);

                    switch (conditionMode)
                    {
                        case ConditionMode.Equal:
                            return a == b;
                        case ConditionMode.Less:
                            return a < b;
                        case ConditionMode.Greater:
                            return a > b;
                        default:
                            return false;
                    }
                }

                case ParameterType.Float:
                {
                    float a = Convert.ToSingle(parameter.value);
                    float b = Convert.ToSingle(value);

                    switch (conditionMode)
                    {
                        case ConditionMode.Equal:
                            return a == b;
                        case ConditionMode.Less:
                            return a < b;
                        case ConditionMode.Greater:
                            return a > b;
                        default:
                            return false;
                    }
                }

                case ParameterType.String:
                {
                    string a = Convert.ToString(parameter.value);
                    string b = Convert.ToString(value);

                    switch (conditionMode)
                    {
                        case ConditionMode.Equal:
                            return a == b;
                        default:
                            return false;
                    }
                }
                default:
                    return false;
            }
        }

        public bool DoConditions(List<Condition> conditions)
        {
            if (conditions == null || conditions.Count <= 0)
                return true;
            
            for (int i = 0; i < conditions.Count; i++)
            {
                Condition condition = conditions[i];
                if (DoCondition(condition))
                {
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void DoResult(Result result)
        {
            Debug.Log("执行结果: " + result.resultType + ":" + result.parameters);

            if (ResultDelegates != null)
            {
                ResultDelegates(result);
            }
        }

        public void DoResults(List<Result> results)
        {
            for (int i = 0; i < results.Count; i++)
            {
                Result result = results[i];
                DoResult(result);
            }
        }

        public void DoConditionsAndResults(SceneEventType eventType)
        {
            List<SceneEvent> sceneEvents = GetEventsByType(eventType);
            if (sceneEvents != null)
            {
                for (int i = 0; i < sceneEvents.Count; i++)
                {
                    SceneEvent sceneEvent = sceneEvents[i];
                    if (sceneEvent.times < sceneEvent.totalTimes
                        && DoConditions(sceneEvent.conditions))
                    {
                        sceneEvent.times++;
                        DoResults(sceneEvent.results);
                    }
                }
            }
        }

        public delegate void ResultDelegate(Result result);
        public event ResultDelegate ResultDelegates;
    }

    public class LevelConfig
    {
        public int difficultyLevel = 1;
        public List<RoomConfig> rooms = new List<RoomConfig>();
        public List<PathConfig> paths = new List<PathConfig>();
        public List<RoomObjectesConfig> objectses = new List<RoomObjectesConfig>();
        public SceneEvents sceneEvents = new SceneEvents();

        [JsonIgnore] public PathConfig currPath;
    }
}