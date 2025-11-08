using System;

namespace Warehouse.CharacterControllers
{
    public interface IStaminaController
    {
        public void ConsumeStamina(PlayerStaminaStats.StaminaActions action);
    }
}
