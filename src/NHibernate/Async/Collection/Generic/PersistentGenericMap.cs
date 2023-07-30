﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Collection.Trackers;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection.Generic
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class PersistentGenericMap<TKey, TValue> : AbstractPersistentCollection, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ICollection
	{

		//Since 5.3
		/// <inheritdoc />
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public override Task<ICollection> GetOrphansAsync(object snapshot, string entityName, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<ICollection>(cancellationToken);
			}
			try
			{
				return Task.FromResult<ICollection>(GetOrphans(snapshot, entityName));
			}
			catch (Exception ex)
			{
				return Task.FromException<ICollection>(ex);
			}
		}

		public override async Task<bool> EqualsSnapshotAsync(ICollectionPersister persister, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IType elementType = persister.ElementType;
			var xmap = (IDictionary<TKey, TValue>)GetSnapshot();
			if (xmap.Count != WrappedMap.Count)
			{
				return false;
			}
			foreach (KeyValuePair<TKey, TValue> entry in WrappedMap)
			{
				// This method is not currently called if a key has been removed/added, but better be on the safe side.
				if (!xmap.TryGetValue(entry.Key, out var value) ||
					await (elementType.IsDirtyAsync(value, entry.Value, Session, cancellationToken)).ConfigureAwait(false))
				{
					return false;
				}
			}
			return true;
		}

		public override async Task<object> ReadFromAsync(DbDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object element = await (role.ReadElementAsync(rs, owner, descriptor.SuffixedElementAliases, Session, cancellationToken)).ConfigureAwait(false);
			object index = await (role.ReadIndexAsync(rs, descriptor.SuffixedIndexAliases, Session, cancellationToken)).ConfigureAwait(false);

			AddDuringInitialize(index, element);
			return element;
		}

		/// <summary>
		/// Initializes this PersistentGenericMap from the cached values.
		/// </summary>
		/// <param name="persister">The CollectionPersister to use to reassemble the PersistentGenericMap.</param>
		/// <param name="disassembled">The disassembled PersistentGenericMap.</param>
		/// <param name="owner">The owner object.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public override async Task InitializeFromCacheAsync(ICollectionPersister persister, object disassembled, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object[] array = (object[])disassembled;
			int size = array.Length;
			BeforeInitialize(persister, size);

			var indexType = persister.IndexType;
			var elementType = persister.ElementType;
			for (int i = 0; i < size; i += 2)
			{
				await (indexType.BeforeAssembleAsync(array[i], Session, cancellationToken)).ConfigureAwait(false);
				await (elementType.BeforeAssembleAsync(array[i + 1], Session, cancellationToken)).ConfigureAwait(false);
			}

			for (int i = 0; i < size; i += 2)
			{
				WrappedMap[(TKey)await (indexType.AssembleAsync(array[i], Session, owner, cancellationToken)).ConfigureAwait(false)] =
					(TValue)await (elementType.AssembleAsync(array[i + 1], Session, owner, cancellationToken)).ConfigureAwait(false);
			}
		}

		public override async Task<object> DisassembleAsync(ICollectionPersister persister, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object[] result = new object[WrappedMap.Count * 2];
			int i = 0;
			foreach (KeyValuePair<TKey, TValue> e in WrappedMap)
			{
				result[i++] = await (persister.IndexType.DisassembleAsync(e.Key, Session, null, cancellationToken)).ConfigureAwait(false);
				result[i++] = await (persister.ElementType.DisassembleAsync(e.Value, Session, null, cancellationToken)).ConfigureAwait(false);
			}
			return result;
		}

		public override Task<IEnumerable> GetDeletesAsync(ICollectionPersister persister, bool indexIsFormula, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<IEnumerable>(cancellationToken);
			}
			try
			{
				return Task.FromResult<IEnumerable>(GetDeletes(persister, indexIsFormula));
			}
			catch (Exception ex)
			{
				return Task.FromException<IEnumerable>(ex);
			}
		}

		public override Task<bool> NeedsInsertingAsync(object entry, int i, IType elemType, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(NeedsInserting(entry, i, elemType));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}

		public override async Task<bool> NeedsUpdatingAsync(object entry, int i, IType elemType, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var sn = (IDictionary)GetSnapshot();
			var e = (KeyValuePair<TKey, TValue>)entry;
			var snValue = sn[e.Key];
			var isNew = !sn.Contains(e.Key);
			return e.Value != null && snValue != null && await (elemType.IsDirtyAsync(snValue, e.Value, Session, cancellationToken)).ConfigureAwait(false)
				|| (!isNew && ((e.Value == null) != (snValue == null)));
		}
	}
}
