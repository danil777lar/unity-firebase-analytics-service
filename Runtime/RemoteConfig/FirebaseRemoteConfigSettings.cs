using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Analytics.Firebase.RemoteConfig
{
    [CreateAssetMenu(fileName = "FirebaseRemoteConfigSettings", menuName = "Larje/Analytics/Firebase Remote Config Settings")]
    public sealed class FirebaseRemoteConfigSettings : ScriptableObject
    {
        [Header("Fetch")]
        [SerializeField, Min(0f)] private float cacheExpirationHours = 12f;

        [Header("Defaults")]
        [SerializeField] private List<DefaultValue> defaults = new List<DefaultValue>();

        public TimeSpan CacheExpiration => TimeSpan.FromHours(cacheExpirationHours);
        public IReadOnlyList<DefaultValue> Defaults => defaults;

        [Serializable]
        public sealed class DefaultValue
        {
            [SerializeField] private string key;
            [SerializeField] private string text;

            public string Key => key;
            public string Text => text;
        }
    }
}
