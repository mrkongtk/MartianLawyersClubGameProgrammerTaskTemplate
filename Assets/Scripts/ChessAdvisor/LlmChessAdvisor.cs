using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gpt4All;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(LlmManager))]
public class LlmChessAdvisor : IChessAdvisor
{
    private LlmManager llmManager;

    private System.Action<string> onResponseUpdatedListener;

    [Inject]
    private void Init()
    {
        llmManager = GetComponent<LlmManager>();
        llmManager.OnResponseUpdated += OnResponseUpdated;
    }

    public override bool IsLoaded()
    {
        return llmManager.IsLoaded;
    }

    public override void AddResponseUpdatedListener(System.Action<string> listener)
    {
        onResponseUpdatedListener += listener;
    }
    public override void RemoveResponseUpdatedListener(System.Action<string> listener)
    {
        onResponseUpdatedListener -= listener;
    }

    public override async Task<string> Submit(string text)
    {
        return await llmManager.Prompt(text);
    }

    private void OnResponseUpdated(string response)
    {
        onResponseUpdatedListener?.Invoke(response);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
