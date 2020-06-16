using UnityEngine;

//
public class UIManager : UIBaseManager
{
    private static UIManager _Instance;
    public static UIManager Instance
    {
        get
        {
            if (!_Instance)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>(UIManagerPath.RootPath));
                _Instance = go.GetComponent<UIManager>();
                DontDestroyOnLoad(go);
            }
            return _Instance;
        }
    }


    [SerializeField] private Transform FGUIParent;
    [SerializeField] private Transform SpineUIParent;



    private void Awake()
    {
        FGUIParent = transform.Find("FairyUI").transform;
        SpineUIParent = transform.Find("UICamera").transform;
    }



    public override void CreatFGUIObj(string Name)
    {
        judgeFGUICreat(Name);
    }
    public T CreatFGUIObj<T>(string Name)
    {
        judgeFGUICreat(Name);
        string name = Name + "(Clone)";
        return insObjs[name].GetComponent<T>();
    }
    public override void CreatSUIObj(string Name)
    {
        judgeSpineGUICreat(Name);
    }
    public T CreatSUIObj<T>(string Name)
    {
        judgeSpineGUICreat(Name);
        string name = Name + "(Clone)";
        return insObjs[name].GetComponent<T>();
    }
    public override void DesUI(string Name)
    {
        judgeDesUI(Name);
    }
    public override void ShowUI(string Name)
    {
        string name = Name + "(Clone)";
        insObjs[name].SetActive(true);
    }
    public override void HideUI(string Name)
    {
        string name = Name + "(Clone)";
        insObjs[name].SetActive(false);
    }
    public override void DesIm(string Name)
    {

        DestroyImmediate(objs[Name]);
    }

    private void judgeFGUICreat(string Name)
    {
        if (!objs.ContainsKey(Name) && !insObjs.ContainsKey(Name + "(Clone)"))
        {
            GameObject Obj = Resources.Load<GameObject>(UIManagerPath.FGUIPath + Name);
            objs.Add(Obj.name, Obj);
            GameObject ins = Instantiate(Obj, FGUIParent);
            insObjs.Add(ins.name, ins);
        }
        else if (objs.ContainsKey(Name) && !insObjs.ContainsKey(Name + "(Clone)"))
        {
            GameObject ins = Instantiate(objs[Name], FGUIParent);
            insObjs.Add(ins.name, ins);
        }
        else
        {
            Debug.Log("创建内容已存在！");
        }
    }

    private void judgeSpineGUICreat(string Name)
    {
        if (!objs.ContainsKey(Name) && !insObjs.ContainsKey(Name + "(Clone)"))
        {
            GameObject Obj = Resources.Load<GameObject>(UIManagerPath.SpinePath + Name);
            objs.Add(Obj.name, Obj);
            GameObject ins = Instantiate(Obj, SpineUIParent);
            insObjs.Add(ins.name, ins);


        }
        else if (objs.ContainsKey(Name) && !insObjs.ContainsKey(Name + "(Clone)"))
        {
            GameObject ins = Instantiate(insObjs[Name], SpineUIParent);
            insObjs.Add(ins.name, ins);
        }
        else
        {
            Debug.Log("创建内容已存在");
        }
    }
    private void judgeDesUI(string Name)
    {
        string name = Name + "(Clone)";
        if (insObjs.ContainsKey(name))
        {

            Destroy(insObjs[name]);
            insObjs.Remove(name);
        }
        else
        {
            Debug.Log("该物体不存在");
        }
    }





    public T GetUI<T>(string uiName) where T : MonoBehaviour
    {
        UIInterface uiInterface = null;
        if (uis.TryGetValue(uiName, out uiInterface))
        {
        }
        else
        {
            GameObject uiPrefab = Resources.Load<GameObject>(UIManagerPath.FGUIPath + uiName);
            if (uiPrefab == null)
                uiPrefab = Resources.Load<GameObject>(UIManagerPath.SpinePath + uiName);

            if (uiPrefab != null)
            {
                var uiObj = Instantiate(uiPrefab, FGUIParent);

                if (!objs.ContainsKey(uiName))
                    objs.Add(uiName, uiObj);

                uiInterface = uiObj.GetComponent<UIInterface>();
                if (uiInterface != null)
                {
                    // uiInterface.Init();
                    if (!uis.ContainsKey(uiName))
                        uis.Add(uiName, uiInterface);
                }
                else
                {
                    Debug.LogError("UI不为UIInterface类型: " + uiName);
                }
            }
            else
            {
                Debug.LogError("UI找不到: " + uiName);
            }
        }
        
        return uiInterface as T;
    }

    public void ShowAll(bool withAnim)
    {
        foreach (var ui in uis)
        {
            ui.Value.Show(withAnim);
        }
    }

    public void HideAll(bool withAnim)
    {
        foreach (var ui in uis)
        {
            ui.Value.Hide(withAnim);
        }
    }
}
