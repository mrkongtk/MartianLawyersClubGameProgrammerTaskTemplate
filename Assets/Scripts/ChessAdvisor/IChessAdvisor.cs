using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IChessAdvisor : MonoBehaviour
{
    public abstract bool IsLoaded();

    public abstract void AddResponseUpdatedListener(System.Action<string> listener);
    public abstract void RemoveResponseUpdatedListener(System.Action<string> listener);

    public abstract Task<string> Submit(string text);

}
