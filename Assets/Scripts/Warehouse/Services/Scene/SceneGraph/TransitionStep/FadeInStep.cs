using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    public class FadeInStep : ScriptableObject, ISceneTransitionStep
    {
        [Inject(Id = "fadeGroup")] 
        private readonly CanvasGroup _fadeGroup;
        private Sequence _sequence;

        [Inject]
        private void Construct(float duration)
        {
            _sequence = DOTween.Sequence()
                .Append(
                    DOTween.To(
                        () => _fadeGroup.alpha,
                        x =>
                        {
                            _fadeGroup.alpha = x;
                        },
                        1f,
                        duration
                    )
                )
                .SetAutoKill(false); 
        }
        
        public async UniTask Execute(CancellationToken token)
        {
            _sequence.Rewind();
            await _sequence.Play().AwaitForComplete(cancellationToken: token);
        }
        
        public class Factory : PlaceholderFactory<float, FadeInStep> {}
    }
}