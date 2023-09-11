using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gpt4All;
using UnityEngine;
using Zenject;
using System.Linq;
using IChessAdvisorSpace;
using System.Text.RegularExpressions;

[RequireComponent(typeof(LlmManager))]
public class LlmChessAdvisor : IChessAdvisor
{
    private LlmManager llmManager;

    private IChessAdvisorResponseUpdatedDelegate onResponseUpdatedListener;
    private IChessAdvisorSubmitDelegate onSubmitListener;

    private bool isAnalysing = false;

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
        var result = await llmManager.Prompt(text);
        isAnalysing = false;
        onSubmitListener?.Invoke(text, false);
        return result;
    }

    public override async Task<List<Vector2Int>> SuggestMovement(Query query)
    {
        var targetChessman = query.target;
        var chessmans = query.board;
        var allowedMoves = query.allowedMove;

        var stringBuilder = new StringBuilder();
        stringBuilder.Append("In a chess game, chessman locations are: ");

        stringBuilder.Append(string.Join(", ", chessmans.Select(a => a.ToLocationString())));

        stringBuilder.Append($". Which location should {targetChessman.ToLocationString()} move? ");

        var possibleLocationsString = string.Join(" or ", allowedMoves.Select(a =>
        {
            var xLocation = (char)('a' + a.x);
            var yLocation = (char)('1' + a.y);
            return $"{xLocation}{yLocation}";
        }));
        stringBuilder.Append($"{possibleLocationsString}?");

        var response = await Submit(stringBuilder.ToString());

        var allExistingLocations = ExtractLocations(response);
        return allExistingLocations.AsParallel().Where(a =>
            allowedMoves.Contains(a)
            && !(a.x == targetChessman.x && a.y == targetChessman.y))
            .ToHashSet()
            .ToList();
    }

    private void OnResponseUpdated(string response)
    {
        onResponseUpdatedListener?.Invoke(response);
    }


    private List<Vector2Int> ExtractLocations(string message)
    {
        var results = new List<Vector2Int>();
        string pattern = @"([a-h][1-8])";
        Regex rg = new Regex(pattern);
        var collections = rg.Matches(message);
        for (int i = 0; i < collections.Count; i++)
        {
            try
            {
                var location = collections[i].Groups[1].Value.FromLocationString();
                if (location.HasValue)
                {
                    results.Add(location.Value);
                }
            }
            catch
            {
            }
        }
        return results;
    }
}
