using System.Threading;
using Cysharp.Threading.Tasks;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    public interface ISceneTransitionStep
    {
        UniTask Execute(CancellationToken token);
    }
}