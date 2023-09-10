using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class RandomChessAdvisor : IChessAdvisor
{
    private IChessAdvisorResponseUpdatedDelegate onResponseUpdatedListener;
    private IChessAdvisorSubmitDelegate onSubmitListener;

    private bool isAnalysing = false;

    [Inject]
    private void Init()
    {
    }

    public override bool IsLoaded()
    {
        return false;
    }

    public override bool IsAnalysing()
    {
        return isAnalysing;
    }

    public override void AddResponseUpdatedListener(IChessAdvisorResponseUpdatedDelegate listener)
    {
        onResponseUpdatedListener += listener;
    }
    public override void RemoveResponseUpdatedListener(IChessAdvisorResponseUpdatedDelegate listener)
    {
        onResponseUpdatedListener -= listener;
    }

    public override void AddOnSubmitListener(IChessAdvisorSubmitDelegate listener)
    {
        onSubmitListener += listener;
    }

    public override void RemoveOnSubmitListener(IChessAdvisorSubmitDelegate listener)
    {
        onSubmitListener -= listener;
    }

    public override async Task<string> Submit(string text)
    {
        onSubmitListener?.Invoke(text, true);
        while (isAnalysing)
        {
            await Task.Yield();
        }
        isAnalysing = true;
        await Task.Delay(Random.Range(500, 1000));
        isAnalysing = false;
        onSubmitListener?.Invoke(text, false);
        return "";
    }

    public override async Task<List<Vector2Int>> SuggestMovement(Query query)
    {
        var allowedMoves = query.allowedMove;

        await Submit("");

        var random = Random.Range(-1, allowedMoves.Count);

        if (random >= 0)
        {
            return new List<Vector2Int>() { allowedMoves[random] };
        }
        else
        {
            return new List<Vector2Int>();
        }
    }
}
