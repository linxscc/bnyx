using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using FairyGUI;
using Spine.Unity;
using Tang;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class GameStart : MyMonoBehaviour
{
    private static GameStart s_instance;

    public static GameStart Instance
    {
        get { return s_instance; }
    }

    private AsyncOperation _asyncOperation;
    private UIPanel _uiPanel;
    private GProgressBar _gProgressBar;

    private bool isLoading = false;

    private int level = 1;

    private LoadingUIController loadingUIController;
    
    // Start is called before the first frame update
    void Start()
    {
        s_instance = this;
        StartCoroutine("LoadGame", LoadGame());
    }

    IEnumerator InitAsset(int weight)
    {
        var handle = Addressables.InitializeAsync();
        while (!handle.IsDone)
        {
            yield return null;
        }

        AssetManager.Initialize();
        
        yield return weight;
    }
    IEnumerator LoadAssetBylabel<T>(string label,string extension,int weight)
    {
        IList<IResourceLocation> iResourceLocations;
        Addressables.ResourceLocators[0].Locate(label,typeof(Object), out iResourceLocations);
        for (int i = 0; i < iResourceLocations.Count; i++)
        {
            if (Path.GetExtension(iResourceLocations[i].InternalId).ToLower() == extension)
            {
                var handle = Addressables.LoadAssetAsync<T>(iResourceLocations[i]);
#if UNITY_EDITOR
#elif UNITY_STANDALONE_WIN
                while (!handle.IsDone)
                {
                    yield return (int) ((handle.PercentComplete + i) / iResourceLocations.Count * weight);
                }
#endif
            }
        }
        yield return weight;
    }
    IEnumerator LoadSceneGame(int weight)
    {
        Scene scene = SceneManager.GetSceneByName("Game");
        
        if (scene.isLoaded)
        {
            _asyncOperation = SceneManager.UnloadSceneAsync(scene);
            while (!_asyncOperation.isDone)
            {
                yield return 0;
            }
        }

        _asyncOperation = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        _asyncOperation.allowSceneActivation = false;
        yield return 0;

        float progress = 0;

        while (!_asyncOperation.isDone)
        {
            progress = _asyncOperation.progress < 0.9f ? _asyncOperation.progress : 1;
            
            yield return progress*weight;

            if (progress >= 0.9f)
            {
                _asyncOperation.allowSceneActivation = true;
            }
        }
    }
    
  
    IEnumerator LoadGame(int level = 1)
    {
        if (isLoading == false)
        {
            isLoading = true;
            
            Application.backgroundLoadingPriority = ThreadPriority.Low;
           
        
            loadingUIController = UIManager.Instance.GetUI<LoadingUIController>("Loading");
            loadingUIController.Show();
            
            loadingUIController.SetProgress(0);
            yield return null;
            
            Loader loader = new Loader();
        
            loader.Items = new List<Loader.Item>
            {
                new Loader.Item()
                {
                  stepCount  = 50,
                  action = InitAsset(50)
                },
                new Loader.Item()
                {
                    stepCount = 1500,
                    action = LoadAssetBylabel<GameObject>("Prefab",".prefab",1500)
                },
                new Loader.Item()
                {
                    stepCount = 300,
                    action = LoadAssetBylabel<AudioClip>("Music",".wav",300)
                },
                new Loader.Item()
                {
                    stepCount = 100,
                    action = LoadAssetBylabel<TextAsset>("Text",".txt",100)
                },
                new Loader.Item()
                {
                    stepCount = 300,
                    action = LoadAssetBylabel<Texture>("Texture",".png",300)
                },
                new Loader.Item()
                {
                    stepCount = 10,
                    action = LoadSceneGame(10)
                }
            };

            IEnumerator update = loader.Update();
            while (update.MoveNext())
            {
                float percent = (float)update.Current;
                loadingUIController.SetProgress(percent);
                yield return null;
            }
            // 完成 add by TangJian 2019/3/25 21:37
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            
            // 隐藏UI add by TangJian 2019/3/25 22:17
//            loadingUIController.Hide();
            
            // 开始游戏 add by TangJian 2019/3/25 21:38
            GameManager.Instance.difficultyLevel = level;
            GameManager.Instance.StartGame();

            isLoading = false;
        }
    }

    public override void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ReloadGame();
        }
    }

    public void ReloadGame()
    {
        StartCoroutine(LoadGame());
    }

    public void ReloadNextLevel(RoleData player1Data)
    {
        StartCoroutine(LoadGame(++level));
    }
}