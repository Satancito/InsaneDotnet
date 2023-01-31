﻿using InsaneIO.Insane.Cryptography;
using System.Runtime.Versioning;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Cryptography
{
    [RequiresPreviewFeatures]
    public interface IEncoder : IJsonSerialize
    {
        public static abstract Type EncoderType { get; } 
        public string Encode(byte[] data);
        public byte[] Decode(string data);

    }
}