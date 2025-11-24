using UniRx;
using UnityEngine;

namespace Warehouse.Models
{
    [CreateAssetMenu(menuName = "Warehouse/UI/Loading data", fileName = "NewLoadingSceneData")]
    public class LoadingSceneData : ScriptableObject
    {
        public readonly ReactiveProperty<float> LoadingProgressRx = new();
        public float LoadingProgress
        {
            get => LoadingProgressRx.Value;
            set => LoadingProgressRx.Value = value;
        }
    }
}