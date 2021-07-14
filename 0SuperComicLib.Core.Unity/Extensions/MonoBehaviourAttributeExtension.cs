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