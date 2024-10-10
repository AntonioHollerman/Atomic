using UnityEngine;

namespace BaseClasses
{
    /// <summary>
    /// will run following effects every frame
    /// <param name="cs">The combative character effects will be applied on</param>
    /// <param name="deltaTime">Used to help with applying logic per second</param>
    /// </summary>
    public delegate void StatusEffect(CharacterSheet cs, float deltaTime);
    
    public abstract class Technique : MonoBehaviour
    {
        // How much implemented technique cost
        public int ManaCost { get; protected set; }
        
        /// <summary>
        /// Activates ability
        /// </summary>
        /// <param name="caster">Information on caster provided to prevent self targeting</param>
        public abstract void Cast(CharacterSheet caster);
        
        /// <summary>
        /// Apply the affects when the target is hit
        /// </summary>
        /// <param name="target">The CharacterSheet collided with</param>
        public abstract void Effect(CharacterSheet target);
        
        /// <summary>
        /// If user affected then display UI showing duration
        /// </summary>
        /// <returns>The path to the icon</returns>
        public abstract string GetIconPath();
    }
}