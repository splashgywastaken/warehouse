using UniRx;
using Warehouse.Models;
using Zenject;

namespace Warehouse.UI.Warehouse.UI
{
    public class LoadingScreenViewModel
    {
        public readonly ReadOnlyReactiveProperty<float> Progress;
        
        [Inject]
        private LoadingScreenViewModel(LoadingSceneData loadingData)
        {
            Progress = loadingData.LoadingProgressRx.ToReadOnlyReactiveProperty();
        } 
    }
}