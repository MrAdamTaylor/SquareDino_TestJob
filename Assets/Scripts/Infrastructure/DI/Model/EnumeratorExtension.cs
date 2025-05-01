using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.DI.Model
{
    public static class EnumeratorExtension 
    {
        public static object MergeCollections(object first, object second)
        {
            if (first == null) return second;
            if (second == null) return first;

            Type type = first.GetType();
    
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var addMethod = type.GetMethod("Add");
                foreach (var item in (IEnumerable)second)
                {
                    addMethod.Invoke(first, new object[] { item });
                }
        
                return first;
            }
    
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var firstDict = (IDictionary)first;
                var secondDict = (IDictionary)second;
        
                foreach (DictionaryEntry entry in secondDict)
                {
                    if (!firstDict.Contains(entry.Key))
                    {
                        firstDict[entry.Key] = entry.Value;
                    }
                }
                return firstDict;
            }
    
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
            {
                var addMethod = type.GetMethod("Add");
                foreach (var item in (IEnumerable)second)
                {
                    addMethod.Invoke(first, new object[] { item });
                }
        
                return first;
            }
    
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Queue<>))
            {
                var enqueueMethod = type.GetMethod("Enqueue");
                foreach (var item in (IEnumerable)second)
                {
                    enqueueMethod.Invoke(first, new object[] { item });
                }
        
                return first;
            }
    
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>))
            {
                var pushMethod = type.GetMethod("Push");
                List<object> tempList = new List<object>();

                foreach (var item in (IEnumerable)second)
                    tempList.Add(item);

                foreach (var item in (IEnumerable)first)
                    tempList.Add(item);

                var newStack = Activator.CreateInstance(type);
                var newStackAdd = type.GetMethod("Push");

                foreach (var item in tempList)
                    newStackAdd.Invoke(newStack, new object[] { item });
        
                return newStack;
            }
    
            if (first is IEnumerable firstEnum && second is IEnumerable secondEnum)
            {
                var mergedCollection = CreateMergedEnumerable(firstEnum, secondEnum, type);
                return mergedCollection;
            }

            Debug.LogError($"<color=red>Ошибка: Неизвестный тип коллекции {type}. Объединение не выполнено.</color>");
            return null;
        }
    
        private static object CreateMergedEnumerable(IEnumerable first, IEnumerable second, Type originalType)
        {
            Type itemType = originalType.GetGenericArguments()[0];
            Type listType = typeof(List<>).MakeGenericType(itemType);

            var mergedList = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod("Add");

            foreach (var item in first)
                addMethod.Invoke(mergedList, new object[] { item });

            foreach (var item in second)
                addMethod.Invoke(mergedList, new object[] { item });

            return mergedList;
        }

    }
}
