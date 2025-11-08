using UnityEngine;
using Warehouse.Character;
using Warehouse.Input;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private PlayerMovementStats playerMovementStats;
    [SerializeField] private PlayerJumpStats playerJumpStats;
    [SerializeField] private PlayerDashStats playerDashStats;
    [SerializeField] private PlayerStaminaStats playerStaminaStats;

    private CharacterController _characterController;
    
    public override void InstallBindings()
    {
        // Input
        Container
            .Bind<IInputReader>()
            .To<PlayerInputReader>()
            .AsSingle()
            .WhenInjectedInto<PlayerKnightManager>();
        
        // Controllers
        Container
            .Bind<IMoveController>()
            .To<MovementController>()
            .AsTransient()
            .WhenInjectedInto<PlayerKnightManager>();
        
        // Stats
        Container
            .Bind<PlayerMovementStats>()
            .FromInstance(playerMovementStats)
            .AsSingle();
        Container
            .Bind<PlayerDashStats>()
            .FromInstance(playerDashStats)
            .AsSingle();
        Container
            .Bind<PlayerJumpStats>()
            .FromInstance(playerJumpStats)
            .AsSingle();
        Container
            .Bind<PlayerStaminaStats>()
            .FromInstance(playerStaminaStats)
            .AsSingle();
        
        // Components
        _characterController = gameObject.GetComponent<CharacterController>();
        Container.Bind<CharacterController>()
            .FromInstance(_characterController)
            .AsSingle()
            .WhenInjectedInto<MovementController>();
    }
}