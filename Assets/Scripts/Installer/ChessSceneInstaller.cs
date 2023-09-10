using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ChessSceneInstaller : MonoInstaller
{
    [SerializeField]
    protected LlmChessAdvisor llmChessAdvisorPrefab;

    public override void InstallBindings()
    {
        Container.Bind<IChessAdvisor>().To<LlmChessAdvisor>()
            .FromComponentInNewPrefab(llmChessAdvisorPrefab).AsSingle();
    }
}
