using DI;
using MainApp.Configs;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace MainApp
{
    public class ApplicationStartup : MonoBehaviour
    {
        private static Model model;

        private static ServerConfig serverConfig;
        private static EDeviceType deviceType;
        
        private static Injector injector;
        private static ServerService serverService;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Preinitializing()
        {
            using (DeviceTypeChecker deviceTypeChecker = new DeviceTypeChecker())
            {
                deviceType = deviceTypeChecker.GetDeviceType();
            }

            var selfGo = new GameObject(nameof(ApplicationStartup)).AddComponent<ApplicationStartup>();
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            string configNameWithExtension = "server-config.json";
            serverConfig = ScriptableObject.CreateInstance<ServerConfig>();

            string serverConfigPath = Path.Combine(Application.streamingAssetsPath, configNameWithExtension).Replace('/', Path.DirectorySeparatorChar);
            string serverConifgContent = string.Empty;
            if (Application.platform == RuntimePlatform.Android)
            {
                StartCoroutine(LoadConfigOnAndroid(serverConfigPath, serverConifgContent));
            }
            else
            {
                serverConifgContent = LoadConfig(serverConfigPath);
            }

            ApplyConfigFromJson(serverConifgContent, serverConfig);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initializing()
        {
            model = new Model();

            serverService = new GameObject("Server Service").AddComponent<ServerService>();
            DontDestroyOnLoad(serverService);
            serverService.SetModel(model);
            serverService.SetConfig(serverConfig);
            serverService.Init();

            injector = new Injector();

            SceneManager.sceneLoaded += OnMainMenuLoaded;
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        private static void OnMainMenuLoaded(Scene arg0, LoadSceneMode arg1)
        {
            var allClients = FindObjectsOfType<MonoBehaviour>().OfType<IClient>();
            injector.UpdateClients(allClients);
            var allServices = FindObjectsOfType<MonoBehaviour>().OfType<IService>();
            injector.UpdateServices(allServices);
            injector.AddService(serverService);
            injector.InjectAll();

            var modelDependencies = FindObjectsOfType<MonoBehaviour>().OfType<IModelDepend>();
            foreach (var modelDependency in modelDependencies)
            {
                if (modelDependency is ServerService)
                    continue;

                modelDependency.SetModel(model);
            }

            var initializables = FindObjectsOfType<MonoBehaviour>().OfType<IInitializable>();
            foreach (var initializable in initializables)
                initializable.Initialize();

            var deviceTypeDependencies = FindObjectsOfType<MonoBehaviour>().OfType<IDeviceTypeDepender>();
            foreach (var deviceTypeDependency in deviceTypeDependencies)
                deviceTypeDependency.Set(deviceType);

            SceneManager.sceneLoaded -= OnMainMenuLoaded;
        }

        private static string LoadConfig(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogError($"Config file not found: {filePath}");
                return string.Empty;
            }
        }

        private static IEnumerator LoadConfigOnAndroid(string filePath, string json)
        {
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load {Path.GetFileName(filePath)}: {request.error}");
                yield break;
            }
            json = request.downloadHandler.text;

            yield break;
        }

        private static void ApplyConfigFromJson(string json, ServerConfig config)
        {
            try
            {
                JsonUtility.FromJsonOverwrite(json, config);
                Debug.Log($"Config '{config.name}' updated from JSON");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to apply config: {e.Message}\nJSON: {json}");
            }
        }
    }
}
