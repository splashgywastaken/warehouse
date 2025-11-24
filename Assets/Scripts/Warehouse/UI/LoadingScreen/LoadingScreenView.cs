using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Warehouse.UI.Warehouse.UI
{
    public class LoadingScreenView : MonoBehaviour
    {
        [Inject]
        private LoadingScreenViewModel _viewModel;
        
        [SerializeField] private Image imageToAnimate;
        [SerializeField] private Image loadingBar;

        private CancellationToken _token;
        
        private RectTransform _loadingImageTransform;
        private Sequence _loadingAnimation;

        private readonly CompositeDisposable _disposable = new ();
        
        private void Awake()
        {
            _loadingImageTransform = imageToAnimate.GetComponent<RectTransform>();
            _token = this.GetCancellationTokenOnDestroy();
            
            _loadingAnimation = DOTween.Sequence().Append(
                _loadingImageTransform
                    .DORotate(new Vector3(0, 0, -360), 3f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
            ).SetLoops(-1, LoopType.Restart).SetAutoKill(false);
        }
        
        private void Start()
        {
            _viewModel.Progress
                .Subscribe(progress => loadingBar.fillAmount = progress)
                .AddTo(_disposable);
            
            PlayLoadingAnimation(_token).Forget();
        }

        private async UniTask PlayLoadingAnimation(CancellationToken token)
        {
            _loadingAnimation.Rewind();
            await _loadingAnimation.Play().AwaitForComplete(cancellationToken: token);
        } 
        
        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}