using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseClasses
{
    public class CharacterSheet : Savable
    {
        public int lvl;
        public bool IsALive { get; private set;}

        protected int BaseHp;
        protected int BaseMana;
        protected int BaseAtk;
        protected int BaseSpeed;

        private int _hp;
        private int _mana;
        private int _atk;
        private int _speed;
        private int _def = 0;
        
        private float _vulnerableDuration = 0.0f;
        private bool IsVulnerable { get => _vulnerableDuration > 0;}
        private float _stunDuration = 0.0f;
        private bool IsStunned { get => _stunDuration > 0; }
        
        private int _techLen;
        public int TechniquesLength
        {
            get => _techLen;
            set
            {
                if (value > _techLen)
                {
                    for (int i = 0; i < value - _techLen; i++)
                    {
                        _techniques.Add(null);
                    }
                }
                else
                {
                    _techniques = _techniques.Take(value).ToList();
                }
                _techLen = value;
            }
        }

        private List<Technique> _techniques;
        
        protected override void StartWrapper()
        {
            base.StartWrapper();
            UpdateStats();
            _techniques = new List<Technique>();
            for (int i = 0; i < _techLen; i++)
            {
                _techniques.Add(null);
            }
        }

        protected override void UpdateWrapper()
        {
            base.UpdateWrapper();
            _vulnerableDuration -= IsVulnerable ? Time.deltaTime : 0;
            _stunDuration -= IsStunned ? Time.deltaTime : 0;
        }

        public void UpdateStats()
        {
            _hp = (int)(1.8f * (float) Math.Log(lvl) * BaseHp) + 10;
            _mana = 50 * (int)((float) Math.Log(lvl + 0.5) * BaseMana) + 50;
            _atk = (int)((float) Math.Log(lvl) * BaseAtk);
            _speed = (int)(1.05f * Math.Log(lvl) * BaseSpeed) + 5;
        }

        public void DealDamage(int dmg)
        {
            _hp -= IsVulnerable ? GetFinalDamage(dmg, 0) : GetFinalDamage(dmg, _def);
            IsALive = _hp > 0;
        }

        private int GetFinalDamage(int dmg, int defense)
        {
            return (int) (dmg * (1 / (0.01f * defense + 0.8f)));
        }

        public bool LoadTechnique(Technique tech, int position)
        {
            if (!_techniques.Contains(tech))
            {
                _techniques[position] = tech;
                return true;
            }

            return false;
        }

        public void RemoveTechnique(int position)
        {
            _techniques[position] = null;
        }

        public void CastAbility(int position)
        {
            if (_techniques[position] != null && _mana > _techniques[position].ManaCost)
            {
                _techniques[position].Cast(this);
                _mana -= _techniques[position].ManaCost;
            }
        }

        public void Vulnerable(float duration)
        {
            _vulnerableDuration = duration > _vulnerableDuration ? duration : _vulnerableDuration;
        }

        public void Stun(float duration)
        {
            _stunDuration = duration > _stunDuration ? duration : _stunDuration;
        }
    }
}