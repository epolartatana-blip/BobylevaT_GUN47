using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks
{
	/// <summary>
	/// Обработчик пулов эффектов
	/// </summary>
	public class EffectPoolContainer : MonoBehaviour
	{
		// Словарь пулов эффектов по EntityId
		private readonly Dictionary<EntityId, EffectPool> _pools = new(16);

		public ParticleSystem this[ParticleSystem prefab] 
			=> GetPool(prefab).GetElement();

		public ParticleSystem this[EntityId id]
		{
			get
			{
#if UNITY_EDITOR
				if (_pools.TryGetValue(id, out var pool))
					return pool.GetElement();
				
				throw new ApplicationException($"Use {nameof(CreatePool)} method before this[EntityId id]!");
#else
				return _pools[id].GetElement();
#endif
			}
		}

		public EffectPool CreatePool(ParticleSystem prefab)
			=> GetPool(prefab);

		private EffectPool GetPool(ParticleSystem prefab)
		{
			if (prefab.main.stopAction != ParticleSystemStopAction.Disable)
			{
				throw new ApplicationException($"{nameof(EffectPoolContainer)}.{prefab.name} " +
					$"<i>Incorrect Effect Setting:</i> In <b>Main.StopAction</b> need <b>Disable</b> mode!");
			}
			
			var id = prefab.GetEntityId();
			if (!_pools.TryGetValue(id, out var pool))
			{
				var obj = new GameObject(string.Concat("Pool_", prefab.name));
				var tr = obj.transform;
				tr.parent = transform;
				tr.localScale = Vector3.one;
				tr.SetPositionAndRotation(new Vector3(), Quaternion.identity);
				pool = obj.AddComponent<EffectPool>();
				pool.Init(prefab);
				_pools.Add(id, pool);
			}

			return pool;
		}
	}
}
