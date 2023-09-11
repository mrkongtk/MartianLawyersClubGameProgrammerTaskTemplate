using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ChessSceneInstaller : MonoInstaller
{
    [SerializeField]
    protected IChessAdvisor chessAdvisorPrefab;

    public override void InstallBindings()
    {
        Container.Bind<IChessAdvisor>()
            .FromComponentInNewPrefab(chessAdvisorPrefab).AsSingle();
    }
}
