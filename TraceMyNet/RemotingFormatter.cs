namespace CodePlex.Tools.TraceMyNet.RemotingFormatter
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using CodePlex.Tools.TraceMyNet;
    using Ranamauro.Net;

    public class BinaryFormatter : ICustomHandler
    {
        public string FormatLabel(IDataPacket dataPacket)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (dataPacket.Exception != null)
            {
                stringBuilder.Append("EXC");
            }
            else if (dataPacket.Data == null || dataPacket.Data.Length == 0)
            {
                stringBuilder.Append("SHT");
            }
            else
            {
                stringBuilder.Append(dataPacket.Direction == DataDirection.Received ? "RCV" : "SND");
            }
            stringBuilder.Append(dataPacket.Direction == DataDirection.Received ? " < " : " > ");
            stringBuilder.Append(AccurateTimer.FormatAbsolute(dataPacket.TimeStamp));
            if (dataPacket.Data != null && dataPacket.Data.Length > 0)
            {
                stringBuilder.Append(" (" + dataPacket.Data.Length.ToString() + " bytes)");
            }
            return stringBuilder.ToString();
        }
        public void FormatData(IDataPacket dataPacket, RichTextBox richTextBox)
        {
        }
        public void FormatData(IDataPacket[] dataPackets, RichTextBox richTextBox)
        {
            if (dataPackets == null || dataPackets.Length == 0)
            {
                return;
            }
            richTextBox.Clear();
            ScatterGatherStream sgStream = new ScatterGatherStream();
            DataDirection dir = dataPackets[0].Direction;
            Color color = dataPackets[0].Direction == DataDirection.Received ? Color.Purple : Color.Blue;
            BinaryFormatReader bfr = new BinaryFormatReader();
            int dataCount = 0;
            for (int i = 0; i < dataPackets.Length; i++)
            {
                IDataPacket dataPacket = dataPackets[i];
                if (dataPacket.Direction != dir)
                {
                    if (dataCount > 0)
                    {
                        StringWriter writer = new StringWriter();
                        bfr.ReadAndDump(sgStream, writer);
                        richTextBox.SelectionColor = color;
                        richTextBox.AppendText(writer.ToString());
                        richTextBox.AppendText("\n----------------------------------------------\n");
                    }
                    sgStream = new ScatterGatherStream();
                    dir = dataPacket.Direction;
                    color = dir == DataDirection.Received ? Color.Purple : Color.Blue;
                    dataCount = 0;
                }
                byte[] data = dataPacket.Data;
                if (data != null && data.Length > 0)
                {
                    if (data[0] == 0x2E && data[1] == 0x4E && data[2] == 0x45 && data[3] == 0x54)
                    {
                        richTextBox.SelectionColor = color;
                        richTextBox.AppendText("\nHeader For " + dir.ToString());
                        richTextBox.AppendText("\n----------------------------------------------\n");
                    }
                    else
                    {
                        sgStream.Add(data);
                        dataCount += data.Length;
                    }
                }
            }
        }

    }

    internal class ScatterGatherStream : Stream
    {
        private const int MemStreamMaxLength = Int32.MaxValue;

        private MemoryChunk headChunk = null;
        private MemoryChunk currentChunk = null;

        private int currentOffset = 0;
        private int endOffset = 0;
        private long currentChunkStartPos = 0;

        internal ScatterGatherStream()
        {
            currentChunk = headChunk = null;
            currentOffset = endOffset = 0;
            currentChunkStartPos = 0;
        }


        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }

        public override void Close()
        {
            headChunk = null;
            currentChunk = null;
            endOffset = currentOffset = 0;
            currentChunkStartPos = 0;
        }

        public override void Flush() { }

        public override long Length
        {
            get
            {
                MemoryChunk endChunk;
                return GetLengthInternal(out endChunk);
            }
        }

        private long GetLengthInternal(out MemoryChunk endChunk)
        {
            long length = currentChunkStartPos;
            MemoryChunk chunk = currentChunk;
            while (chunk.Next != null)
            {
                length += chunk.Buffer.Length;
                chunk = chunk.Next;
            }
            length += endOffset;
            endChunk = chunk;
            return length;
        }

        public override long Position
        {
            get
            {
                return Seek(0, SeekOrigin.Current);
            }

            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }


        public override long Seek(long offset, SeekOrigin loc)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long absNewLen)
        {
            throw new NotImplementedException();
        }



        public override int Read(byte[] buffer, int offset, int count)
        {
            byte[] chunkBuffer = currentChunk.Buffer;
            int chunkSize = chunkBuffer.Length;
            if (currentChunk.Next == null)
                chunkSize = endOffset;

            int bytesRead = 0;

            while (count > 0)
            {
                if (currentOffset == chunkSize)
                {
                    // exit if no more chunks are currently available
                    if (currentChunk.Next == null)
                        break;

                    currentChunkStartPos += currentChunk.Buffer.Length;
                    currentChunk = currentChunk.Next;
                    currentOffset = 0;
                    chunkBuffer = currentChunk.Buffer;
                    chunkSize = chunkBuffer.Length;
                    if (currentChunk.Next == null)
                        chunkSize = endOffset;
                }

                int readCount = min(count, chunkSize - currentOffset);
                Buffer.BlockCopy(chunkBuffer, currentOffset, buffer, offset, readCount);
                offset += readCount;
                count -= readCount;
                currentOffset += readCount;
                bytesRead += readCount;
            }

            return bytesRead;
        }

        byte[] oneByteBuffer = new byte[1];
        public override int ReadByte()
        {
            if (Read(oneByteBuffer, 0, 1) == 1)
                return oneByteBuffer[0];
            return -1;
        }

        public void Add(byte[] buffer)
        {
            MemoryChunk lastChunk;
            if (headChunk == null)
                headChunk = currentChunk = lastChunk = new MemoryChunk();
            else
            {
                for (lastChunk = headChunk; lastChunk.Next != null; lastChunk = lastChunk.Next)
                    ;
                lastChunk = lastChunk.Next = new MemoryChunk();
            }
            lastChunk.Buffer = buffer;
            endOffset = buffer.Length;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotImplementedException();
        }


        private static int min(int a, int b) { return a < b ? a : b; }

        private class MemoryChunk
        {
            internal byte[] Buffer = null;
            internal MemoryChunk Next = null;
        }
    }
}
