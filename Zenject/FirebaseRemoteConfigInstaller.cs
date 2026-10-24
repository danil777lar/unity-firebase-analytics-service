#if LARJE_FIREBASE_REMOTE_CONFIG
using Larje.Analytics.Firebase.RemoteConfig;
using UnityEngine;
using Zenject;

namespace Larje.Analytics.Firebase.Zenject
{
    [CreateAssetMenu(fileName = "FirebaseRemoteConfigInstaller", menuName = "Larje/Analytics/Firebase Remote Config Installer")]
    public sealed class FirebaseRemoteConfigInstaller : ScriptableObjectInstaller<FirebaseRemoteConfigInstaller>
    {
        [SerializeField] private FirebaseRemoteConfigSettings settings;

        public override void InstallBindings()
        {
            Container.BindInstance(settings);
            Container.BindInterfacesAndSelfTo<FirebaseRemoteConfigService>().AsSingle();
        }
    }
}
#endif
