// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;

//using System.Linq;

//using NHibernate.Event;
//using NHibernate.Persister.Entity;


//namespace Hydrogen.Data.NHibernate {
//	internal class BusinessObjectSealer : IPreInsertEventListener, IPreUpdateEventListener, IPreLoadEventListener {
//		private static volatile BusinessObjectSealer _instance;
//		private static volatile object _threadLock;
//		private byte[] _sealSecret;

//		static BusinessObjectSealer() {
//			_instance = null;
//			_threadLock = new object();
//		}

//		private BusinessObjectSealer() {
//			_sealSecret = ServerSecrets.SealSecret; // cache for efficiency. Security leak? (obfuscate in memory maybe?)
//		}

//		public static BusinessObjectSealer Instance {
//			get {
//				if (_instance == null) {
//					lock (_threadLock) {
//						if (_instance == null) {
//							_instance = new BusinessObjectSealer();
//						}
//					}
//				}
//				return _instance;
//			}
//		}

//		public bool OnPreInsert(PreInsertEvent @event) {
//			var entity = @event.Entity as SealedBusinessObject;
//			if (entity != null) {
//				if (@event.Persister.HasLazyProperties)
//					throw new SoftwareException("Unable to seal entity '{0}' as has it has lazy properties", @event.Entity);
//				entity.Seal = ComputeSeal(@event.Persister, @event.State);
//				ListenerHelper.SetProperty(@event.Persister, @event.State, "Seal", entity.Seal);
//			}
//			return false;
//		}

//		public bool OnPreUpdate(PreUpdateEvent @event) {
//			var entity = @event.Entity as SealedBusinessObject;
//			if (entity != null) {
//				if (@event.Persister.HasLazyProperties)
//					throw new SoftwareException("Unable to seal entity '{0}' as has it has lazy properties", @event.Entity);
//				entity.Seal = ComputeSeal(@event.Persister, @event.State);
//				ListenerHelper.SetProperty(@event.Persister, @event.State, "Seal", entity.Seal);
//			}
//			return false;
//		}

//		public void OnPreLoad(PreLoadEvent @event) {
//			var entity = @event.Entity as SealedBusinessObject;
//			if (entity != null) {
//				if (@event.Persister.HasLazyProperties)
//					throw new SoftwareException("Unable to validate entity '{0}' seal as it has lazy properties", @event.Entity);

//				if (ComputeSeal(@event.Persister, @event.State) != (int)ListenerHelper.GetProperty(@event.Persister, @event.State, "Seal")) {
//					throw new CorruptDataException(@event.Persister.EntityName, (@event.Id ?? "NULL").ToString());
//				}
//			}
//		}


//		private int ComputeSeal(IEntityPersister persister, object[] state) {
//			var bytes = new ByteArrayBuilder();
//			foreach (var val in GatherSealableColumns(persister, state)) {
//				bytes.Append(SerializeValue(val));
//			}
//			bytes.Append(_sealSecret);
//			var checkSumCalculator = new BufferChecksumCalculator();
//			return checkSumCalculator.ComputeChecksum(bytes.ToArray());
//		}


//		private IEnumerable<object> GatherSealableColumns(IEntityPersister persister, object[] state) {
//			return
//				persister
//					.PropertyNames
//					.Zip(state, Tuple.Create)
//					.Where(t => !t.Item1.IsIn("ID", "Seal"))
//					.Where(t => !persister.PropertyTypes[ListenerHelper.FastGetEntityPropertyIndices(persister)[t.Item1]].IsComponentType)
//					.Where(t => !persister.PropertyTypes[ListenerHelper.FastGetEntityPropertyIndices(persister)[t.Item1]].IsAnyType)
//					.Where(t => !persister.PropertyTypes[ListenerHelper.FastGetEntityPropertyIndices(persister)[t.Item1]].IsAssociationType)
//					.Where(t => !persister.PropertyTypes[ListenerHelper.FastGetEntityPropertyIndices(persister)[t.Item1]].IsEntityType)
//					.Where(t => !persister.PropertyTypes[ListenerHelper.FastGetEntityPropertyIndices(persister)[t.Item1]].IsCollectionType)
//					//.ForEach(t => System.Console.WriteLine(string.Format("{0} -> {1}", t.Item1, t.Item2)))
//					.Select(t => t.Item2);
//		}


//		private byte[] SerializeValue(object obj) {
//			// NEEDS TO CONSIDER Components (force structs?)
//			// Needs to consider arrays/collections/ienumerables of primitive types
//			var reinterpret = new ReinterpretArray();

//			var result = new byte[0];
//			TypeSwitch.Do(obj ?? DBNull.Value,

//				// Null
//				TypeSwitch.Case<DBNull>(n => result = new byte[0]),

//				// Enum
//				TypeSwitch.Case<Enum>(n => result = SerializeValue(Tools.Object.ChangeType<long>(n))),

//				// Standard types (bool, numerics, enums, guids, etc)
//				TypeSwitch.Case<bool>(b => result = new byte[] { b ? (byte)1 : (byte)0 }),
//				TypeSwitch.Case<bool?>(b => result = new byte[] { b.Value ? (byte)1 : (byte)0 }),
//				TypeSwitch.Case<byte>(b => result = new byte[] { b }),
//				TypeSwitch.Case<byte?>(b => result = new byte[] { b.Value }),
//				TypeSwitch.Case<char>(n => result = EndianBitConverter.Little.GetBytes(n)),
//				TypeSwitch.Case<char?>(n => result = EndianBitConverter.Little.GetBytes(n.Value)),
//				TypeSwitch.Case<ushort>(n => result = EndianBitConverter.Little.GetBytes(n)),
//				TypeSwitch.Case<ushort?>(n => result = EndianBitConverter.Little.GetBytes(n.Value)),
//				TypeSwitch.Case<short>(n => result = EndianBitConverter.Little.GetBytes(n)),
//				TypeSwitch.Case<short?>(n => result = EndianBitConverter.Little.GetBytes(n.Value)),
//				TypeSwitch.Case<uint>(n => result = EndianBitConverter.Little.GetBytes(n)),
//				TypeSwitch.Case<uint?>(n => result = EndianBitConverter.Little.GetBytes(n.Value)),
//				TypeSwitch.Case<int>(n => result = EndianBitConverter.Little.GetBytes(n)),
//				TypeSwitch.Case<int?>(n => result = EndianBitConverter.Little.GetBytes(n.Value)),
//				TypeSwitch.Case<ulong>(n => result = EndianBitConverter.Little.GetBytes(n)),
//				TypeSwitch.Case<ulong?>(n => result = EndianBitConverter.Little.GetBytes(n.Value)),
//				TypeSwitch.Case<float>(n => result = EndianBitConverter.Little.GetBytes(n)),
//				TypeSwitch.Case<float?>(n => result = EndianBitConverter.Little.GetBytes(n.Value)),
//				TypeSwitch.Case<double>(n => result = EndianBitConverter.Little.GetBytes(n)),
//				TypeSwitch.Case<double?>(n => result = EndianBitConverter.Little.GetBytes(n.Value)),
//				TypeSwitch.Case<decimal>(n => result = SerializeValue(decimal.GetBits(Sanitize(n)))),
//				TypeSwitch.Case<decimal?>(n => result = SerializeValue(decimal.GetBits(Sanitize(n.Value)))),
//				TypeSwitch.Case<long>(n => result = EndianBitConverter.Little.GetBytes(n)),
//				TypeSwitch.Case<long?>(n => result = EndianBitConverter.Little.GetBytes(n.Value)),
//				TypeSwitch.Case<Guid>(n => result = n.ToByteArray()),
//				TypeSwitch.Case<Guid?>(n => result = n.Value.ToByteArray()),
//				TypeSwitch.Case<int[]>(n => result = n.SelectMany(EndianBitConverter.Little.GetBytes).ToArray()),

//				// String
//				TypeSwitch.Case<string>(n => result = System.Text.Encoding.UTF32.GetBytes(n.ToCharArray())),

//				// DateTime
//				TypeSwitch.Case<DateTime>(n => result = SerializeValue(Santize(n).Ticks)),
//				TypeSwitch.Case<DateTime?>(n => result = SerializeValue(Santize(n.Value).Ticks)),
//				TypeSwitch.Case<TimeSpan>(n => result = SerializeValue(Santize(n).Ticks)),
//				TypeSwitch.Case<TimeSpan?>(n => result = SerializeValue(Santize(n.Value).Ticks)),

//				// Collections/Arr
//				TypeSwitch.Case<byte[]>(n => result = n),


//				// TODO: Dealing with foreign keys?

//				// Exception Default
//				TypeSwitch.Default(() => {
//					throw new SoftwareException("Unsupported type '{0}", obj.GetType());
//				})
//				);

//			return result;
//		}


//		private DateTime Santize(DateTime dateTime) {
//			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);
//		}

//		private TimeSpan Santize(TimeSpan timeSpan) {
//			return new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, 0);
//		}

//		private decimal Sanitize(decimal dec) {
//			// Returns bit-level consistency (since 0.0M != 0.00M in bits) 
//			return 0.0000000000000000000000000000M + dec;
//		}

//	}
//}


