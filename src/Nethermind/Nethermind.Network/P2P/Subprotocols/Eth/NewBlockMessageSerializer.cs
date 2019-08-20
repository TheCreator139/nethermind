﻿/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System.IO;
using Nethermind.Core;
using Nethermind.Core.Encoding;
using Nethermind.Core.Extensions;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.Network.P2P.Subprotocols.Eth
{
    public class NewBlockMessageSerializer : IMessageSerializer<NewBlockMessage>
    {
        private BlockDecoder _blockDecoder = new BlockDecoder();
        
        public byte[] Serialize(NewBlockMessage message)
        {
            int contentLength = _blockDecoder.GetLength(message.Block, RlpBehaviors.None) + Rlp.LengthOf((UInt256)message.TotalDifficulty);
            int totalLength = Rlp.LengthOfSequence(contentLength);
            RlpStream rlpStream = new RlpStream(totalLength);
            rlpStream.StartSequence(contentLength);
            rlpStream.Encode(message.Block);
            rlpStream.Encode((UInt256)message.TotalDifficulty);
            return rlpStream.Data;
        }
        
        public void Serialize(MemoryStream memoryStream, NewBlockMessage message)
        {
            int contentLength = _blockDecoder.GetLength(message.Block, RlpBehaviors.None) + Rlp.LengthOf((UInt256)message.TotalDifficulty);
            int totalLength = Rlp.LengthOfSequence(contentLength);
            Rlp.StartSequence(memoryStream, contentLength);
            Rlp.Encode(memoryStream, message.Block);
            Rlp.Encode(memoryStream, (UInt256)message.TotalDifficulty);
        }

        public NewBlockMessage Deserialize(byte[] bytes)
        {
            RlpStream rlpStream = bytes.AsRlpStream();
            NewBlockMessage message = new NewBlockMessage();
            rlpStream.ReadSequenceLength();
            message.Block = Rlp.Decode<Block>(rlpStream);
            message.TotalDifficulty = rlpStream.DecodeUBigInt();
            return message;
        }
    }
}