using System;
using UnityEngine;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    [System.Serializable]
    public enum SceneNames
    {
        Core,
        MainMenuUI,
        LoadingScene,
        BasicGameplay,
        PlayerUI
    }
    public readonly struct SceneName : IEquatable<SceneName>
    {
        private SceneNames Value { get; }
        public SceneName(SceneNames value) => Value = value;


        public static implicit operator string(SceneName sn) => sn.Value.ToString();
        public static implicit operator SceneName(SceneNames e) => new SceneName(e);

        public bool Equals(SceneName other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is SceneName other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}