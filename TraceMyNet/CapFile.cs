#define WRITERS

namespace Ranamauro.Net
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;
#if WRITERS
    using CodePlex.Tools.TraceMyNet;
#endif

    public interface IPacketStreamReceiver
    {
        void AddConversation(TcpPairKey tcpPairKey, DateTime timeStamp);

        void MoveToClosedConversation(TcpPairKey tcpPairKey);

        void AddData(TcpPairKey tcpPairKey, DataDirection dataDirection, byte[] buffer, int length, int packetsReceived, Exception exception, DateTime timeStamp);
    }

    public enum DataDirection
    {
        Received,
        Sent,
    }

    public class TcpPairKey : IEquatable<TcpPairKey>
    {
        public IPEndPoint Source { get; private set; }
        public IPEndPoint Destination { get; private set; }

        public TcpPairKey(IPEndPoint source, IPEndPoint destination)
        {
            this.Source = source;
            this.Destination = destination;
        }

        public TcpPairKey(uint sourceAddress, uint destinationAddress, ushort sourcePort, ushort destinationPort)
        {
            this.Source = new IPEndPoint(new IPAddress((long)sourceAddress), (int)sourcePort);
            this.Destination = new IPEndPoint(new IPAddress((long)destinationAddress), (int)destinationPort);
        }

        public override int GetHashCode()
        {
            return (int)(this.Source.Address.GetHashCode() ^ this.Destination.Address.GetHashCode());
        }

        public override string ToString()
        {
            return this.Source.ToString() + "->" + this.Destination.ToString();
        }

        public bool Equals(TcpPairKey other)
        {
            {
                if (other == null)
                {
                    return this == null;
                }
                return
                    (this.Source.Equals(other.Source) && this.Destination.Equals(other.Destination))
                    ||
                    (this.Source.Equals(other.Destination) && this.Destination.Equals(other.Source));
            }
        }
    }

    public class CapFile
    {
        // From: Bill Zitek  
        // The defining structures are in netmon.h, which is the platform SDK include directory.
        // At the beginning of the file, is:
        // FrameTableOffset is the offset (from the beginning of the file) to the Frame 
        // table, which is simply an array of uints, in order, containing the offset to 
        // the nth frame data.
        // The number of frames is FrameTableLength/ sizeof(uint).
        // The offset points to the address of a FRAME:

        // also note that most of these are in network order,
        // so the byte ordering is reversed.

        public enum MacType : ushort
        {
            MAC_TYPE_UNKNOWN = 0,
            MAC_TYPE_ETHERNET = 1, // this is the only one we care about
            MAC_TYPE_TOKENRING = 2,
            MAC_TYPE_FDDI = 3,
            MAC_TYPE_ATM = 4,
            MAC_TYPE_1394 = 5,
        }

        [Flags]
        public enum TcpFlags : byte
        {
            UrgentData = 32, // 0x00100000
            Acknowledgment = 16, // 0x00010000
            Push = 8, // 0x00001000
            Reset = 4, // 0x00000100
            Synchronize = 2, // 0x00000010
            Fin = 1, // 0x00000001
            None = 0, // 0x00000000
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CaptureFileHeader
        {
            public uint Signature; // "GMBU" 0x47 0x4D 0x42 0x55
            public byte BcdVerMinor; // 0x00
            public byte BcdVerMajor; // 0x02
            public MacType MacType; // 0x01
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Milliseconds;
            public uint FrameTableOffset;
            public uint FrameTableLength;
            public uint UserDataOffset;
            public uint UserDataLength;
            public uint CommentDataOffset;
            public uint CommentDataLength;
            public uint StatisticsOffset;
            public uint StatisticsLength;
            public uint NetworkInfoOffset;
            public uint NetworkInfoLength;
            public uint ConversationStatsOffset;
            public uint ConversationStatsLength;

            public bool Is20AndUp
            {
                get
                {
                    return BcdVerMajor >= 2;
                }
            }

            public DateTime TimeStamp
            {
                get
                {
                    return new DateTime((int)Year, (int)Month, (int)Day, (int)Hour, (int)Minute, (int)Second, (int)Milliseconds);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FrameHeader
        {
            public long TimeStamp;
            public uint Length;
            public uint BytesAvailable;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FrameHeader11
        {
            public ushort TimeStamp;
            public ushort TimeStamp2;
            public ushort Length;
            public ushort BytesAvailable;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EthernetHeader
        {
            public byte DestinationAddress0;
            public byte DestinationAddress1;
            public byte DestinationAddress2;
            public byte DestinationAddress3;
            public byte DestinationAddress4;
            public byte DestinationAddress5;

            public byte SourceAddress0;
            public byte SourceAddress1;
            public byte SourceAddress2;
            public byte SourceAddress3;
            public byte SourceAddress4;
            public byte SourceAddress5;

            public ushort Type;

            bool IsEthernet
            {
                get
                {
                    return Type == (ushort)0x0008;
                }
            }

            public bool IsSupported
            {
                get
                {
                    return IsEthernet;
                }
            }
        }

        //
        //                          IP Frame Structure  
        //   
        //     0       4       8               16              24
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |Version|HdrLen |Type of Service|         Total Length          | 
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |      Identification           |Flags|     Fragment Offset     |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |  Time to Live |    Protocol   |         Header Checksum       |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |                        Source Address                         |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |                      Destination Address                      |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |               Options                         |    Padding    |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IpHeader
        {
            public byte VersionAndHeaderLength; // how are these split?
            public byte ServiceType;
            public ushort TotalLength;
            public ushort Identification;
            public ushort FlagsAndFragmentOffset; // how are these split?
            public byte TimeToLive;
            public byte Protocol; // = 0x06-(TCP) 0x32-(IPSEC)
            public ushort CheckSum;
            public uint SourceAddress;
            public uint DestinationAddress;

            public byte Version
            {
                get
                {
                    return (byte)(VersionAndHeaderLength >> 4);
                }
            }

            public byte HeaderLength
            {
                get
                {
                    return (byte)(VersionAndHeaderLength & 0x0F);
                }
            }

            public ushort Flags
            {
                get
                {
                    return (ushort)(FlagsAndFragmentOffset >> 13);
                }
            }

            public ushort FragmentOffset
            {
                get
                {
                    return (ushort)(FlagsAndFragmentOffset & 0x01FFF);
                }
            }

            public bool IsIPSec
            {
                get
                {
                    return Protocol == (byte)0x32;
                }
            }

            public bool IsSupported
            {
                get
                {
                    return Protocol == (byte)0x32 || Protocol == (byte)0x06;
                }
            }

            public void InvertByteOrder()
            {
                TotalLength = (ushort)IPAddress.HostToNetworkOrder(unchecked((short)TotalLength));
                Identification = (ushort)IPAddress.HostToNetworkOrder(unchecked((short)Identification));
                FlagsAndFragmentOffset = (ushort)IPAddress.HostToNetworkOrder(unchecked((short)FlagsAndFragmentOffset));
                CheckSum = (ushort)IPAddress.HostToNetworkOrder(unchecked((short)CheckSum));
            }
        }

        //
        //                          TCP Frame Structure  
        //   
        //     0       4       8               16              24
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |          Source Port          |       Destination Port        |   
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |                       Sequence Number                         |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |                   Acknowledgement Number                      |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ 
        //    |DataOff| Reserved  |U A P R S F|            Window             |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |          Checksum             |         Urgent Pointer        |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //    |               Options                         |    Padding    |
        //    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TcpHeader
        {
            public ushort SourcePort;
            public ushort DestinationPort;
            public uint SequenceNumber;
            public uint AcknowledgementNumber;
            public byte DataOffsetX;
            public TcpFlags Flags;
            public ushort Window;
            public ushort CheckSum;
            public ushort UrgentPointer;

            public byte DataOffset
            {
                get
                {
                    return (byte)(DataOffsetX >> 2);
                }
            }

            public void InvertByteOrder()
            {
                SourcePort = (ushort)IPAddress.HostToNetworkOrder(unchecked((short)SourcePort));
                DestinationPort = (ushort)IPAddress.HostToNetworkOrder(unchecked((short)DestinationPort));
                SequenceNumber = (uint)IPAddress.HostToNetworkOrder(unchecked((int)SequenceNumber));
                AcknowledgementNumber = (uint)IPAddress.HostToNetworkOrder(unchecked((int)AcknowledgementNumber));
                Window = (ushort)IPAddress.HostToNetworkOrder(unchecked((short)Window));
                CheckSum = (ushort)IPAddress.HostToNetworkOrder(unchecked((short)CheckSum));
                UrgentPointer = (ushort)IPAddress.HostToNetworkOrder(unchecked((short)UrgentPointer));
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EspHeader
        {
            public byte EspHeader0;
            public byte EspHeader1;
            public byte EspHeader2;
            public byte EspHeader3;
            public byte EspHeader4;
            public byte EspHeader5;
            public byte EspHeader6;
            public byte EspHeader7;
        }

        public static byte[] ToByteArray(object thisObject)
        {
            GCHandle gcHandle = GCHandle.Alloc(thisObject, GCHandleType.Pinned);
            byte[] managedBuffer = new byte[Marshal.SizeOf(thisObject)];
            Marshal.Copy(gcHandle.AddrOfPinnedObject(), managedBuffer, 0, managedBuffer.Length);
            gcHandle.Free();
            return managedBuffer;
        }

#if WRITERS
        public static void WriteToFile(TreeView treeView, string filename)
        {
            if (!filename.EndsWith(".cap"))
            {
                WriteToTxtFile(treeView, filename);
            }
            else
            {
                WriteToCapFile(treeView, filename);
            }
        }

        public static void WriteToTxtFile(TreeView treeView, string filename)
        {
            // create the file
            FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file);

            // enumerate all captured frames
            writer.WriteLine("ConnectionNodes: " + treeView.Nodes.Count.ToString());
            ConnectionNode connectionNode = null;
            for (int i = 0; i < treeView.Nodes.Count; i++)
            {
                connectionNode = treeView.Nodes[i] as ConnectionNode;

                writer.WriteLine("AcceptedEndPoint: " + connectionNode.AcceptedEndPoint.ToString());
                writer.WriteLine("ConnectedEndPoint: " + connectionNode.ConnectedEndPoint.ToString());
                writer.WriteLine("TimeStamp: " + connectionNode.TimeStamp.Ticks.ToString());

                writer.WriteLine("BufferNodes: " + connectionNode.Nodes.Count.ToString());
                BufferNode bufferNode = null;
                for (int j = 0; j < connectionNode.Nodes.Count; j++)
                {
                    bufferNode = connectionNode.Nodes[j] as BufferNode;

                    // write captured frame to the file
                    writer.WriteLine("FrameNumber: " + bufferNode.FrameNumber.ToString());
                    writer.WriteLine("TimeStamp: " + bufferNode.TimeStamp.Ticks.ToString());
                    writer.WriteLine("DataDirection: " + bufferNode.Direction.ToString());
                    writer.WriteLine("Exception: " + (bufferNode.Exception == null ? "0" : bufferNode.Exception.ToString()));
                    if (bufferNode.Data == null || bufferNode.Data.Length == 0)
                    {
                        writer.WriteLine("Data: 0");
                    }
                    else
                    {
                        writer.WriteLine("Data: " + bufferNode.Data.Length.ToString());
                        string data = DefaultHandler.FormatBinary(bufferNode.Data);
                        writer.WriteLine(data);
                    }
                }
            }
            // done.
            writer.Flush();
            file.Flush();
            writer.Close();
            file.Close();
        }

        public static void WriteToCapFile(TreeView treeView, string filename)
        {
            // create the file
            FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(file);

            // create CaptureFileHeader and write it to the file
            // NYI

            // enumerate all captured frames
            ConnectionNode connectionNode = null;
            for (int i = 0; i < treeView.Nodes.Count; i++)
            {
                connectionNode = treeView.Nodes[i] as ConnectionNode;
                BufferNode bufferNode = null;
                for (int j = 0; j < connectionNode.Nodes.Count; j++)
                {
                    bufferNode = connectionNode.Nodes[j] as BufferNode;
                    // write captured frame to the file
                    // NYI
                }
            }
            // done.
            writer.Close();
            file.Close();
        }
#endif

        public static T Read<T>(Stream stream) where T : new()
        {
            byte[] thisByteArray = new byte[Marshal.SizeOf(typeof(T))];
            stream.Read(thisByteArray, 0, thisByteArray.Length);
            object thisObject = new T();
            GCHandle gcHandle = GCHandle.Alloc(thisObject, GCHandleType.Pinned);
            Marshal.Copy(thisByteArray, 0, gcHandle.AddrOfPinnedObject(), thisByteArray.Length);
            gcHandle.Free();
            return (T)thisObject;
        }

        public static void ToObject(byte[] thisByteArray, ref object thisObject)
        {
            if (thisByteArray.Length != Marshal.SizeOf(thisObject))
            {
                throw new InvalidOperationException();
            }
            GCHandle gcHandle = GCHandle.Alloc(thisObject, GCHandleType.Pinned);
            Marshal.Copy(thisByteArray, 0, gcHandle.AddrOfPinnedObject(), thisByteArray.Length);
            gcHandle.Free();
        }

        public static object Read(Stream stream, Type type)
        {
            object thisObject =
                Activator.CreateInstance(
                    type,
                    BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null, // no binder
                new object[0], // no arguments
                CultureInfo.InvariantCulture);
            byte[] buffer = new byte[Marshal.SizeOf(thisObject)];
            stream.Read(buffer, 0, buffer.Length);
            ToObject(buffer, ref thisObject);
            return thisObject;
        }

        public static string GetParameter(StreamReader reader, string beginning)
        {
            string whole = reader.ReadLine();
            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("GetParameter: whole: " + whole + " beginning:" + beginning);
            if (whole != null && whole.Length > beginning.Length + 2 && whole.StartsWith(beginning) && whole[beginning.Length] == ':' && whole[beginning.Length + 1] == ' ')
            {
                return whole.Substring(beginning.Length + 2);
            }
            return null;
        }

        public static IPEndPoint IPEndPointParse(string tostring)
        {
            int i = tostring.IndexOf(':');
            return new IPEndPoint(IPAddress.Parse(tostring.Substring(0, i)), int.Parse(tostring.Substring(i + 1)));
        }

        static int HexToInt(char h)
        {
            return (h >= '0' && h <= '9') ? h - '0' : (h >= 'a' && h <= 'f') ? h - 'a' + 10 : (h >= 'A' && h <= 'F') ? h - 'A' + 10 : -1;
        }

        public static void ReadFromFile(IPacketStreamReceiver traceMyNet, string filename)
        {
            if (!filename.EndsWith(".cap"))
            {
                ReadFromTxtFile(traceMyNet, filename);
            }
            else
            {
                ReadFromCapFile(traceMyNet, filename);
            }
        }

        public static void ReadFromTxtFile(IPacketStreamReceiver traceMyNet, string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fileStream);

            string parameter = null;
            int packetsReceived = 0;
            try
            {
                // write captured frame to the file
                parameter = GetParameter(reader, "ConnectionNodes");
                int connectionNodes = int.Parse(parameter);
                for (int i = 0; i < connectionNodes; i++)
                {
                    parameter = GetParameter(reader, "AcceptedEndPoint");
                    IPEndPoint acceptedEndPoint = IPEndPointParse(parameter);
                    parameter = GetParameter(reader, "ConnectedEndPoint");
                    IPEndPoint connectedEndPoint = IPEndPointParse(parameter);
                    parameter = GetParameter(reader, "TimeStamp");
                    DateTime timeStamp = new DateTime(long.Parse(parameter));

                    TcpPairKey tpk = new TcpPairKey(connectedEndPoint, acceptedEndPoint);
                    traceMyNet.AddConversation(tpk, timeStamp);

                    parameter = GetParameter(reader, "BufferNodes");
                    int bufferNodes = int.Parse(parameter);
                    for (int j = 0; j < bufferNodes; j++)
                    {
                        parameter = GetParameter(reader, "FrameNumber");
                        int frameNumber = int.Parse(parameter);
                        parameter = GetParameter(reader, "TimeStamp");
                        DateTime timeStamp2 = new DateTime(long.Parse(parameter));
                        parameter = GetParameter(reader, "DataDirection");
                        DataDirection dataDirection = (DataDirection)Enum.Parse(typeof(DataDirection), parameter);
                        string exception = GetParameter(reader, "Exception");
                        if (exception == "0")
                        {
                            exception = null;
                        }
                        parameter = GetParameter(reader, "Data");
                        int length = int.Parse(parameter);
                        byte[] buffer = null;
                        if (length > 0)
                        {
                            buffer = new byte[length];
                            int charOffset = 11;
                            for (int offset = 0; offset < length; offset++)
                            {
                                if (offset % 16 == 0)
                                {
                                    charOffset = 11;
                                    parameter = reader.ReadLine();
                                }
                                buffer[offset] = (byte)(HexToInt(parameter[charOffset]) * 16 + HexToInt(parameter[charOffset + 1]));
                                charOffset += 3;
                            }
                        }
                        traceMyNet.AddData(tpk, dataDirection, buffer, length, packetsReceived++, null, timeStamp2);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("WARNING: Possible corruption!\r\n\r\nCaught the following error while parsing:\r\n\r\n" + ex.ToString());
            }
        }

        public class PacketStream
        {
            CaptureFileHeader captureFileHeader;
            bool fromCapFile;
            IPacketStreamReceiver receiver;

            Dictionary<TcpPairKey, MyConversation> conversations = new Dictionary<TcpPairKey, MyConversation>();

            public PacketStream(IPacketStreamReceiver receiver)
            {
                this.receiver = receiver;
                this.fromCapFile = false;
            }

            public PacketStream(IPacketStreamReceiver receiver, CaptureFileHeader captureFileHeader)
            {
                this.receiver = receiver;
                this.fromCapFile = true;
                this.captureFileHeader = captureFileHeader;

                if (TraceMyNet.Debug)
                {
                    TraceMyNet.LogFile.WriteLine("== captureFileHeader ==");
                    TraceMyNet.LogFile.WriteLine("FrameTableLength: " + this.captureFileHeader.FrameTableLength);
                    TraceMyNet.LogFile.WriteLine("FrameTableLength(actual): " + ((int)this.captureFileHeader.FrameTableLength) / 4);
                    TraceMyNet.LogFile.WriteLine("FrameTableOffset: " + this.captureFileHeader.FrameTableOffset);
                    TraceMyNet.LogFile.WriteLine("TimeStamp: " + this.captureFileHeader.TimeStamp.ToString("R") + ":" + this.captureFileHeader.TimeStamp.ToString());
                    TraceMyNet.LogFile.WriteLine("TimeStamp.Ticks: " + this.captureFileHeader.TimeStamp.Ticks.ToString());
                    TraceMyNet.LogFile.Flush();
                }
            }

            public void Process(Stream stream, int offset, int received, int frame, bool is20AndUp)
            {
                stream.Seek(offset, SeekOrigin.Begin);
                FrameHeader frameHeader;
                if (is20AndUp)
                {
                    frameHeader = Read<FrameHeader>(stream);
                }
                else
                {
                    frameHeader = new FrameHeader();
                    FrameHeader11 frameHeader11 = Read<FrameHeader11>(stream);
                    frameHeader.BytesAvailable = frameHeader11.BytesAvailable;
                    frameHeader.Length = frameHeader11.Length;
                    frameHeader.TimeStamp = frameHeader11.TimeStamp;
                }

                long offset2 = stream.Position;
                IpHeader ipHeader = new IpHeader { SourceAddress = 0, DestinationAddress = 0 };

                if (TraceMyNet.Debug)
                {
                    TraceMyNet.LogFile.WriteLine("== frameHeader ==");
                    TraceMyNet.LogFile.WriteLine("  TimeStamp: 0x" + frameHeader.TimeStamp.ToString("X") + " Sec: " + frameHeader.TimeStamp.ToString());
                    TraceMyNet.LogFile.WriteLine("  Length: 0x" + frameHeader.Length.ToString("X") + " Sec: " + frameHeader.Length.ToString());
                    TraceMyNet.LogFile.WriteLine("  BytesAvailable: 0x" + frameHeader.BytesAvailable.ToString("X") + " Sec: " + frameHeader.BytesAvailable.ToString());
                    TraceMyNet.LogFile.Flush();
                }

                bool isEmpty = false;
                bool isWiFiHack = false;
                EthernetHeader ethernetHeader = Read<EthernetHeader>(stream);
                if (TraceMyNet.Debug)
                {
                    TraceMyNet.LogFile.WriteLine("  == ethernetHeader ==");
                    TraceMyNet.LogFile.WriteLine("    IsSupported: " + ethernetHeader.IsSupported.ToString());
                    TraceMyNet.LogFile.WriteLine("    Type: 0x" + ethernetHeader.Type.ToString("X"));
                    TraceMyNet.LogFile.Flush();
                }
                if (!ethernetHeader.IsSupported)
                {
                    string wince = null;
                    if (frameHeader.BytesAvailable > 72 + Marshal.SizeOf(typeof(TcpHeader)))
                    {
                        // try wincewifi workaround
                        stream.Seek(offset2 + 3, SeekOrigin.Begin);
                        byte[] thisByteArray = new byte[17];
                        stream.Read(thisByteArray, 0, thisByteArray.Length);
                        wince = Encoding.ASCII.GetString(thisByteArray);
                    }
                    if (!"wincewifimetadata".Equals(wince))
                    {
                        // try wincewifi workaround
                        stream.Seek(offset2 + 0x3e, SeekOrigin.Begin);
                        byte[] thisByteArray = new byte[2];
                        stream.Read(thisByteArray, 0, thisByteArray.Length);
                        ushort type = BitConverter.ToUInt16(thisByteArray, 0);
                        if (type != 8)
                        {
                            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("*** Skipping: ethernetHeader is not supported.");
                            return;
                        }
                        isWiFiHack = true;
                        stream.Seek(offset2 + 84, SeekOrigin.Begin);
                        if (frameHeader.BytesAvailable == 104)
                        {
                            isEmpty = true;
                        }
                    }
                    else
                    {
                        stream.Seek(offset2 + 72, SeekOrigin.Begin);
                    }
                }
                else
                {
                    ipHeader = Read<IpHeader>(stream);
                    ipHeader.InvertByteOrder();
                    if (TraceMyNet.Debug)
                    {
                        TraceMyNet.LogFile.WriteLine("    == ipHeader ==");
                        TraceMyNet.LogFile.WriteLine("      IsSupported: " + ipHeader.IsSupported.ToString());
                        TraceMyNet.LogFile.WriteLine("      IsIPSec: " + ipHeader.IsIPSec.ToString());
                        TraceMyNet.LogFile.WriteLine("      VersionAndHeaderLength: 0x" + ipHeader.VersionAndHeaderLength.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      -Version: 0x" + ipHeader.Version.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      -HeaderLength: 0x" + ipHeader.HeaderLength.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      ServiceType: 0x" + ipHeader.ServiceType.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      TotalLength: 0x" + ipHeader.TotalLength.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      Identification: 0x" + ipHeader.Identification.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      FlagsAndFragmentOffset: 0x" + ipHeader.FlagsAndFragmentOffset.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      -Flags: 0x" + ipHeader.Flags.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      -FragmentOffset: 0x" + ipHeader.FragmentOffset.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      TimeToLive: 0x" + ipHeader.TimeToLive.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      Protocol: 0x" + ipHeader.Protocol.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      CheckSum: 0x" + ipHeader.CheckSum.ToString("X"));
                        TraceMyNet.LogFile.WriteLine("      SourceAddress: 0x" + ipHeader.SourceAddress.ToString("X") + " " + (new IPAddress(ipHeader.SourceAddress)).ToString());
                        TraceMyNet.LogFile.WriteLine("      DestinationAddress: 0x" + ipHeader.DestinationAddress.ToString("X") + " " + (new IPAddress(ipHeader.DestinationAddress)).ToString());
                        TraceMyNet.LogFile.Flush();
                    }
                    if (!ipHeader.IsSupported)
                    {
                        if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("*** Skipping: ipHeader is not supported.");
                        return;
                    }

                    if (ipHeader.IsIPSec)
                    {
                        EspHeader espHeader = Read<EspHeader>(stream);
                    }
                }

                TcpHeader tcpHeader = Read<TcpHeader>(stream);
                tcpHeader.InvertByteOrder();
                if (TraceMyNet.Debug)
                {
                    TraceMyNet.LogFile.WriteLine("      == tcpHeader @" + offset + " ==");
                    TraceMyNet.LogFile.WriteLine("        SourcePort: 0x" + tcpHeader.SourcePort.ToString("X4"));
                    TraceMyNet.LogFile.WriteLine("        DestinationPort: 0x" + tcpHeader.DestinationPort.ToString("X4"));
                    TraceMyNet.LogFile.WriteLine("        SequenceNumber: 0x" + tcpHeader.SequenceNumber.ToString("X"));
                    TraceMyNet.LogFile.WriteLine("        AcknowledgementNumber: 0x" + tcpHeader.AcknowledgementNumber.ToString("X"));
                    TraceMyNet.LogFile.WriteLine("        DataOffsetX: 0x" + tcpHeader.DataOffsetX.ToString("X"));
                    TraceMyNet.LogFile.WriteLine("        -DataOffset: 0x" + tcpHeader.DataOffset.ToString("X"));
                    TraceMyNet.LogFile.WriteLine("        Flags: 0x" + tcpHeader.Flags.ToString("X") + " " + tcpHeader.Flags.ToString());
                    TraceMyNet.LogFile.WriteLine("        Window: 0x" + tcpHeader.Window.ToString("X"));
                    TraceMyNet.LogFile.WriteLine("        CheckSum: 0x" + tcpHeader.CheckSum.ToString("X"));
                    TraceMyNet.LogFile.WriteLine("        UrgentPointer: 0x" + tcpHeader.UrgentPointer.ToString("X"));
                    TraceMyNet.LogFile.Flush();
                }
                if (tcpHeader.DataOffset == 0)
                {
                    return;
                }

                if (!fromCapFile)
                {
                    frameHeader.Length = (ushort)received;
                    ethernetHeader.Type = (ushort)8;
                }

                // read TCP data from the file
                byte[] buffer = null;
                int length = 0;
                if (!isEmpty)
                {
                    length = ((int)(frameHeader.Length)) - tcpHeader.DataOffset - 34;
                }
                if (!fromCapFile)
                {
                    length += 14; // add back the 14 from Ethernet
                }
                if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("frame#:" + frame.ToString() + " length: " + length + " flags:" + tcpHeader.Flags);

                if (length > 0)
                {
                    if (tcpHeader.DataOffset > 20)
                    {
                        // skip TCP Options (and Padding) section
                        stream.Seek(tcpHeader.DataOffset - 20, SeekOrigin.Current);
                    }
                    if (isWiFiHack)
                    {
                        length -= 50;
                    }
                    buffer = new byte[length];
                    stream.Read(buffer, 0, length);
                    if (length == 6)
                    {
                        // hack!!! here we should skip the Options and the Padding
                        if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0 && buffer[3] == 0 && buffer[4] == 0 && buffer[5] == 0)
                        {
                            buffer = null;
                            length = 0;
                        }
                    }
                }
                if ((tcpHeader.Flags & TcpFlags.Synchronize) != 0 || ((tcpHeader.Flags & TcpFlags.Reset) != 0))
                {
                    buffer = null;
                    length = 0;
                }

                TcpPairKey tcpPairKey = new TcpPairKey(ipHeader.SourceAddress, ipHeader.DestinationAddress, tcpHeader.SourcePort, tcpHeader.DestinationPort);
                MyConversation conversation = null;
                conversations.TryGetValue(tcpPairKey, out conversation);
                DateTime timeStamp;

                if (fromCapFile)
                {
                    double ms = frameHeader.TimeStamp / 1000.0;
                    timeStamp = captureFileHeader.TimeStamp.AddMilliseconds(ms);
                }
                else
                {
                    timeStamp = AccurateTimer.GetTimeStamp();
                }

                if (conversation != null)
                {
                    if ((tcpHeader.Flags & TcpFlags.Synchronize) != 0 && (tcpHeader.Flags & TcpFlags.Acknowledgment) == 0)
                    {
                        if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("SYN packet on existing connection, this is a new one");
                        this.receiver.MoveToClosedConversation(tcpPairKey);
                        conversations.Remove(tcpPairKey);
                        conversation = null;
                    }
                    else
                    {
                        DataDirection dataDirection = conversation.AcceptedEndPoint.Equals(tcpPairKey.Source) ? DataDirection.Sent : DataDirection.Received;
                        if ((tcpHeader.Flags & TcpFlags.Reset) != 0)
                        {
                            if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("adding new RST packet to tcpPairKey:" + tcpPairKey.ToString());
                            conversation.Add(dataDirection, null, 0, frame, new SocketException(10054), timeStamp);
                            this.receiver.AddData(tcpPairKey, dataDirection, null, 0, frame, new SocketException(10054), timeStamp);
                        }
                        else
                        {
                            if (length > 0)
                            {
                                if (length <= 28 && ipHeader.IsIPSec)
                                {
                                    if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("ignore packet smaller than 28 bytes on ipSec");
                                }
                                else
                                {
                                    if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("adding new packet to tcpPairKey:" + tcpPairKey.ToString());
                                    conversation.Add(dataDirection, buffer, length, frame, null, timeStamp);
                                    this.receiver.AddData(tcpPairKey, dataDirection, buffer, length, frame, null, timeStamp);
                                }
                            }
                            if ((tcpHeader.Flags & TcpFlags.Fin) == TcpFlags.Fin)
                            {
                                if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("adding new FIN packet to tcpPairKey:" + tcpPairKey.ToString());
                                conversation.Add(dataDirection, null, 0, frame, null, timeStamp);
                                this.receiver.AddData(tcpPairKey, dataDirection, buffer, length, frame, null, timeStamp);
                            }
                        }
                    }
                }
                if (conversation == null && (((tcpHeader.Flags & TcpFlags.Synchronize) != 0 && (tcpHeader.Flags & TcpFlags.Acknowledgment) == 0) || length > 0))
                {
                    if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("creating new ConnectionNode for tcpPairKey:" + tcpPairKey.ToString());
                    this.receiver.AddConversation(tcpPairKey, timeStamp);
                    conversation = new MyConversation(tcpPairKey.Source, tcpPairKey.Destination, timeStamp);
                    conversations.Add(tcpPairKey, conversation);
                }
            }

            public class MyConversation
            {
                public IPEndPoint AcceptedEndPoint { get; private set; }

                IPEndPoint connectedEndPoint;
                DateTime timeStamp;

                public MemoryStream client = new MemoryStream();
                public MemoryStream server = new MemoryStream();

                public MyConversation(IPEndPoint connectedEndPoint, IPEndPoint acceptedEndPoint, DateTime timeStamp)
                {
                    this.connectedEndPoint = connectedEndPoint;
                    this.AcceptedEndPoint = acceptedEndPoint;
                    this.timeStamp = timeStamp;
                }

                public void Add(DataDirection dataDirection, byte[] buffer, int length, int packetsReceived, Exception exception, DateTime timeStamp)
                {
                    if (buffer != null && length > 0)
                    {
                        if (dataDirection == DataDirection.Received)
                        {
                            client.Write(buffer, 0, length);
                        }
                        else
                        {
                            server.Write(buffer, 0, length);
                        }
                    }
                }
            }
        }

        public static void ReadFromCapFile(IPacketStreamReceiver traceMyNet, string filename)
        {
            // open the file and dump it to a MemoryStream
            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            MemoryStream stream = new MemoryStream((int)fileStream.Length);
            Pump(fileStream, stream);
            fileStream.Close();

            // read CaptureFileHeader from the file
            stream.Seek(0, SeekOrigin.Begin);
            CaptureFileHeader captureFileHeader = Read<CaptureFileHeader>(stream);
            int frames = ((int)captureFileHeader.FrameTableLength) / 4;
            PacketStream ps = new PacketStream(traceMyNet, captureFileHeader);
            for (int packetsReceived = 0; packetsReceived < frames; packetsReceived++)
            {
                // read offset of current header from the file
                int frameOffset = ((int)captureFileHeader.FrameTableOffset) + packetsReceived * 4;
                stream.Seek(frameOffset, SeekOrigin.Begin);
                int offset = Read<int>(stream);
                if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("----- frame [" + (packetsReceived + 1) + "/" + frames + "], frameOffset: " + frameOffset + ", offset : " + offset);
                ps.Process(stream, offset, 0, packetsReceived + 1, captureFileHeader.Is20AndUp);
            }
            stream.Close();
        }

        /// <summary>
        /// Writes one stream (starting from the current position) into 
        /// an output stream, connecting them up and reading until 
        /// hitting the end of the input stream.  
        /// </summary>
        /// <param name="input">Stream to read from</param>
        /// <param name="output">Stream to write to</param>
        /// <returns>Number of bytes copied</returns>
        public static long Pump(Stream input, Stream output)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (output == null) throw new ArgumentNullException("output");
            if (input.CanSeek) input.Seek(0, SeekOrigin.Begin);
            if (output.CanSeek) output.Seek(0, SeekOrigin.Begin);

            long totalBytes = 0;

            // Use MemoryStream's WriteTo(Stream) method if possible
            MemoryStream inputMS = input as MemoryStream;
            if (inputMS != null && inputMS.Position == 0)
            {
                inputMS.WriteTo(output);
                totalBytes = inputMS.Length;
            }
            else
            {
                const int count = 4096;
                byte[] bytes = new byte[count];
                int numBytes;
                while ((numBytes = input.Read(bytes, 0, count)) > 0)
                {
                    output.Write(bytes, 0, numBytes);
                    totalBytes += numBytes;
                }
            }

            if (input.CanSeek) input.Seek(0, SeekOrigin.Begin);
            if (output.CanSeek) output.Seek(0, SeekOrigin.Begin);

            return totalBytes;
        }

        static bool IsManaged(string filename)
        {
            try
            {
                byte[] data = new byte[4096];
                FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                int iRead = fileStream.Read(data, 0, 4096);
                fileStream.Close();
                // Verify this is a executable/dll
                if ((data[1] << 8 | data[0]) != 0x5a4d)
                {
                    return false;
                }
                // This will get the address for the WinNT header
                int iWinNTHdr = data[63] << 24 | data[62] << 16 | data[61] << 8 | data[60];
                // Verify this is an NT address
                if ((data[iWinNTHdr + 3] << 24 | data[iWinNTHdr + 2] << 16 | data[iWinNTHdr + 1] << 8 | data[iWinNTHdr]) != 0x00004550)
                {
                    return false;
                }
                int iLightningAddr = iWinNTHdr + 24 + 208;
                int iSum = 0;
                int iTop = iLightningAddr + 8;
                for (int i = iLightningAddr; i < iTop; i++)
                {
                    iSum |= data[i];
                }
                return iSum != 0;
            }
            catch
            {
                return false;
            }
        }

        public class PacketSniffer
        {
            Socket listenerSocket;
            int packetsReceived;
            EndPoint remoteEP;
            byte[] data = new byte[10240];
            PacketStream packetStream;
            AsyncCallback processData;

            const int skip = 16 + 14;

            public PacketSniffer(IPacketStreamReceiver receiver)
            {
                packetStream = new PacketStream(receiver);
                processData = new AsyncCallback(ProcessData);
            }

            public void Start()
            {
                try
                {
                    listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                    IPHostEntry host = Dns.GetHostEntry(string.Empty);
                    IPAddress v4Address = null;
                    foreach (IPAddress address in host.AddressList)
                    {
                        if (address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            v4Address = address;
                            break;
                        }
                    }
                    // port here is irrelevant
                    listenerSocket.Bind(new IPEndPoint(v4Address, 0));
                    int code = unchecked((int)(0x80000000 | 0x18000000 | 0x00000001));
                    byte[] inBuf = new byte[4];
                    inBuf[0] = 1;
                    byte[] outBuf = new byte[4];
                    listenerSocket.IOControl(code, inBuf, outBuf);
                    remoteEP = listenerSocket.LocalEndPoint;
                    listenerSocket.BeginReceiveFrom(data, skip, data.Length - skip, SocketFlags.None, ref remoteEP, processData, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            public void Stop()
            {
                listenerSocket.Close();
            }

            void ProcessData(IAsyncResult asyncResult)
            {
                MemoryStream stream;
                try
                {
                    int received = listenerSocket.EndReceiveFrom(asyncResult, ref remoteEP);
                    if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("Received " + received + " bytes, from " + remoteEP);
                    if (TraceMyNet.Debug)
                    {
                        // Dump(data, skip, received);
                    }
                    packetsReceived++;
                    stream = new MemoryStream(data, 0, received + skip, false);

                    packetStream.Process(stream, 0, received, packetsReceived, true);
                    listenerSocket.BeginReceiveFrom(data, skip, data.Length - skip, SocketFlags.None, ref remoteEP, processData, null);
                }
                catch (SocketException socketException)
                {
                    // ignore this guy
                    if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("ignored socketException:" + socketException);
                }
                catch (ObjectDisposedException objectDisposedException)
                {
                    // ignore this guy
                    if (TraceMyNet.Debug) TraceMyNet.LogFile.WriteLine("ignored objectDisposedException:" + objectDisposedException);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            unsafe void Dump(byte[] buffer, int offset, int length)
            {
                do
                {
                    int n = Math.Min(length, 16);
                    string disp = offset.ToString("X8") + " : ";
                    byte current;
                    for (int i = 0; i < n; ++i)
                    {
                        current = buffer[offset + i];
                        disp += current.ToString("X2") + ((i == 7) ? '-' : ' ');
                    }
                    for (int i = n; i < 16; ++i)
                    {
                        disp += "   ";
                    }
                    disp += ": ";
                    for (int i = 0; i < n; ++i)
                    {
                        current = buffer[offset + i];
                        disp += ((current < 0x20) || (current > 0x7e)) ? '.' : (char)current;
                    }
                    TraceMyNet.LogFile.WriteLine(disp);
                    offset += n;
                    length -= n;
                } while (length > 0);
            }
        }
    }

    public static class AccurateTimer
    {
        const string Format = "hh:mm:ss.fffffff";

        static DateTime start = DateTime.Now;
        static Stopwatch now = Stopwatch.StartNew();

        public static DateTime GetTimeStamp()
        {
            return start.AddTicks(now.ElapsedTicks);
        }

        public static string FormatAbsolute(DateTime timeStamp)
        {
            return timeStamp.ToString(Format, DateTimeFormatInfo.InvariantInfo);
        }

        public static string FormatRelative(DateTime now, DateTime start)
        {
            string result = now.Subtract(start).ToString();
            if (result.Length < 16)
            {
                result += ".0000000";
            }
            return result;
        }
    }
}
