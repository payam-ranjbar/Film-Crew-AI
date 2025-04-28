using UnityEngine;
using UnityEngine.UI;

namespace Agents.Utils
{
    public class ActorAgentExpression : MonoBehaviour
    {
        [SerializeField] private FacialExpressionProfile expressionProfile;
        [SerializeField] private Image faceImage;
        
        public void SetExpression(FacialExpression expression)
        {
            var sprite = expressionProfile.GetSprite(expression);
            faceImage.sprite = sprite;
        }

    }
}