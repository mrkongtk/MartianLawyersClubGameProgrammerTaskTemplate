using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gpt4All;
using UnityEngine;
using Zenject;
using System.Linq;
using UnityEngine.Windows;
using LlmChessAdvisorSpace;
using System.Text.RegularExpressions;

namespace LlmChessAdvisorSpace
{
    public static class ChessmanExtension
    {
        public static string ToLocationString(this Chessman chessman)
        {
            var targetColor = chessman.isWhite ? "White" : "Black";
            var targetXLocation = (char)('a' + chessman.CurrentX);
            var targetYLocation = (char)('1' + chessman.CurrentY);
            return $"{targetColor} {chessman.GetType()} {targetXLocation}{targetYLocation}";
        }

        public static string ToLocationString(this Vector2Int location)
        {
            var xLocation = (char)('a' + location.x);
            var yLocation = (char)('1' + location.y);
            return $"{xLocation}{yLocation}";
        }
    }
}

[RequireComponent(typeof(LlmManager))]
public class LlmChessAdvisor : IChessAdvisor
{
    private LlmManager llmManager;

    private IChessAdvisorResponseUpdatedDelegate onResponseUpdatedListener;
    private IChessAdvisorSubmitDelegate onSubmitListener;

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
        var result = await llmManager.Prompt(text);
        onSubmitListener?.Invoke(text, false);
        return result;
    }

    public override async Task<List<Vector2Int>> SuggestMovement(Chessman targetChessman, Chessman[,] chessmans, bool[,] allowedMoves)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("In a chess game, chessman locations are: ");

        var chessmanLocations = new List<string>();
        for (int x = 0; x <= chessmans.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= chessmans.GetUpperBound(1); y++)
            {
                var chessman = chessmans[x, y];
                if (chessman != null)
                {
                    chessmanLocations.Add(chessman.ToLocationString());
                }
            }
        }
        stringBuilder.Append(string.Join(", ", chessmanLocations));

        stringBuilder.Append($". Which location should {targetChessman.ToLocationString()} move? ");

        var allowedList = new List<Vector2Int>();
        for (int x = 0; x <= allowedMoves.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= allowedMoves.GetUpperBound(1); y++)
            {
                if (allowedMoves[x, y])
                {
                    allowedList.Add(new Vector2Int(x, y));
                }
            }
        }
        var possibleLocationsString = string.Join(" or ", allowedList.Select(a =>
        {
            var xLocation = (char)('a' + a.x);
            var yLocation = (char)('1' + a.y);
            return $"{xLocation}{yLocation}";
        }));
        stringBuilder.Append($"{possibleLocationsString}?");

        var response = await Submit(stringBuilder.ToString());

        var allExistingLocations = ExtractLocations(response);
        return allExistingLocations.AsParallel().Where(a =>
            allowedList.Contains(a)
            && !(a.x == targetChessman.CurrentX && a.y == targetChessman.CurrentY))
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
                var locationString = collections[i].Groups[1].Value;
                var x = locationString[0] - 'a';
                var y = locationString[1] - '1';
                results.Add(new Vector2Int(x, y));
            }
            catch
            {
            }
        }
        return results;
    }
}
