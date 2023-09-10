using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlights : MonoBehaviour
{

    public static BoardHighlights Instance { set; get; }

    public GameObject highlightPrefab;
    private List<GameObject> highlights;

    public GameObject suggestionPrefab;
    private List<GameObject> suggestions;

    private void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
        suggestions = new List<GameObject>();
    }

    private GameObject GetHighLightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);

        if (go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }

        return go;
    }

    private GameObject GetSuggestionObject()
    {
        GameObject go = suggestions.Find(g => !g.activeSelf);

        if (go == null)
        {
            go = Instantiate(suggestionPrefab);
            suggestions.Add(go);
        }

        return go;
    }

    public void HighLightAllowedMoves(bool[,] moves)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j])
                {
                    GameObject go = GetHighLightObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i + 0.5f, 0.0001f, j + 0.5f);
                }
            }

        }
    }

    public void HideHighlights()
    {
        foreach (GameObject go in highlights)
            go.SetActive(false);
    }

    public void SuggestionMoves(List<Vector2Int> locations)
    {
        locations.ForEach(a =>
        {
            GameObject go = GetSuggestionObject();
            go.SetActive(true);
            go.transform.position = new Vector3(a.x + 0.5f, 0.0002f, a.y + 0.5f);
        });
    }

    public void HideSuggestions()
    {
        foreach (GameObject go in suggestions)
        {
            go.SetActive(false);
        }
    }

    public void HideAll()
    {
        HideHighlights();
        HideSuggestions();
    }
}
