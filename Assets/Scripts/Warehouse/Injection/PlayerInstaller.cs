using UnityEngine;
using Warehouse.Character;
using Warehouse.Character.PlayerSignals;
using Warehouse.CharacterControllers;
using Warehouse.CharacterControllers.States;
using Warehouse.Input;
using Warehouse.Models;
using Zenject;

namespace Warehouse.Injection
{
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
            DeclarePlayerSignals();
            InstallInputBindings();
            InstallMovementStateMachine();
            InstallControllerBindings();
            InstallMonoBehaviourBindings();
            InstallPlayerStatsBindings();
        }

        private void InstallMovementStateMachine()
        {
            var fsmData = new MovementStateMachineData();

            Container
                .Bind<MovementStateMachineData>()
                .FromInstance(fsmData)
                .AsSingle();
            
            Container
                .Bind<IMovementState>()
                .To<IdleState>()
                .AsSingle();
            Container
                .Bind<IMovementState>()
                .To<WalkState>()
                .AsSingle();
            Container
                .Bind<IMovementState>()
                .To<DashState>()
                .AsSingle();
            Container
                .Bind<IMovementState>()
                .To<FallState>()
                .AsSingle();
            Container
                .Bind<IMovementState>()
                .To<JumpState>()
                .AsSingle();
            Container.Bind<MovementStateMachine>().AsSingle();
        }

        private void DeclarePlayerSignals()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<PlayerEnabledSignal>().OptionalSubscriber();
            Container.DeclareSignal<PlayerDisabledSignal>().OptionalSubscriber();
        }
        
        private void InstallInputBindings()
        {
            Container
                .Bind<IInputReader>()
                .To<PlayerInputReader>()
                .AsSingle()
                .WhenInjectedInto<PlayerKnightManager>();
        }

        private void InstallControllerBindings()
        {
            Container
                .Bind<IAttackController>()
                .To<AttackController>()
                .AsTransient()
                .WhenInjectedInto<PlayerKnightManager>();
            Container
                .Bind<IStaminaController>()
                .To<StaminaController>()
                .AsTransient();
            Container
                .Bind<IMovementController>()
                .To<MovementController>()
                .AsTransient()
                .WhenInjectedInto<PlayerKnightManager>();
        }

        private void InstallPlayerStatsBindings()
        {
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

            Container
                .Bind<MovementContext>()
                .AsSingle();
        }

        private void InstallMonoBehaviourBindings()
        {
            _characterController = gameObject.GetComponent<CharacterController>();
            Container.Bind<CharacterController>()
                .FromInstance(_characterController)
                .AsSingle();
        }
    }
}