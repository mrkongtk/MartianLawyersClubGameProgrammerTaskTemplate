using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using IChessAdvisorSpace;

namespace IChessAdvisorSpace
{
    public static class ChessmanExtension
    {
        public static string ToLocationString(this IChessAdvisor.ChessmanInfo chessman)
        {
            var targetColor = chessman.isWhite ? "White" : "Black";
            var targetXLocation = (char)('a' + chessman.x);
            var targetYLocation = (char)('1' + chessman.y);
            return $"{targetColor} {chessman.type} {targetXLocation}{targetYLocation}";
        }

        public static string ToLocationString(this Vector2Int location)
        {
            var xLocation = (char)('a' + location.x);
            var yLocation = (char)('1' + location.y);
            return $"{xLocation}{yLocation}";
        }

        public static Vector2Int? FromLocationString(this string locationString)
        {
            string pattern = @"([a-h][1-8])";
            Regex rg = new Regex(pattern);
            if (rg.IsMatch(locationString))
            {
                var x = locationString[0] - 'a';
                var y = locationString[1] - '1';
                return new Vector2Int(x, y);
            }
            return null;
        }
    }
}

public abstract class IChessAdvisor : MonoBehaviour
{
    public delegate void IChessAdvisorSubmitDelegate(string query, bool start);
    public delegate void IChessAdvisorResponseUpdatedDelegate(string text);

    public abstract bool IsLoaded();
    public abstract bool IsAnalysing();

    public abstract void AddResponseUpdatedListener(IChessAdvisorResponseUpdatedDelegate listener);
    public abstract void RemoveResponseUpdatedListener(IChessAdvisorResponseUpdatedDelegate listener);

    public abstract void AddOnSubmitListener(IChessAdvisorSubmitDelegate listener);
    public abstract void RemoveOnSubmitListener(IChessAdvisorSubmitDelegate listener);

    public abstract Task<string> Submit(string text);

    public abstract Task<List<Vector2Int>> SuggestMovement(Query query);

    public class ChessmanInfo
    {
        public readonly int x;
        public readonly int y;

        public readonly Type type;
        public readonly bool isWhite;

        public ChessmanInfo(Chessman chessman)
        {
            x = chessman.CurrentX;
            y = chessman.CurrentY;
            type = chessman.GetType();
            isWhite = chessman.isWhite;
        }

        public bool IsSame(Chessman chessman)
        {
            if (chessman == null)
            {
                return false;
            }
            return x == chessman.CurrentX
                && y == chessman.CurrentY
                && type == chessman.GetType()
                && isWhite == chessman.isWhite;
        }
    }

    public class Query
    {
        public readonly ChessmanInfo target;
        public readonly List<ChessmanInfo> board;
        public readonly List<Vector2Int> allowedMove;

        public Query(Chessman target, Chessman[,] chessmen, bool[,] allowedMove)
        {
            this.target = new ChessmanInfo(target);
            board = new List<ChessmanInfo>();
            for (int x = 0; x <= chessmen.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= chessmen.GetUpperBound(1); y++)
                {
                    if (chessmen[x, y] != null)
                    {
                        board.Add(new ChessmanInfo(chessmen[x, y]));
                    }
                }
            }
            this.allowedMove = new List<Vector2Int>();
            for (int x = 0; x <= allowedMove.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= allowedMove.GetUpperBound(1); y++)
                {
                    if (allowedMove[x, y])
                    {
                        this.allowedMove.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }
}
