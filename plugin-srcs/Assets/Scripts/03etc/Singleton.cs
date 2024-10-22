using System;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Singleton<T> : MonoBehaviour
	where T : Singleton<T>
{
	private static T s_instance = default;

	protected virtual void Awake()
	{
		if (s_instance)
		{
			Destroy(this);
			Debug.LogWarning($"{typeof(T)}のインスタンスは既に生成されています");

			return;
		}

		s_instance = this as T;
	}
	protected virtual void OnDestroy()
	{
		if (ReferenceEquals(this, s_instance))
			s_instance = null;
	}

	public static void InvokeIfInstanceIsNotNull(Action<T> act)
	{
		if (s_instance)
			act?.Invoke(s_instance);
	}

	public static TResult InvokeIfInstanceIsNotNull<TResult>(Func<T, TResult> func)
	{
		if (!s_instance || func is null)
			return default;

		return func.Invoke(s_instance);
	}
}