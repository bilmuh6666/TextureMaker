using System;
using UnityEngine;

namespace Applications.Slots.Common
{
    public class EventServices : MonoBehaviour
    {
        public static EventServices Instance;

        private void Awake()
        {
            Instance ??= this;
        }

        [HideInInspector]
        public class AddressableLoad
        {
            public static Action IconTextureLoaded;
        }

    }
}