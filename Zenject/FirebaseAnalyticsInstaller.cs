using Larje.Core.Zenject;
using UnityEngine;
using Zenject;

namespace Larje.Analytics.Firebase.Zenject
{
    [CreateAssetMenu(fileName = "FirebaseAnalyticsInstaller", menuName = "Larje/Analytics/Firebase Analytics Installer")]
    public sealed class FirebaseAnalyticsInstaller : ScriptableObjectInstaller<FirebaseAnalyticsInstaller>
    {
        [SerializeField] private FirebaseAnalyticsConfig config;

        public override void InstallBindings()
        {
            Container.BindInstance(config);
            Container.BindLarjeAnalyticsProvider<FirebaseAnalyticsService>();
        }
    }
}
