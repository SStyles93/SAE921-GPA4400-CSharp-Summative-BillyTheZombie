using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Managers
{
    public class BillyHouseScene : MonoBehaviour
    {
        [SerializeField] GameObject _player;

        private void Start()
        {
            OverrideStats();
        }

        /// <summary>
        /// Overrides the player stats when he is in a Non-combat scene
        /// </summary>
        private void OverrideStats()
        {
            PlayerStats playerStats = _player.GetComponent<PlayerStats>();
            playerStats.Speed = 8.0f;
            //playerStats.ResetLife();

            PlayerActions playerActions = _player.GetComponent<PlayerActions>();
            playerActions.CanHit = false;
            playerActions.IsInCombat = false;
            _player.GetComponent<PlayerController>().CanRepeateActions = true;

        }
    }
}
