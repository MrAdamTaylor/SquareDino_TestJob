using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.DI.Model;
using UnityEngine;

namespace Infrastructure.DI.Container
{
    public class GameStateLifetimeManager
    {
        private readonly Dictionary<DIGameState, List<object>> _stateInstances = new();

        public void Track(DIGameState state, object instance)
        {
            if (instance == null)
                return;

            if (!_stateInstances.TryGetValue(state, out var list))
            {
                list = new List<object>();
                _stateInstances[state] = list;
            }

            list.Add(instance);
        }

        public void Clear(DIGameState state)
        {
            if (_stateInstances.TryGetValue(state, out var list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var instance = list[i];
                    if (instance is IDisposable disposable)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Dispose error: {e}");
                        }
                    }
                }

                list.Clear();
            }
        }

        public void ClearAll()
        {
            var keys = _stateInstances.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                for (int j = 0; j < _stateInstances.Values.ElementAt(i).Count; j++)
                {
                    var instance = _stateInstances.Values.ElementAt(i).ElementAt(j);
                    CheckOnDispose(instance);
                }

                Clear(keys[i]);
            }
        }

        public bool IsGetData(Type type)
        {
            var states = _stateInstances.Keys.ToList();
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                var list = _stateInstances[state];

                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] != null && type.IsInstanceOfType(list[j]))
                        return true;
                }
            }

            return false;
        }

        public object ReturnService(Type type)
        {
            var states = _stateInstances.Keys.ToList();
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                var list = _stateInstances[state];

                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] != null && type.IsInstanceOfType(list[j]))
                        return list[j];
                }
            }

            return null;
        }

        private void CheckOnDispose(object instance)
        {
            if (instance is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Dispose error: {e}");
                }
            }
        }
    }
}