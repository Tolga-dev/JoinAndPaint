
using TMPro;
using UnityEngine;

namespace GameObjects.Prizes
{
    public enum SelectorEnum
    {
        Good, // sum
        Bad // subs
    }
    
    public class Selector : Prize
    {
        public SelectorEnum selectorEnum;

        public TextMeshPro selectionText;
        public SpriteRenderer gateSprite;

        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (isHitPlayer)
            {
                gameManager.spawnerManager.prizeSpawner.CreateMemberFromSelector(prizeAmount, transform);
            }
        }

        public void SetText()
        {
            selectionText.text =  GetOperation() + " " + prizeAmount;
        }

        public string GetOperation()
        {
            if (selectorEnum == SelectorEnum.Bad)
                return "-";
            return "+";
        }
    }
}