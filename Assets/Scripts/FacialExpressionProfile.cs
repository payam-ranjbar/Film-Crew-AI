using System;
using System.Collections.Generic;
using UnityEngine;

public enum FacialExpression
{
    Neutral, 
    Happy,
    Sad,
    Angry, 
    Excited,
    Cocky, 
    Bored,
    Curious
}

[Serializable]
public class FacialExpressionSprite
{
    public FacialExpression expression;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "Facial Expression Profile", menuName = "Facial Expression Profile", order = 0)]
public class FacialExpressionProfile : ScriptableObject
{
    [SerializeField] private Sprite defaultSprite;

    [SerializeField] private List<FacialExpressionSprite> expressions;

    public Sprite GetSprite(FacialExpression expression)
    {
        for (var i = 0; i < expressions.Count; i++)
        {
            var expr = expressions[i];
            if (expr.expression == expression)
                return expr.sprite;
        }

        return defaultSprite;
    }

    [ContextMenu("Generate empty list")]
    private void InitializeList()
    {
        var exprs = new List<string>(Enum.GetNames(typeof(FacialExpression)));
        
        expressions = new List<FacialExpressionSprite>();
        
        for (var i = 0; i < exprs.Count; i++)
        {
            Debug.Log(exprs[i]);
            Enum.TryParse(exprs[i], out FacialExpression ex);
            expressions.Add(new FacialExpressionSprite(){expression = ex});
        }
    }
}