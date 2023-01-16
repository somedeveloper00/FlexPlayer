using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlexPlayer.Pool
{
	/// <summary>
	/// a list for <see cref="IPoolable"/> items
	/// </summary>
	public class PoolList<T> where T : IPoolable
	{
		[SerializeField] List<T> elements;
		int indexer;

		public PoolList(int size = 16) {
			elements = new List<T>( size );
		}

		public T this[int index] {
			get => elements[index];
			set => elements[index] = value;
		}

		public int Count => elements.Count;

		public T GetInactive() {
			int alpha = indexer;
			do {
				if ( !elements[indexer].IsActive() ) {
					alpha = indexer; // for returning
					indexer++; // for next time
					if ( indexer == elements.Count )
						indexer = 0;
					return elements[alpha];
				}
				indexer++;
				if ( indexer == elements.Count )
					indexer = 0;
			}
			while( alpha != indexer );

			elements.Add( (T)elements[0].Duplicate() );
			Debug.Log( $"duplicated. {string.Join( ", ", elements.Select( (e, i) => $"{i} {e.IsActive()}" ) )}" );
			indexer = 0;
			return elements[^1];
		}

		public void Dispose() {
			foreach ( var element in elements ) element.Deactivate();
		}
	}
}