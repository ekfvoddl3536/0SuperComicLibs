#region LICENSE
/* MIT License
 * 
 * Copyright (c) 2021 SuperComic <ekfvoddl3535@naver.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE
 */
#endregion

using System.Reflection;

namespace SuperComicWorld
{
    /// <summary>
    /// MonoBehaviour의 필드를 손쉽게 관리합니다
    /// </summary>
    public static class MonoBehaviourAttributeExtension
    {
        private const BindingFlags everything = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// 모든 필드를 열거하며 애트리뷰에 따라 액션을 취합니다
        /// </summary>
        public static void UpdateAnyFields(this UnityEngine.Component target)
        {
            var go = target.gameObject;

            var arr = target.GetType().GetFields(everything);
            
            int idx = arr.Length;

            UnityEngine.Component tmpComp;
            while (--idx >= 0)
            {
                var current = arr[idx];

                if (current.GetValue(target) == null)
                {
                    var fType = current.FieldType;

                    if (current.GetCustomAttribute<AddCompAttribute>() != null)
                        current.SetValue(target, go.AddComponent(fType));
                    else if (current.GetCustomAttribute<GetCompBaseAttribute>() is GetCompBaseAttribute getComp)
                    {
                        tmpComp = go.GetComponent(fType);

                        if (tmpComp == null && !getComp.findRootOnly)
                            tmpComp = go.GetComponentInChildren(fType);

                        if (tmpComp == null && getComp is GetOrAddCompAttribute)
                            tmpComp = go.AddComponent(fType);

                        current.SetValue(target, tmpComp);
                    }
                }

                if (current.GetCustomAttribute<ValueContractAttribute>() is ValueContractAttribute contract)
                    ValidField(current, target, contract);
            }
        }

        private static void ValidField(FieldInfo info, object target, ValueContractAttribute contract)
        {
            var ft = info.FieldType;
            if (contract.IsValidContract(ft) == false)
            {
                UnityEngine.Debug.LogWarning($"Invalid '{nameof(ValueContractAttribute)}': class: {info.DeclaringType.FullName} field: {info.Name}");
                return;
            }

            if (contract.Check(info.GetValue(target), ft) == false)
            {
                string msg =
                    contract.Message ??
                    $"Invalid field value! class: '{info.DeclaringType.FullName}'" + System.Environment.NewLine +
                    $"field: '{info.Name}' (field-value: '{info.GetValue(target)}', compare-value: '{contract.otherValue}', compare-mode: '{contract.mode}'";

                if (contract.ThrowException)
                    UnityEngine.Debug.LogException(new System.Exception(msg));
                else
                    UnityEngine.Debug.LogError(msg);
            }
        }
    }
}