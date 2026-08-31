using UnityEngine;
using UnityEngine.EventSystems;


    public class PlayerTarget : MonoBehaviour,IPointerClickHandler
    {
        [SerializeField] private PlayerState player;
        [SerializeField] private BattleManager battleManager;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (player == null || battleManager == null)
            {
                return;
            }

            battleManager.AttackPlayer(player);
        }
    }

