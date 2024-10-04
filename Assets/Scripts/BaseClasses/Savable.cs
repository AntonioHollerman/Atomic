using UnityEngine;

namespace BaseClasses
{
    public class Savable : MonoBehaviour
    {
        protected virtual void StartWrapper()
        {
            
        }

        protected virtual void UpdateWrapper()
        {
            
        }
        // Start is called before the first frame update
        void Start()
        {
            StartWrapper();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateWrapper();
        }
    }
}