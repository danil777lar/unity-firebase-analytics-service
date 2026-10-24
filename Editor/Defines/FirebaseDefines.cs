using Larje.Core.Editor.Defines;
using UnityEditor;

namespace Larje.Analytics.Firebase.Editor.Defines
{
    [InitializeOnLoad]
    internal static class FirebaseDefines
    {
        static FirebaseDefines()
        {
            OptionalDependencyDefines.Register("Firebase.Analytics", "LARJE_FIREBASE_ANALYTICS");
            OptionalDependencyDefines.Register("Firebase.RemoteConfig", "LARJE_FIREBASE_REMOTE_CONFIG");
        }
    }
}
