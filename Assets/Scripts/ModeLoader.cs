using System.Collections.Generic;
using Applications.Slots.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

public class ModeLoader : MonoBehaviour
{
    public static ModeLoader Instance;
    public List<Texture2D> iconTextures;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Addressables.LoadAssetsAsync<Texture2D>($"default", null).Completed +=
            (o) =>
            {
                if (this == null || gameObject == null) return;
                if (!o.IsValid() || o.Status != AsyncOperationStatus.Succeeded) return;

                iconTextures = (List<Texture2D>)o.Result;
                EventServices.AddressableLoad.IconTextureLoaded?.Invoke();
            };
    }
}