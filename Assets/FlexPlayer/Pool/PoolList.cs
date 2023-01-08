using System.Collections.Generic;
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
			int starting_index = indexer;
			for ( ; indexer < elements.Count; indexer++ ) {
				if ( !elements[indexer].IsActive() )
					return elements[indexer++];
				if ( starting_index == indexer ) // all have been searched
					break;
				if ( indexer == elements.Count - 1 )
					indexer = 0;
			}

			elements.Add( (T)elements[0].Duplicate() );
			indexer = 0;
			return elements[^1];
		}

		public void Dispose() {
			foreach ( var element in elements ) element.Deactivate();
		}
	}
}