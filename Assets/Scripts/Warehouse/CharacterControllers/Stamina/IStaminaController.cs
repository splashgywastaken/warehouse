using System;
using Zenject;

namespace Warehouse.CharacterControllers
{
    public interface IStaminaController : IInitializable, IDisposable
    {
        public void ConsumeStamina(PlayerStaminaStats.StaminaActions action);
        public void OnEnable();
        public void OnDisable();
    }
}
