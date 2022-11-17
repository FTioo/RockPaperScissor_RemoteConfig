using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using System;

public struct userAttributes{
    public string difficultyLevel;
}

public struct appAttributes{

}

public class RemoteConfigFetcher : MonoBehaviour
{
    [SerializeField] string environmentName;
    [SerializeField] string difficultyLevel;
    [SerializeField] bool fetch;
    [SerializeField] float choosingInterval;
    [SerializeField] Player player;

    async void Awake() {
        var options = new InitializationOptions();
        options.SetEnvironmentName(environmentName);
        await UnityServices.InitializeAsync(options);

        Debug.Log("UGS Initialzed");

        if(AuthenticationService.Instance.IsSignedIn == false)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("Player Signed In");

        RemoteConfigService.Instance.FetchCompleted += OnFetchConfig;
    }

    private void OnDestroy() {
        RemoteConfigService.Instance.FetchCompleted -=OnFetchConfig;
    }

    private void OnFetchConfig(ConfigResponse response)
    {
        Debug.Log(response.requestOrigin);
        Debug.Log(response.body);

        switch (response.requestOrigin){
            case ConfigOrigin.Default:
                Debug.Log("Default");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("Cached");
                break;
            case ConfigOrigin.Remote:
                Debug.Log("Remote");
                choosingInterval = RemoteConfigService.Instance.appConfig.GetFloat("ChoosingInterval");
                player.SetChooseInterval(choosingInterval);
                break;
        }
    }

    void Update() {
        if(fetch)
        {
            fetch = false;

            RemoteConfigService.Instance.FetchConfigs(
                new userAttributes(){ difficultyLevel = this.difficultyLevel},
                new appAttributes(){}
            );
        }
    }
}
