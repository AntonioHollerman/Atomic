namespace BaseClasses
{
    public delegate void StatusEffect(CharacterSheet cs, float deltaTime);
    public abstract class Technique
    {
        public int ManaCost { get; protected set; }
        public abstract void Cast(CharacterSheet caster);
        public abstract void Effect(CharacterSheet cs);
        public abstract string GetIconPath();
    }
}