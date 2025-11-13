using System;
using Zenject;

namespace Warehouse.CharacterControllers
{
    public interface IStaminaController : IInitializable, IDisposable
    {
        public bool ConsumeStamina(PlayerStaminaStats.StaminaActions action);
        public void Enable();
        public void Disable();
    }
}
