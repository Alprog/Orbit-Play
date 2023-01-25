using UnityEngine;

namespace interfaces
{
    public interface ISatellite
    {
        public GameObject GameObject { get; }
        public IAttractor CurrentAttractor { get; set; }
        
        // call only from attractor script!
        public void AttachAttractor(IAttractor attractor);

        // call only from attractor script!
        public void DetachAttractor();

        public void UpdateOrbitalMovement();
    }
}