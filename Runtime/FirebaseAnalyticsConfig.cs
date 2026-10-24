using UnityEngine;

namespace Larje.Analytics.Firebase
{
    [CreateAssetMenu(fileName = "FirebaseAnalyticsConfig", menuName = "Larje/Analytics/Firebase Analytics Config")]
    public sealed class FirebaseAnalyticsConfig : ScriptableObject
    {
        [Header("Events")]
        [SerializeField, Min(0)] private int maxQueuedEvents = 100;
        [SerializeField] private bool logEvents;

        [Header("Collection")]
        [SerializeField] private bool collectionEnabled = true;

        public int MaxQueuedEvents => maxQueuedEvents;
        public bool LogEvents => logEvents;
        public bool CollectionEnabled => collectionEnabled;
    }
}
