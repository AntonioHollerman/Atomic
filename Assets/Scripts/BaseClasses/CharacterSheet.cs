using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseClasses
{
    /// <summary>
    /// Represents a character's core attributes and behaviors in the game.
    /// </summary>
    public class CharacterSheet : Savable
    {
        public int lvl; // Character level
        public bool IsALive { get; private set; } // Flag indicating if the character is alive

        // Base stats for derived classes to define
        protected int BaseHp;
        protected int BaseMana;
        protected int BaseAtk;
        protected int BaseSpeed;

        // Private fields for current character stats
        private int _hp;    // Current health points
        private int _mana;  // Current mana points
        private int _atk;   // Current attack power
        private int _speed; // Current speed value
        private int _def = 0; // Current defense value, default is 0

        // Dictionary to track active status effects and their durations
        private Dictionary<StatusEffect, float> _activeEffects;
        private float _vulnerableDuration = 0.0f; // Duration for which the character is vulnerable
        private bool IsVulnerable { get => _vulnerableDuration > 0; } // Property to check if character is vulnerable
        private float _stunDuration = 0.0f; // Duration for which the character is stunned
        private bool IsStunned { get => _stunDuration > 0; } // Property to check if character is stunned

        // Dictionary for equipped items and list for techniques
        private Dictionary<string, Equipment> _equipment;
        private List<Technique> _techniques;

        // Property to manage the length of the techniques list
        private int _techLen;
        public int TechniquesLength
        {
            get => _techLen;
            set
            {
                if (value > _techLen)
                {
                    // If the new length is greater, add null entries to the list
                    for (int i = 0; i < value - _techLen; i++)
                    {
                        _techniques.Add(null);
                    }
                }
                else
                {
                    // If the new length is smaller, truncate the list
                    _techniques = _techniques.Take(value).ToList();
                }
                _techLen = value; // Update the technique length value
            }
        }

        /// <summary>
        /// Initializes the character's state at the start of the game.
        /// </summary>
        protected override void StartWrapper()
        {
            base.StartWrapper(); // Call base start logic
            UpdateStats(); // Update initial stats based on level and base attributes

            _activeEffects = new Dictionary<StatusEffect, float>(); // Initialize status effects
            _equipment = new Dictionary<string, Equipment>(); // Initialize equipment dictionary

            // Initialize techniques list with null entries
            _techniques = new List<Technique>();
            for (int i = 0; i < _techLen; i++)
            {
                _techniques.Add(null);
            }
        }

        /// <summary>
        /// Updates the character state each frame.
        /// </summary>
        protected override void UpdateWrapper()
        {
            base.UpdateWrapper(); // Call base update logic

            // Decrease vulnerability and stun duration if applicable
            _vulnerableDuration -= IsVulnerable ? Time.deltaTime : 0;
            _stunDuration -= IsStunned ? Time.deltaTime : 0;

            // Filter out expired status effects
            _activeEffects = _activeEffects
                .Where(kvp => kvp.Value > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Apply active status effects
            foreach (var kvp in _activeEffects)
            {
                kvp.Key(this, Time.deltaTime); // Apply effect to this character
                _activeEffects[kvp.Key] -= Time.deltaTime; // Reduce the effect duration
            }
        }

        /// <summary>
        /// Calculates and updates character stats based on level and base values.
        /// </summary>
        private void UpdateStats()
        {
            _hp = (int)(1.8f * (float)Math.Log(lvl) * BaseHp) + 10; // Calculate health points
            _mana = 50 * (int)((float)Math.Log(lvl + 0.5) * BaseMana) + 50; // Calculate mana points
            _atk = (int)((float)Math.Log(lvl) * BaseAtk); // Calculate attack power
            _speed = (int)(1.05f * Math.Log(lvl) * BaseSpeed) + 5; // Calculate speed
        }

        /// <summary>
        /// Calculates and updates the character's defense value based on equipped armor.
        /// </summary>
        private void UpdateDefense()
        {
            int newDef = 0; // Temporary variable to store new defense value
            foreach (var kvp in _equipment)
            {
                if (kvp.Value is Armor armor) // Check if the equipment is armor
                {
                    newDef += armor.Def; // Add armor's defense to total defense
                }
            }

            _def = newDef; // Update the defense field with the new value
        }

        /// <summary>
        /// Applies damage to the character, considering vulnerabilities and defense.
        /// </summary>
        /// <param name="dmg">The amount of damage to apply.</param>
        public void DealDamage(int dmg)
        {
            _hp -= IsVulnerable ? GetFinalDamage(dmg, 0) : GetFinalDamage(dmg, _def); // Adjust health based on damage
            IsALive = _hp > 0; // Check if the character is still alive
        }

        /// <summary>
        /// Calculates final damage after applying defense reduction.
        /// </summary>
        /// <param name="dmg">The initial damage value.</param>
        /// <param name="defense">The defense value to consider in damage reduction.</param>
        /// <returns>The final damage value after defense is applied.</returns>
        private int GetFinalDamage(int dmg, int defense)
        {
            return (int)(dmg * (1 / (0.01f * defense + 0.8f))); // Damage formula considering defense
        }

        /// <summary>
        /// Equips an item and updates defense if it's armor.
        /// </summary>
        /// <param name="eq">The equipment to be equipped.</param>
        public void Equip(Equipment eq)
        {
            _equipment[eq.GearType] = eq; // Add equipment to the dictionary
            if (eq is Armor) // If the equipment is armor, update defense
            {
                UpdateDefense();
            }
        }

        /// <summary>
        /// Loads a status effect or updates its duration if it already exists.
        /// </summary>
        /// <param name="se">The status effect to load.</param>
        /// <param name="duration">The duration for which the effect will be active.</param>
        public void LoadEffect(StatusEffect se, float duration)
        {
            if (_activeEffects.Keys.Contains(se)) // If effect already exists, update duration
            {
                float currentDuration = _activeEffects[se];
                _activeEffects[se] = currentDuration > duration ? currentDuration : duration;
            }
            else
            {
                _activeEffects[se] = duration; // Add new effect with duration
            }
        }

        /// <summary>
        /// Loads a technique into a specific position if not already present.
        /// </summary>
        /// <param name="tech">The technique to be loaded.</param>
        /// <param name="position">The position to insert the technique.</param>
        /// <returns>True if the technique was successfully loaded; otherwise, false.</returns>
        public bool LoadTechnique(Technique tech, int position)
        {
            if (!_techniques.Contains(tech))
            {
                _techniques[position] = tech;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a technique from a specific position.
        /// </summary>
        /// <param name="position">The position from which to remove the technique.</param>
        public void RemoveTechnique(int position)
        {
            _techniques[position] = null;
        }

        /// <summary>
        /// Casts a technique at a specific position if enough mana is available.
        /// </summary>
        /// <param name="position">The position of the technique to be cast.</param>
        public void CastAbility(int position)
        {
            if (_techniques[position] != null && _mana > _techniques[position].ManaCost)
            {
                _techniques[position].Cast(this);
                _mana -= _techniques[position].ManaCost;
            }
        }

        /// <summary>
        /// Makes the character vulnerable for a specific duration.
        /// </summary>
        /// <param name="duration">The duration of vulnerability.</param>
        public void Vulnerable(float duration)
        {
            _vulnerableDuration = duration > _vulnerableDuration ? duration : _vulnerableDuration;
        }

        /// <summary>
        /// Stuns the character for a specific duration.
        /// </summary>
        /// <param name="duration">The duration of the stun effect.</param>
        public void Stun(float duration)
        {
            _stunDuration = duration > _stunDuration ? duration : _stunDuration;
        }
    }
}
