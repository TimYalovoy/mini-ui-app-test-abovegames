using DI;
using MainApp.Configs;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace MainApp
{
    public class ApplicationStartup : MonoBehaviour
    {
        private static ServerConfig serverConfig;
        private static EDeviceType deviceType;
        
        private static Injector injector;
        private static ServerService serverService;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Preinitializing()
        {
            DeviceTypeChecker deviceTypeChecker = new DeviceTypeChecker();
            deviceType = deviceTypeChecker.GetDeviceType();

            serverConfig = ScriptableObject.CreateInstance<ServerConfig>();

        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initializing()
        {
            
            


            serverService = new ServerService();

            injector = new Injector();
            var allClients = FindObjectsOfType<MonoBehaviour>().OfType<IClient>();
            injector.UpdateClients(allClients);
            var allServices = FindObjectsOfType<MonoBehaviour>().OfType<IService>();
            injector.UpdateServices(allServices);
            injector.AddService(serverService);
        }
    }
}
