﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen {
    //TODO
    public interface IEncrypted<TObject> {
        byte[] EncryptedBytes { get; }
        bool TryDecrypt(byte[] secret, out TObject decryptedObject);

        public bool TryDecrypt(string secret, out TObject decryptedObject)
            => TryDecrypt(Encoding.ASCII.GetBytes(secret), out decryptedObject);

    }

    public class Encrypted {
        public static Encrypted<T> For<T>(T item, byte[] secret) where T : new()
            => new(item, secret, new GenericSerializer<T>());
    }

    public sealed class Encrypted<TObject> : Encrypted, IEncrypted<TObject> {
        private IItemSerializer<TObject> _serializer;

        public Encrypted(TObject @object, byte[] secret, IItemSerializer<TObject> serializer) {
            SetItem(@object, secret);
        }

        public void SetItem(TObject item, byte[] password) {
            var itemBytes = _serializer.SerializeLE(item);
            //EncryptedBytes = Encryptor.EncryptBytes(password);
        }

        public byte[] EncryptedBytes { get; private set; }

        public bool TryDecrypt(byte[] secret, out TObject decryptedObject) {
            throw new NotImplementedException();
        }
    }
}
