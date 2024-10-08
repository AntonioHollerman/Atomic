using Unity;
using BaseClasses;

namespace Implementation
{
    public class PlayerScript : CharacterSheet
    {
        protected override void StartWrapper()
        {
            base.StartWrapper();
        }

        protected override void UpdateWrapper()
        {
            base.UpdateWrapper();
            WalkCycle();
        }

        private void WalkCycle()
        {
            
        }
    }
}