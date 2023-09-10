using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IChessAdvisor : MonoBehaviour
{
    public delegate void IChessAdvisorSubmitDelegate(string query, bool start);
    public delegate void IChessAdvisorResponseUpdatedDelegate(string text);

    public abstract bool IsLoaded();

    public abstract void AddResponseUpdatedListener(IChessAdvisorResponseUpdatedDelegate listener);
    public abstract void RemoveResponseUpdatedListener(IChessAdvisorResponseUpdatedDelegate listener);

    public abstract void AddOnSubmitListener(IChessAdvisorSubmitDelegate listener);
    public abstract void RemoveOnSubmitListener(IChessAdvisorSubmitDelegate listener);

    public abstract Task<string> Submit(string text);

    public abstract Task<List<Vector2Int>> SuggestMovement(Chessman targetChessman, Chessman[,] chessmans, bool[,] allowedMoves);
}
