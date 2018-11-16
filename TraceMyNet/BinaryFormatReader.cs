namespace CodePlex.Tools.TraceMyNet.RemotingFormatter
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Xml;

    public class MainClass
    {
        // use this for testing outside of TraceMyNet
        public static void MainMethod(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("USAGE: BinaryFormatViewer <filename>");
                return;
            }
            Stream stream = File.OpenRead(args[0]);
            BinaryFormatReader reader = new BinaryFormatReader();
            reader.ReadAndDump(stream, Console.Out);
            stream.Close();
        }
    }

    internal class BinaryFormatReader
    {
        internal BinaryFormatReader()
        {
        }
        internal void ReadAndDump(Stream stream, TextWriter writer)
        {
            try
            {
                this.reader = new BinaryReader(stream);
                endOfStream = false;
                xmlWriter = new XmlTextWriter(writer);
                xmlWriter.Formatting = Formatting.Indented;
                Dumper.xmlWriter = xmlWriter;
                xmlWriter.WriteStartElement("bf", "binaryFormat", XmlNamespaces.BinaryFormatDefinitions);
                SerializationHeaderRecord header = new SerializationHeaderRecord();
                header.Read(this.reader);
                Dumper.Dump("Header", header);
                ReadAndDump(null);
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
            }
            catch (Exception ex)
            {
                writer.WriteLine(ex.ToString());
            }
        }
        internal void ReadAndDump(ArrayOrClassEnumerator enumerator)
        {
            while (!endOfStream)
            {
                if (enumerator != null)
                    xmlWriter.WriteComment("<" + enumerator.ItemName + ">");
                if (enumerator != null && enumerator.ItemType != InternalPrimitiveTypeE.Invalid)
                {//handle primitive types
                    ReadPrimitive(enumerator.ItemType);
                    if (enumerator.NextItem() == false)
                        break;
                }
                else
                {
                    Byte inByte = reader.ReadByte();
                    BinaryHeaderEnum binaryHeaderEnum = (BinaryHeaderEnum)inByte;
                    switch (binaryHeaderEnum)
                    {
                        case BinaryHeaderEnum.Assembly:
                        case BinaryHeaderEnum.CrossAppDomainAssembly:
                            ReadAssembly(binaryHeaderEnum);
                            break;
                        case BinaryHeaderEnum.Object:
                            ReadObject(binaryHeaderEnum);
                            break;
                        case BinaryHeaderEnum.ObjectWithMap:
                        case BinaryHeaderEnum.ObjectWithMapAssemId:
                            ReadObjectWithMap(binaryHeaderEnum);
                            break;
                        case BinaryHeaderEnum.ObjectWithMapTyped:
                        case BinaryHeaderEnum.ObjectWithMapTypedAssemId:
                            ReadObjectWithMapTyped(binaryHeaderEnum);
                            break;
                        case BinaryHeaderEnum.MemberReference:
                            ReadMemberReference(binaryHeaderEnum);
                            break;
                        case BinaryHeaderEnum.Array:
                        case BinaryHeaderEnum.ArraySinglePrimitive:
                        case BinaryHeaderEnum.ArraySingleObject:
                        case BinaryHeaderEnum.ArraySingleString:
                            ReadArray(binaryHeaderEnum);
                            break;
                        case BinaryHeaderEnum.ObjectString:
                        case BinaryHeaderEnum.CrossAppDomainString:
                            ReadObjectString(binaryHeaderEnum);
                            break;
                        case BinaryHeaderEnum.MemberPrimitiveTyped:
                            ReadMemberPrimitiveTyped(binaryHeaderEnum);
                            break;
                        case BinaryHeaderEnum.MessageEnd:
                            ReadMessageEnd(binaryHeaderEnum);
                            endOfStream = true;
                            break;
                        case BinaryHeaderEnum.ObjectNull:
                        case BinaryHeaderEnum.ObjectNullMultiple256:
                        case BinaryHeaderEnum.ObjectNullMultiple:
                            ReadObjectNull(binaryHeaderEnum);
                            break;
                        /*
                    case BinaryHeaderEnum.CrossAppDomainMap:
                        ReadCrossAppDomainMap();
                        break;
                        
                    */
                        case BinaryHeaderEnum.MethodCall:
                        case BinaryHeaderEnum.MethodReturn:
                            ReadMethodObject(binaryHeaderEnum);
                            break;
                        default:
                            throw new Exception("Unknown Enum " + binaryHeaderEnum);
                    }
                    if (enumerator != null && binaryHeaderEnum != BinaryHeaderEnum.Assembly && binaryHeaderEnum != BinaryHeaderEnum.CrossAppDomainAssembly)
                    {
                        xmlWriter.WriteComment("</" + enumerator.ItemName + ">");
                        if (enumerator.NextItem() == false)
                            break;
                    }
                }
            }
        }
        internal void ReadAssembly(BinaryHeaderEnum binaryHeaderEnum)
        {
            if (binaryHeaderEnum == BinaryHeaderEnum.CrossAppDomainAssembly)
            {
                BinaryCrossAppDomainAssembly crossAppDomainAssembly = new BinaryCrossAppDomainAssembly();
                crossAppDomainAssembly.Read(reader);
                Dumper.Dump("CrossAppDomainAssembly", crossAppDomainAssembly);
            }
            else
            {
                BinaryAssembly assemblyRecord = new BinaryAssembly();
                assemblyRecord.Read(reader);
                Dumper.Dump("AssemblyRecord", assemblyRecord);
                assemIdToAssemblyTable[assemblyRecord.assemId] = assemblyRecord.assemblyString;
            }
        }
        internal void ReadObject(BinaryHeaderEnum binaryHeaderEnum)
        {
            BinaryObject binaryObject = new BinaryObject();
            binaryObject.Read(reader);
            Dumper.Dump("ObjectWithNoMap", binaryObject);
            object map = mapIdToMapTable[binaryObject.mapId];
            if (map == null)
                throw new Exception("Map " + binaryObject.mapId + " is not seen before");
            if (map is BinaryObjectWithMap)
                ReadAndDump(new ArrayOrClassEnumerator((BinaryObjectWithMap)map));
            else
                ReadAndDump(new ArrayOrClassEnumerator((BinaryObjectWithMapTyped)map));

        }

        internal void ReadObjectWithMap(BinaryHeaderEnum binaryHeaderEnum)
        {
            BinaryObjectWithMap bowm = new BinaryObjectWithMap();
            bowm.binaryHeaderEnum = binaryHeaderEnum;
            bowm.Read(reader);
            Dumper.Dump("ObjectWithMap", bowm);
            mapIdToMapTable[bowm.objectId] = bowm;
            ReadAndDump(new ArrayOrClassEnumerator(bowm));
        }
        internal void ReadObjectWithMapTyped(BinaryHeaderEnum binaryHeaderEnum)
        {
            BinaryObjectWithMapTyped bowm = new BinaryObjectWithMapTyped();
            bowm.binaryHeaderEnum = binaryHeaderEnum;
            bowm.Read(reader);
            Dumper.Dump("ObjectWithMapTyped", bowm);
            mapIdToMapTable[bowm.objectId] = bowm;
            ReadAndDump(new ArrayOrClassEnumerator(bowm));
        }
        internal void ReadMemberReference(BinaryHeaderEnum binaryHeaderEnum)
        {
            MemberReference memberReference = new MemberReference();
            memberReference.Read(reader);
            Dumper.Dump("MemberReference", memberReference);
        }

        internal void ReadArray(BinaryHeaderEnum binaryHeaderEnum)
        {
            BinaryArray record = new BinaryArray(binaryHeaderEnum);
            record.Read(reader);
            Dumper.Dump("BinaryArray", record);
            ReadAndDump(new ArrayOrClassEnumerator(record));
        }

        internal void ReadObjectString(BinaryHeaderEnum binaryHeaderEnum)
        {
            if (binaryHeaderEnum == BinaryHeaderEnum.ObjectString)
            {
                BinaryObjectString objectString = new BinaryObjectString();
                objectString.Read(reader);
                Dumper.Dump("String", objectString);
            }
            else
            {
                BinaryCrossAppDomainString crossAppDomainString = new BinaryCrossAppDomainString();
                crossAppDomainString.Read(reader);
                Dumper.Dump("CrossAppDomainString", crossAppDomainString);
            }
        }
        internal void ReadMemberPrimitiveTyped(BinaryHeaderEnum binaryHeaderEnum)
        {
            MemberPrimitiveTyped memberPrimitiveTyped = new MemberPrimitiveTyped();
            memberPrimitiveTyped.Read(reader);
            Dumper.Dump("Boxed Primitive", memberPrimitiveTyped);
        }
        internal void ReadPrimitive(InternalPrimitiveTypeE primitiveTypeEnum)
        {
            Dumper.Dump("primitive", BinaryConverter.ReadPrimitiveType(reader, primitiveTypeEnum));
        }
        internal void ReadMethodObject(BinaryHeaderEnum binaryHeaderEnum)
        {
            if (binaryHeaderEnum == BinaryHeaderEnum.MethodCall)
            {
                BinaryMethodCall bmc = new BinaryMethodCall();
                bmc.Read(reader);
                Dumper.Dump("Method Call", bmc);
            }
            else
            {
                BinaryMethodReturn bmc = new BinaryMethodReturn();
                bmc.Read(reader);
                Dumper.Dump("Method Call", bmc);
            }
        }
        internal void ReadObjectNull(BinaryHeaderEnum binaryHeaderEnum)
        {
            ObjectNull objectNull = new ObjectNull(binaryHeaderEnum);
            objectNull.Read(reader);
            Dumper.Dump("null", objectNull);
        }

        internal void ReadMessageEnd(BinaryHeaderEnum binaryHeaderEnum)
        {
        }

        BinaryReader reader;
        XmlTextWriter xmlWriter;
        Hashtable assemIdToAssemblyTable = new Hashtable();
        Hashtable mapIdToMapTable = new Hashtable();
        bool endOfStream = false;
    }

    internal class Dumper
    {
        internal static XmlTextWriter xmlWriter;

        public static void Dump(string name, object o)
        {
            xmlWriter.WriteStartElement(name);
            xmlWriter.WriteString(o.ToString());
            xmlWriter.WriteEndElement();
        }

        public static void Dump(string name, IXmlizable xmlizable)
        {
            xmlizable.Write(xmlWriter);
            xmlWriter.Flush();
        }
    }
}
