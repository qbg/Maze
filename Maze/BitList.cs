using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Maze {
	/// <summary>
	/// An IList<bool> optimized for space usage.
	/// </summary>
	public class BitList : IList<bool> {
		private long _count;
		private ulong[] _storage;
		
		/// <summary>
		/// Create a BitList of the given size with all the elements set to the defaultValue.
		/// </summary>
		/// <param name="count">The size</param>
		/// <param name="defaultValue">The default value of the elements</param>
		public BitList(long count, bool defaultValue = false) {
			if (count < 0) {
				throw new ArgumentOutOfRangeException("Count " + count + " must be at least 0");
			}
			_count = count;
			_storage = new ulong[(int)((count + 63) / 64)];
			if (defaultValue) {
				for (var i = 0; i < _storage.Length; i++) {
					_storage[i] = ~0UL;
				}
			}
		}
		
		/// <summary>
		/// Create a copy of another BitList.
		/// </summary>
		/// <param name="other">The other BitList</param>
		public BitList(BitList other) {
			_count = other._count;
			_storage = new ulong[other._storage.Length];
			other._storage.CopyTo(_storage, 0);
		}
		
		private BitList(long count, ulong[] storage) {
			_count = count;
			_storage = storage;
		}
		
		public bool this[int idx] {
			get {
				return this[(long)idx];
			}
			set {
				this[(long)idx] = value;
			}
		}
		
		public bool this[long idx] {
			get {
				AssertRange(idx);
				var major = idx / 64;
				var minor = idx - major * 64;
				var num = _storage[(int)major];
				return ((num >> (int)minor) & 1) == 1;
			}
			set {
				AssertRange(idx);
				var major = idx / 64;
				var minor = idx - major * 64;
				var num = _storage[(int)major];
				if (value) {
					num |= (1UL << (int)minor);
				} else {
					num &= ~(1UL << (int)minor);
				}
				_storage[(int)major] = num;
			}
		}
		
		private void AssertRange(long idx) {
			if (idx < 0 || idx >= _count) {
				throw new ArgumentOutOfRangeException("Index " + idx + " not within range [0," + idx + ")");
			}
		}
		
		public int Count {
			get {
				return Count;
			}
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public int IndexOf(bool item) {
			ulong skip;
			if (item) {
				skip = 0UL;
			} else {
				skip = ~0UL;
			}
			for (var i = 0; i < _storage.Length; i++) {
				var num = _storage[i];
				if (num != skip) {
					for (var j = i * 64; j < Math.Min(i * 64 + 64, Count); j++) {
						if (item == this[j]) {
							return j;
						}
					}
				}
			}
			return -1;
		}
		
		public void Insert(int index, bool item) {
			throw new InvalidOperationException("Size is immutable");
		}
		
		public void RemoveAt(int index) {
			throw new InvalidOperationException("Size is immutable");
		}
		
		public void Add(bool item) {
			throw new InvalidOperationException("Size is immutable");
		}
		
		public void Clear() {
			for (var i = 0; i < _storage.Length; i++) {
				_storage[i] = 0UL;
			}
		}
		
		public bool Contains(bool item) {
			return IndexOf(item) != -1;
		}
		
		public void CopyTo(bool[] array, int arrayIndex) {
			if ((array.Length - arrayIndex) < Count) {
				throw new ArgumentException("Insufficient space in array");
			}
			for (var i = 0; i < _count; i++) {
				array[arrayIndex + i] = this[i];
			}
		}
		
		public bool Remove(bool item) {
			throw new InvalidOperationException("Size is immutable");
		}
		
		public IEnumerator<bool> GetEnumerator() {
			return Enumerate().GetEnumerator();
		}
		
		IEnumerable<bool> Enumerate() {
			for (var i = 0; i < _count; i++) {
				yield return this[i];
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return (System.Collections.IEnumerator)GetEnumerator();
		}
		
		/// <summary>
		/// Write the BitList to a stream.
		/// </summary>
		/// <param name="stream">The stream</param>
		public void ToStream(Stream stream) {
			using (var writer = new BinaryWriter(stream, Encoding.UTF8, true)) {
				writer.Write(_count);
				for (var i = 0; i < _storage.Length; i++) {
					writer.Write(_storage[i]);
				}
			}
		}
		
		/// <summary>
		/// Read a BitList from a stream.
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <returns>The BitList</returns>
		public static BitList FromStream(Stream stream) {
			using (var reader = new BinaryReader(stream, Encoding.UTF8, true)) {
				var count = reader.ReadInt64();
				var storage = new ulong[(count + 63) / 64];
				for (var i = 0; i < storage.Length; i++) {
					storage[i] = reader.ReadUInt64();
				}
				return new BitList(count, storage);
			}
		}
	}
}
