namespace CodePlex.Tools.TraceMyNet.RemotingFormatter
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    [Serializable]
    internal enum BinaryHeaderEnum
    {
        SerializedStreamHeader = 0,
        Object = 1,
        ObjectWithMap = 2,
        ObjectWithMapAssemId = 3,
        ObjectWithMapTyped = 4,
        ObjectWithMapTypedAssemId = 5,
        ObjectString = 6,
        Array = 7,
        MemberPrimitiveTyped = 8,
        MemberReference = 9,
        ObjectNull = 10,
        MessageEnd = 11,
        Assembly = 12,
        ObjectNullMultiple256 = 13,
        ObjectNullMultiple = 14,
        ArraySinglePrimitive = 15,
        ArraySingleObject = 16,
        ArraySingleString = 17,
        CrossAppDomainMap = 18,
        CrossAppDomainString = 19,
        CrossAppDomainAssembly = 20,
        MethodCall = 21,
        MethodReturn = 22,
    }

    // BinaryTypeEnum is used specify the type on the wire.
    // Additional information is transmitted with Primitive and Object types
    [Serializable]
    internal enum BinaryTypeEnum
    {
        Primitive = 0,
        String = 1,
        Object = 2,
        ObjectUrt = 3,
        ObjectUser = 4,
        ObjectArray = 5,
        StringArray = 6,
        PrimitiveArray = 7,
    }

    [Serializable]
    internal enum BinaryArrayTypeEnum
    {
        Single = 0,
        Jagged = 1,
        Rectangular = 2,
        SingleOffset = 3,
        JaggedOffset = 4,
        RectangularOffset = 5,
    }
    // Data Type Enums
    [Serializable]
    internal enum InternalPrimitiveTypeE
    {
        Invalid = 0,
        Boolean = 1,
        Byte = 2,
        Char = 3,
        Currency = 4,
        Decimal = 5,
        Double = 6,
        Int16 = 7,
        Int32 = 8,
        Int64 = 9,
        SByte = 10,
        Single = 11,
        TimeSpan = 12,
        DateTime = 13,
        UInt16 = 14,
        UInt32 = 15,
        UInt64 = 16,

        // Used in only for MethodCall or MethodReturn header
        Null = 17,
        String = 18,
    }

    [Serializable, Flags]
    internal enum MessageEnum
    {
        NoArgs = 0x1,
        ArgsInline = 0x2,
        ArgsIsArray = 0x4,
        ArgsInArray = 0x8,
        NoContext = 0x10,
        ContextInline = 0x20,
        ContextInArray = 0x40,
        MethodSignatureInArray = 0x80,
        PropertyInArray = 0x100,
        NoReturnValue = 0x200,
        ReturnValueVoid = 0x400,
        ReturnValueInline = 0x800,
        ReturnValueInArray = 0x1000,
        ExceptionInArray = 0x2000,
    }

    // Interface for Binary Records.
    internal interface IStreamable
    {
        void Read(BinaryReader input);
        void Write(BinaryWriter sout);
        void Dump();
    }
    internal class XmlNamespaces
    {
        internal static string BinaryFormatDefinitions = "v1.bf.microsoft.com";
    }
    internal interface IXmlizable
    {
        void Write(XmlWriter xmlWriter);
    }

    internal sealed class SerializationHeaderRecord : IStreamable, IXmlizable
    {
        internal Int32 binaryFormatterMajorVersion = 1;
        internal Int32 binaryFormatterMinorVersion = 0;
        internal BinaryHeaderEnum binaryHeaderEnum;
        internal Int32 topId;
        internal Int32 headerId;
        internal Int32 majorVersion;
        internal Int32 minorVersion;

        internal SerializationHeaderRecord()
        {
        }

        public void Write(BinaryWriter sout)
        {
            majorVersion = binaryFormatterMajorVersion;
            minorVersion = binaryFormatterMinorVersion;
            sout.Write((Byte)binaryHeaderEnum);
            sout.Write(topId);
            sout.Write(headerId);
            sout.Write(binaryFormatterMajorVersion);
            sout.Write(binaryFormatterMinorVersion);
        }


        public void Read(BinaryReader input)
        {
            // binaryHeaderEnum has already been read
            binaryHeaderEnum = (BinaryHeaderEnum)input.ReadByte();
            topId = input.ReadInt32();
            headerId = input.ReadInt32();
            majorVersion = input.ReadInt32();
            minorVersion = input.ReadInt32();
        }

        public void Dump()
        {

        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("header", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("majorVersion", XmlNamespaces.BinaryFormatDefinitions, majorVersion.ToString());
            xmlWriter.WriteAttributeString("minorVersion", XmlNamespaces.BinaryFormatDefinitions, minorVersion.ToString());
            xmlWriter.WriteAttributeString("topId", XmlNamespaces.BinaryFormatDefinitions, topId.ToString());
            xmlWriter.WriteAttributeString("headerId", XmlNamespaces.BinaryFormatDefinitions, headerId.ToString());
            xmlWriter.WriteEndElement();
        }

    }


    // The Following classes read and write the binary records


    internal sealed class BinaryAssembly : IStreamable, IXmlizable
    {
        internal Int32 assemId;
        internal String assemblyString;

        internal BinaryAssembly()
        {
        }


        internal void Set(Int32 assemId, String assemblyString)
        {
            this.assemId = assemId;
            this.assemblyString = assemblyString;
        }


        public void Write(BinaryWriter sout)
        {
            sout.Write((Byte)BinaryHeaderEnum.Assembly);
            sout.Write(assemId);
            sout.Write(assemblyString);
        }

        public void Read(BinaryReader input)
        {
            assemId = input.ReadInt32();
            assemblyString = input.ReadString();
        }

        public void Dump()
        {
        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("assembly", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("id", XmlNamespaces.BinaryFormatDefinitions, assemId.ToString());
            xmlWriter.WriteAttributeString("name", XmlNamespaces.BinaryFormatDefinitions, assemblyString);
            xmlWriter.WriteEndElement();
        }
    }

    internal sealed class BinaryCrossAppDomainAssembly : IStreamable, IXmlizable
    {
        internal Int32 assemId;
        internal Int32 assemblyIndex;

        internal BinaryCrossAppDomainAssembly()
        {
        }


        internal void Set(Int32 assemId, Int32 assemblyIndex)
        {
            this.assemId = assemId;
            this.assemblyIndex = assemblyIndex;
        }


        public void Write(BinaryWriter sout)
        {
            sout.Write((Byte)BinaryHeaderEnum.CrossAppDomainAssembly);
            sout.Write(assemId);
            sout.Write(assemblyIndex);
        }

        public void Read(BinaryReader input)
        {
            assemId = input.ReadInt32();
            assemblyIndex = input.ReadInt32();
        }

        public void Dump()
        {
        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("assembly", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("id", XmlNamespaces.BinaryFormatDefinitions, assemId.ToString());
            xmlWriter.WriteAttributeString("index", XmlNamespaces.BinaryFormatDefinitions, assemblyIndex.ToString());
            xmlWriter.WriteEndElement();
        }

    }

    internal sealed class BinaryObject : IStreamable, IXmlizable
    {
        internal Int32 objectId;
        internal Int32 mapId;

        internal void Set(Int32 objectId, Int32 mapId)
        {
            this.objectId = objectId;
            this.mapId = mapId;
        }

        public void Write(BinaryWriter sout)
        {
            sout.Write((Byte)BinaryHeaderEnum.Object);
            sout.Write(objectId);
            sout.Write(mapId);
        }

        public void Read(BinaryReader input)
        {
            objectId = input.ReadInt32();
            mapId = input.ReadInt32();
        }

        public void Dump()
        {
        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("objectWithMapRef", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("id", XmlNamespaces.BinaryFormatDefinitions, objectId.ToString());
            xmlWriter.WriteAttributeString("mapId", XmlNamespaces.BinaryFormatDefinitions, mapId.ToString());
            xmlWriter.WriteEndElement();
        }

    }
    internal sealed class BinaryObjectWithMap : IStreamable, IXmlizable
    {
        internal BinaryHeaderEnum binaryHeaderEnum;
        internal Int32 objectId;
        internal String name;
        internal Int32 numMembers;
        internal String[] memberNames;
        internal Int32 assemId;

        internal BinaryObjectWithMap()
        {
        }


        internal void Set(Int32 objectId, String name, Int32 numMembers, String[] memberNames, Int32 assemId)
        {
            this.objectId = objectId;
            this.name = name;
            this.numMembers = numMembers;
            this.memberNames = memberNames;
            this.assemId = assemId;

            if (assemId > 0)
                binaryHeaderEnum = BinaryHeaderEnum.ObjectWithMapAssemId;
            else
                binaryHeaderEnum = BinaryHeaderEnum.ObjectWithMap;

        }

        public void Write(BinaryWriter sout)
        {
            sout.Write((Byte)binaryHeaderEnum);
            sout.Write(objectId);
            sout.Write(name);
            sout.Write(numMembers);
            for (int i = 0; i < numMembers; i++)
                sout.Write(memberNames[i]);
            if (assemId > 0)
                sout.Write(assemId);
        }

        public void Read(BinaryReader input)
        {
            objectId = input.ReadInt32();
            name = input.ReadString();
            numMembers = input.ReadInt32();
            memberNames = new String[numMembers];
            for (int i = 0; i < numMembers; i++)
            {
                memberNames[i] = input.ReadString();
            }

            if (binaryHeaderEnum == BinaryHeaderEnum.ObjectWithMapAssemId)
            {
                assemId = input.ReadInt32();
            }
        }


        public void Dump()
        {
        }

        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("objectWithNames", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("id", XmlNamespaces.BinaryFormatDefinitions, objectId.ToString());
            xmlWriter.WriteAttributeString("assemblyId", XmlNamespaces.BinaryFormatDefinitions, assemId.ToString());
            xmlWriter.WriteAttributeString("name", XmlNamespaces.BinaryFormatDefinitions, name);
            xmlWriter.WriteAttributeString("numMembers", XmlNamespaces.BinaryFormatDefinitions, numMembers.ToString());
            xmlWriter.WriteStartElement("members", XmlNamespaces.BinaryFormatDefinitions);
            for (int i = 0; i < numMembers; i++)
            {
                xmlWriter.WriteStartElement("member", XmlNamespaces.BinaryFormatDefinitions);
                xmlWriter.WriteAttributeString("name", XmlNamespaces.BinaryFormatDefinitions, memberNames[i]);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
    }



    internal sealed class BinaryObjectWithMapTyped : IStreamable, IXmlizable
    {
        internal BinaryHeaderEnum binaryHeaderEnum;
        internal Int32 objectId;
        internal String name;
        internal Int32 numMembers;
        internal String[] memberNames;
        internal BinaryTypeEnum[] binaryTypeEnumA;
        internal Object[] typeInformationA;
        internal Int32[] memberAssemIds;
        internal Int32 assemId;

        internal void Set(Int32 objectId, String name, Int32 numMembers, String[] memberNames, BinaryTypeEnum[] binaryTypeEnumA, Object[] typeInformationA, Int32[] memberAssemIds, Int32 assemId)
        {
            this.objectId = objectId;
            this.assemId = assemId;
            this.name = name;
            this.numMembers = numMembers;
            this.memberNames = memberNames;
            this.binaryTypeEnumA = binaryTypeEnumA;
            this.typeInformationA = typeInformationA;
            this.memberAssemIds = memberAssemIds;
            this.assemId = assemId;

            if (assemId > 0)
                binaryHeaderEnum = BinaryHeaderEnum.ObjectWithMapTypedAssemId;
            else
                binaryHeaderEnum = BinaryHeaderEnum.ObjectWithMapTyped;
        }


        public void Write(BinaryWriter sout)
        {
            sout.Write((Byte)binaryHeaderEnum);
            sout.Write(objectId);
            sout.Write(name);
            sout.Write(numMembers);
            for (int i = 0; i < numMembers; i++)
                sout.Write(memberNames[i]);
            for (int i = 0; i < numMembers; i++)
                sout.Write((Byte)binaryTypeEnumA[i]);
            for (int i = 0; i < numMembers; i++)
                BinaryConverter.WriteTypeInfo(binaryTypeEnumA[i], typeInformationA[i], memberAssemIds[i], sout);

            if (assemId > 0)
                sout.Write(assemId);

        }

        public void Read(BinaryReader input)
        {
            objectId = input.ReadInt32();
            name = input.ReadString();
            numMembers = input.ReadInt32();
            memberNames = new String[numMembers];
            binaryTypeEnumA = new BinaryTypeEnum[numMembers];
            typeInformationA = new Object[numMembers];
            memberAssemIds = new Int32[numMembers];
            for (int i = 0; i < numMembers; i++)
                memberNames[i] = input.ReadString();
            for (int i = 0; i < numMembers; i++)
                binaryTypeEnumA[i] = (BinaryTypeEnum)input.ReadByte();
            for (int i = 0; i < numMembers; i++)
                typeInformationA[i] = BinaryConverter.ReadTypeInfo(binaryTypeEnumA[i], input, out memberAssemIds[i]);

            if (binaryHeaderEnum == BinaryHeaderEnum.ObjectWithMapTypedAssemId)
                assemId = input.ReadInt32();
        }

        public void Dump()
        {
        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("objectWithNamesAndTypes", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString(null, "id", XmlNamespaces.BinaryFormatDefinitions, objectId.ToString());
            xmlWriter.WriteAttributeString("assemblyId", XmlNamespaces.BinaryFormatDefinitions, assemId.ToString());
            xmlWriter.WriteAttributeString(null, "name", XmlNamespaces.BinaryFormatDefinitions, name);
            xmlWriter.WriteAttributeString(null, "numMembers", XmlNamespaces.BinaryFormatDefinitions, numMembers.ToString());
            xmlWriter.WriteStartElement("members", XmlNamespaces.BinaryFormatDefinitions);
            for (int i = 0; i < numMembers; i++)
            {
                xmlWriter.WriteStartElement("member", XmlNamespaces.BinaryFormatDefinitions);
                xmlWriter.WriteAttributeString("name", XmlNamespaces.BinaryFormatDefinitions, memberNames[i]);
                xmlWriter.WriteAttributeString("kind", XmlNamespaces.BinaryFormatDefinitions, binaryTypeEnumA[i].ToString());
                if (typeInformationA[i] != null)
                {
                    xmlWriter.WriteAttributeString("type", XmlNamespaces.BinaryFormatDefinitions, typeInformationA[i] != null ? typeInformationA[i].ToString() : "anyType");
                    xmlWriter.WriteAttributeString("assemblyId", XmlNamespaces.BinaryFormatDefinitions, memberAssemIds[i].ToString());
                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
    }

    internal sealed class MemberReference : IStreamable, IXmlizable
    {
        internal Int32 idRef;
        internal void Set(Int32 idRef)
        {
            this.idRef = idRef;
        }

        public void Write(BinaryWriter sout)
        {
            sout.Write((Byte)BinaryHeaderEnum.MemberReference);
            sout.Write(idRef);
        }

        public void Read(BinaryReader input)
        {
            //binaryHeaderEnum = input.ReadByte(); already read
            idRef = input.ReadInt32();
        }

        public void Dump()
        {
        }

        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("memberRef", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("idRef", XmlNamespaces.BinaryFormatDefinitions, idRef.ToString());
            xmlWriter.WriteEndElement();
        }
    }
    internal sealed class BinaryArray : IStreamable, IXmlizable
    {
        internal Int32 objectId;
        internal Int32 rank;
        internal Int32[] lengthA;
        internal Int32[] lowerBoundA;
        internal BinaryTypeEnum binaryTypeEnum;
        internal Object typeInformation;
        internal int assemId = 0;

        private BinaryHeaderEnum binaryHeaderEnum;
        internal BinaryArrayTypeEnum binaryArrayTypeEnum;

        internal BinaryArray(BinaryHeaderEnum binaryHeaderEnum)
        {
            this.binaryHeaderEnum = binaryHeaderEnum;
        }


        internal void Set(Int32 objectId, Int32 rank, Int32[] lengthA, Int32[] lowerBoundA, BinaryTypeEnum binaryTypeEnum, Object typeInformation, BinaryArrayTypeEnum binaryArrayTypeEnum, int assemId)
        {
            this.objectId = objectId;
            this.binaryArrayTypeEnum = binaryArrayTypeEnum;
            this.rank = rank;
            this.lengthA = lengthA;
            this.lowerBoundA = lowerBoundA;
            this.binaryTypeEnum = binaryTypeEnum;
            this.typeInformation = typeInformation;
            this.assemId = assemId;
            binaryHeaderEnum = BinaryHeaderEnum.Array;

            if (binaryArrayTypeEnum == BinaryArrayTypeEnum.Single)
            {
                if (binaryTypeEnum == BinaryTypeEnum.Primitive)
                    binaryHeaderEnum = BinaryHeaderEnum.ArraySinglePrimitive;
                else if (binaryTypeEnum == BinaryTypeEnum.String)
                    binaryHeaderEnum = BinaryHeaderEnum.ArraySingleString;
                else if (binaryTypeEnum == BinaryTypeEnum.Object)
                    binaryHeaderEnum = BinaryHeaderEnum.ArraySingleObject;
            }
        }


        public void Write(BinaryWriter sout)
        {
            switch (binaryHeaderEnum)
            {
                case BinaryHeaderEnum.ArraySinglePrimitive:
                    sout.Write((Byte)binaryHeaderEnum);
                    sout.Write(objectId);
                    sout.Write(lengthA[0]);
                    sout.Write((Byte)((InternalPrimitiveTypeE)typeInformation));
                    break;
                case BinaryHeaderEnum.ArraySingleString:
                    sout.Write((Byte)binaryHeaderEnum);
                    sout.Write(objectId);
                    sout.Write(lengthA[0]);
                    break;
                case BinaryHeaderEnum.ArraySingleObject:
                    sout.Write((Byte)binaryHeaderEnum);
                    sout.Write(objectId);
                    sout.Write(lengthA[0]);
                    break;
                default:
                    sout.Write((Byte)binaryHeaderEnum);
                    sout.Write(objectId);
                    sout.Write((Byte)binaryArrayTypeEnum);
                    sout.Write(rank);
                    for (int i = 0; i < rank; i++)
                        sout.Write(lengthA[i]);
                    if ((binaryArrayTypeEnum == BinaryArrayTypeEnum.SingleOffset) ||
                        (binaryArrayTypeEnum == BinaryArrayTypeEnum.JaggedOffset) ||
                        (binaryArrayTypeEnum == BinaryArrayTypeEnum.RectangularOffset))
                    {
                        for (int i = 0; i < rank; i++)
                            sout.Write(lowerBoundA[i]);
                    }
                    sout.Write((Byte)binaryTypeEnum);
                    BinaryConverter.WriteTypeInfo(binaryTypeEnum, typeInformation, assemId, sout);
                    break;
            }
        }

        public void Read(BinaryReader input)
        {
            switch (binaryHeaderEnum)
            {
                case BinaryHeaderEnum.ArraySinglePrimitive:
                    objectId = input.ReadInt32();
                    lengthA = new int[1];
                    lengthA[0] = input.ReadInt32();
                    binaryArrayTypeEnum = BinaryArrayTypeEnum.Single;
                    rank = 1;
                    lowerBoundA = new Int32[rank];
                    binaryTypeEnum = BinaryTypeEnum.Primitive;
                    typeInformation = (InternalPrimitiveTypeE)input.ReadByte();
                    break;
                case BinaryHeaderEnum.ArraySingleString:
                    objectId = input.ReadInt32();
                    lengthA = new int[1];
                    lengthA[0] = (int)input.ReadInt32();
                    binaryArrayTypeEnum = BinaryArrayTypeEnum.Single;
                    rank = 1;
                    lowerBoundA = new Int32[rank];
                    binaryTypeEnum = BinaryTypeEnum.String;
                    typeInformation = null;
                    break;
                case BinaryHeaderEnum.ArraySingleObject:
                    objectId = input.ReadInt32();
                    lengthA = new int[1];
                    lengthA[0] = (int)input.ReadInt32();
                    binaryArrayTypeEnum = BinaryArrayTypeEnum.Single;
                    rank = 1;
                    lowerBoundA = new Int32[rank];
                    binaryTypeEnum = BinaryTypeEnum.Object;
                    typeInformation = null;
                    break;
                default:
                    objectId = input.ReadInt32();
                    binaryArrayTypeEnum = (BinaryArrayTypeEnum)input.ReadByte();
                    rank = input.ReadInt32();
                    lengthA = new Int32[rank];
                    lowerBoundA = new Int32[rank];
                    for (int i = 0; i < rank; i++)
                        lengthA[i] = input.ReadInt32();
                    if ((binaryArrayTypeEnum == BinaryArrayTypeEnum.SingleOffset) ||
                        (binaryArrayTypeEnum == BinaryArrayTypeEnum.JaggedOffset) ||
                        (binaryArrayTypeEnum == BinaryArrayTypeEnum.RectangularOffset))
                    {
                        for (int i = 0; i < rank; i++)
                            lowerBoundA[i] = input.ReadInt32();
                    }
                    binaryTypeEnum = (BinaryTypeEnum)input.ReadByte();
                    typeInformation = BinaryConverter.ReadTypeInfo(binaryTypeEnum, input, out assemId);
                    break;
            }
        }

        public void Dump()
        {
        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("array", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("id", XmlNamespaces.BinaryFormatDefinitions, objectId.ToString());
            xmlWriter.WriteAttributeString("rank", XmlNamespaces.BinaryFormatDefinitions, rank.ToString());
            xmlWriter.WriteAttributeString("assemblyId", XmlNamespaces.BinaryFormatDefinitions, assemId.ToString());
            xmlWriter.WriteAttributeString("itemType", XmlNamespaces.BinaryFormatDefinitions, typeInformation != null ? typeInformation.ToString() : "anyType");
            xmlWriter.WriteStartElement("dimensions", XmlNamespaces.BinaryFormatDefinitions);
            for (int i = 0; i < rank; i++)
            {
                xmlWriter.WriteStartElement("dimension", XmlNamespaces.BinaryFormatDefinitions);
                xmlWriter.WriteAttributeString("size", XmlNamespaces.BinaryFormatDefinitions, lengthA[i].ToString());
                if ((binaryArrayTypeEnum == BinaryArrayTypeEnum.SingleOffset) ||
                    (binaryArrayTypeEnum == BinaryArrayTypeEnum.JaggedOffset) ||
                    (binaryArrayTypeEnum == BinaryArrayTypeEnum.RectangularOffset))
                    xmlWriter.WriteAttributeString("start", XmlNamespaces.BinaryFormatDefinitions, lowerBoundA[i].ToString());
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
    }
    internal sealed class BinaryObjectString : IStreamable, IXmlizable
    {
        internal Int32 objectId;
        internal String value;
        internal void Set(Int32 objectId, String value)
        {
            this.objectId = objectId;
            this.value = value;
        }

        public void Write(BinaryWriter sout)
        {
            sout.Write((Byte)BinaryHeaderEnum.ObjectString);
            sout.Write(objectId);
            sout.Write(value);
        }

        public void Read(BinaryReader input)
        {
            objectId = input.ReadInt32();
            value = input.ReadString();
        }

        public void Dump()
        {
        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("string", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("id", XmlNamespaces.BinaryFormatDefinitions, objectId.ToString());
            xmlWriter.WriteString(value);
            xmlWriter.WriteEndElement();
        }

    }

    internal sealed class BinaryCrossAppDomainString : IStreamable, IXmlizable
    {
        internal Int32 objectId;
        internal Int32 value;

        internal BinaryCrossAppDomainString()
        {
        }

        internal void Set(Int32 objectId, Int32 value)
        {
            this.objectId = objectId;
            this.value = value;
        }

        public void Write(BinaryWriter sout)
        {
            sout.Write((Byte)BinaryHeaderEnum.CrossAppDomainString);
            sout.Write(objectId);
            sout.Write(value);
        }

        public void Read(BinaryReader input)
        {
            objectId = input.ReadInt32();
            value = input.ReadInt32();
        }

        public void Dump()
        {
        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("stringAppDomain", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("id", XmlNamespaces.BinaryFormatDefinitions, objectId.ToString());
            xmlWriter.WriteAttributeString("valueId", XmlNamespaces.BinaryFormatDefinitions, value.ToString());
            xmlWriter.WriteEndElement();
        }

    }
    internal sealed class MemberPrimitiveTyped : IStreamable, IXmlizable
    {
        internal InternalPrimitiveTypeE primitiveTypeEnum;
        internal Object value;

        internal MemberPrimitiveTyped()
        {
        }

        internal void Set(InternalPrimitiveTypeE primitiveTypeEnum, Object value)
        {
            this.primitiveTypeEnum = primitiveTypeEnum;
            this.value = value;
        }


        public void Write(BinaryWriter sout)
        {
            //sout.Write((Byte)BinaryHeaderEnum.MemberPrimitiveTyped);
            //sout.Write((Byte)primitiveTypeEnum); 
            //sout.Write(value);
        }

        public void Read(BinaryReader input)
        {
            primitiveTypeEnum = (InternalPrimitiveTypeE)input.ReadByte();
            value = BinaryConverter.ReadPrimitiveType(input, primitiveTypeEnum);
        }

        public void Dump()
        {
        }

        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("boxedPrimitive", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("type", XmlNamespaces.BinaryFormatDefinitions, primitiveTypeEnum.ToString());
            xmlWriter.WriteString(value.ToString());
            xmlWriter.WriteEndElement();
        }
    }

    internal sealed class ObjectNull : IStreamable, IXmlizable
    {
        internal int nullCount;
        BinaryHeaderEnum binaryHeaderEnum;

        internal ObjectNull(BinaryHeaderEnum binaryHeaderEnum)
        {
            this.binaryHeaderEnum = binaryHeaderEnum;
        }

        internal void SetNullCount(int nullCount)
        {
            this.nullCount = nullCount;
        }

        public void Write(BinaryWriter sout)
        {
            if (nullCount == 1)
            {
                sout.Write((Byte)BinaryHeaderEnum.ObjectNull);
            }
            else if (nullCount < 256)
            {
                sout.Write((Byte)BinaryHeaderEnum.ObjectNullMultiple256);
                sout.Write((Byte)nullCount);
            }
            else
            {
                sout.Write((Byte)BinaryHeaderEnum.ObjectNullMultiple);
                sout.Write(nullCount);
            }
        }


        public void Read(BinaryReader input)
        {
            //binaryHeaderEnum = input.ReadByte(); already read
            switch (binaryHeaderEnum)
            {
                case BinaryHeaderEnum.ObjectNull:
                    nullCount = 1;
                    break;
                case BinaryHeaderEnum.ObjectNullMultiple256:
                    nullCount = input.ReadByte();
                    break;
                case BinaryHeaderEnum.ObjectNullMultiple:
                    nullCount = input.ReadInt32();
                    break;
            }
        }

        public void Dump()
        {
        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("isNull", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("nullCount", XmlNamespaces.BinaryFormatDefinitions, nullCount.ToString());
            xmlWriter.WriteEndElement();
        }
    }

    internal sealed class BinaryMethodCall : IXmlizable
    {
        String methodName;
        String typeName;
        Object[] args;
        String scallContext;
        MessageEnum messageEnum;

        internal void Read(BinaryReader input)
        {
            messageEnum = (MessageEnum)input.ReadInt32();
            //uri = (String)IOUtil.ReadWithCode(input);
            methodName = (String)BinaryConverter.ReadWithCode(input);
            typeName = (String)BinaryConverter.ReadWithCode(input);

            if (BinaryConverter.FlagTest(messageEnum, MessageEnum.ContextInline))
            {
                scallContext = (String)BinaryConverter.ReadWithCode(input);
            }

            if (BinaryConverter.FlagTest(messageEnum, MessageEnum.ArgsInline))
                args = BinaryConverter.ReadArgs(input);
        }


        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("methodCall", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("methodName", XmlNamespaces.BinaryFormatDefinitions, methodName);
            xmlWriter.WriteAttributeString("typeName", XmlNamespaces.BinaryFormatDefinitions, typeName);
            xmlWriter.WriteAttributeString("callType", XmlNamespaces.BinaryFormatDefinitions, messageEnum.ToString());
            xmlWriter.WriteAttributeString("scallContext", XmlNamespaces.BinaryFormatDefinitions, scallContext);
            xmlWriter.WriteStartElement("args", XmlNamespaces.BinaryFormatDefinitions);
            for (int i = 0; i < args.Length; i++)
            {
                xmlWriter.WriteStartElement("arg", XmlNamespaces.BinaryFormatDefinitions);
                xmlWriter.WriteString(EncodeNull(args[0]));
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        public string EncodeNull(object val)
        {
            return val != null ? val.ToString() : "{null}";
        }
    }

    internal sealed class BinaryMethodReturn : IXmlizable
    {
        Object returnValue;
        Object[] args;
        String scallContext;
        MessageEnum messageEnum;
        static object invalidReturn = new object();

        internal BinaryMethodReturn()
        {
        }


        public void Read(BinaryReader input)
        {
            messageEnum = (MessageEnum)input.ReadInt32();

            if (BinaryConverter.FlagTest(messageEnum, MessageEnum.NoReturnValue))
                returnValue = invalidReturn;
            else if (BinaryConverter.FlagTest(messageEnum, MessageEnum.ReturnValueVoid))
            {
                returnValue = invalidReturn;
            }
            else if (BinaryConverter.FlagTest(messageEnum, MessageEnum.ReturnValueInline))
                returnValue = BinaryConverter.ReadWithCode(input);

            if (BinaryConverter.FlagTest(messageEnum, MessageEnum.ContextInline))
            {
                scallContext = (String)BinaryConverter.ReadWithCode(input);
            }

            if (BinaryConverter.FlagTest(messageEnum, MessageEnum.ArgsInline))
                args = BinaryConverter.ReadArgs(input);
        }
        public void Write(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("methodReturn", XmlNamespaces.BinaryFormatDefinitions);
            xmlWriter.WriteAttributeString("scallContext", XmlNamespaces.BinaryFormatDefinitions, scallContext);
            if (returnValue != invalidReturn)
            {
                xmlWriter.WriteStartElement("returnValue", XmlNamespaces.BinaryFormatDefinitions);
                xmlWriter.WriteString(EncodeNull(returnValue));
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteStartElement("args", XmlNamespaces.BinaryFormatDefinitions);
            for (int i = 0; i < args.Length; i++)
            {
                xmlWriter.WriteStartElement("arg", XmlNamespaces.BinaryFormatDefinitions);
                xmlWriter.WriteString(EncodeNull(args[0]));
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
        public string EncodeNull(object val)
        {
            return val != null ? val.ToString() : "{null}";
        }

    }

#if false
    internal sealed class BinaryCrossAppDomainMap : IStreamable
    {
        internal Int32 crossAppDomainArrayIndex;

        internal BinaryCrossAppDomainMap()
        {
        }

        public  void Write(BinaryWriter sout)
        {
            sout.WriteByte((Byte)BinaryHeaderEnum.CrossAppDomainMap);
            sout.WriteInt32(crossAppDomainArrayIndex);
        }

        public  void Read(BinaryReader input)
        {
            crossAppDomainArrayIndex = input.ReadInt32();
        }

        public  void Dump()
        {
        }

    }

    internal sealed class MemberPrimitiveUnTyped : IStreamable
    {
        // Used for members with primitive values and types are needed

        internal InternalPrimitiveTypeE typeInformation;
        internal Object value;

        internal MemberPrimitiveUnTyped()
        {
        }

        internal  void Set(InternalPrimitiveTypeE typeInformation, Object value)
        {
            SerTrace.Log( this, "MemberPrimitiveUnTyped Set typeInformation ",typeInformation," value ",value);
            this.typeInformation = typeInformation;
            this.value = value;
        }

        internal  void Set(InternalPrimitiveTypeE typeInformation)
        {
            SerTrace.Log(this, "MemberPrimitiveUnTyped  Set ",typeInformation);
            this.typeInformation = typeInformation;
        }



        public  void Write(BinaryWriter sout)
        {
            sout.WriteValue(typeInformation, value);
        }

        public  void Read(BinaryReader input)
        {
            //binaryHeaderEnum = input.ReadByte(); already read
            value = input.ReadValue(typeInformation);
        }

        public  void Dump()
        {
        }

    }


    internal sealed class MessageEnd : IStreamable
    {

        internal MessageEnd()
        {
        }

        public  void Write(BinaryWriter sout)
        {
            sout.WriteByte((Byte)BinaryHeaderEnum.MessageEnd);
        }

        public  void Read(BinaryReader input)
        {
            //binaryHeaderEnum = input.ReadByte(); already read
        }

        public  void Dump()
        {
        }

        public  void Dump(Stream sout)
        {
        }


    }

#endif
    internal class BinaryConverter
    {
        internal static void WriteTypeInfo(BinaryTypeEnum binaryTypeEnum, Object typeInformation, int assemId, BinaryWriter sout)
        {
            switch (binaryTypeEnum)
            {
                case BinaryTypeEnum.Primitive:
                case BinaryTypeEnum.PrimitiveArray:
                    //BCLDebug.Assert(typeInformation!=null, "[BinaryConverter.WriteTypeInfo]typeInformation!=null");
                    sout.Write((Byte)((InternalPrimitiveTypeE)typeInformation));
                    break;
                case BinaryTypeEnum.String:
                case BinaryTypeEnum.Object:
                case BinaryTypeEnum.StringArray:
                case BinaryTypeEnum.ObjectArray:
                    break;
                case BinaryTypeEnum.ObjectUrt:
                    //BCLDebug.Assert(typeInformation!=null, "[BinaryConverter.WriteTypeInfo]typeInformation!=null");
                    sout.Write(typeInformation.ToString());
                    break;
                case BinaryTypeEnum.ObjectUser:
                    //BCLDebug.Assert(typeInformation!=null, "[BinaryConverter.WriteTypeInfo]typeInformation!=null");
                    sout.Write(typeInformation.ToString());
                    sout.Write(assemId);
                    break;
                default:
                    throw new Exception("Unknown BinaryTypeEnum " + binaryTypeEnum);
            }
        }
        internal static Object ReadTypeInfo(BinaryTypeEnum binaryTypeEnum, BinaryReader input, out int assemId)
        {
            Object var = null;
            int readAssemId = 0;

            switch (binaryTypeEnum)
            {
                case BinaryTypeEnum.Primitive:
                case BinaryTypeEnum.PrimitiveArray:
                    var = (InternalPrimitiveTypeE)input.ReadByte();
                    break;
                case BinaryTypeEnum.String:
                case BinaryTypeEnum.Object:
                case BinaryTypeEnum.StringArray:
                case BinaryTypeEnum.ObjectArray:
                    break;
                case BinaryTypeEnum.ObjectUrt:
                    var = input.ReadString();
                    break;
                case BinaryTypeEnum.ObjectUser:
                    var = input.ReadString();
                    readAssemId = input.ReadInt32();
                    break;
                default:
                    throw new Exception("Unknown BinaryTypeEnum " + binaryTypeEnum);
            }
            assemId = readAssemId;
            return var;
        }

        internal static object ReadPrimitiveType(BinaryReader input, InternalPrimitiveTypeE primitiveTypeEnum)
        {
            object value = null;
            switch (primitiveTypeEnum)
            {
                case InternalPrimitiveTypeE.Boolean: value = input.ReadBoolean(); break;
                case InternalPrimitiveTypeE.Byte: value = input.ReadByte(); break;
                case InternalPrimitiveTypeE.Char: value = input.ReadChar(); break;
                //case InternalPrimitiveTypeE.Currency: value = input.ReadCurrency();break;
                case InternalPrimitiveTypeE.Decimal: value = input.ReadDecimal(); break;
                case InternalPrimitiveTypeE.Double: value = input.ReadDouble(); break;
                case InternalPrimitiveTypeE.Int16: value = input.ReadInt16(); break;
                case InternalPrimitiveTypeE.Int32: value = input.ReadInt32(); break;
                case InternalPrimitiveTypeE.Int64: value = input.ReadInt64(); break;
                case InternalPrimitiveTypeE.SByte: value = input.ReadSByte(); break;
                case InternalPrimitiveTypeE.Single: value = input.ReadSingle(); break;
                //case InternalPrimitiveTypeE.TimeSpan: value = input.ReadTimeSpan();break;
                //case InternalPrimitiveTypeE.DateTime: value = input.ReadDateTime();break;
                case InternalPrimitiveTypeE.UInt16: value = input.ReadUInt16(); break;
                case InternalPrimitiveTypeE.UInt32: value = input.ReadUInt32(); break;
                case InternalPrimitiveTypeE.UInt64: value = input.ReadUInt64(); break;
                default: throw new Exception("Invalid Primitive Type " + primitiveTypeEnum);
            }
            return value;
        }
        internal static Object ReadWithCode(BinaryReader input)
        {
            InternalPrimitiveTypeE code = (InternalPrimitiveTypeE)input.ReadByte();
            if (code == InternalPrimitiveTypeE.Null)
                return null;
            else if (code == InternalPrimitiveTypeE.String)
                return input.ReadString();
            return BinaryConverter.ReadPrimitiveType(input, code);
        }

        internal static Object[] ReadArgs(BinaryReader input)
        {
            int length = input.ReadInt32();
            Object[] args = new Object[length];
            for (int i = 0; i < length; i++)
                args[i] = ReadWithCode(input);
            return args;
        }
        internal static bool FlagTest(MessageEnum flag, MessageEnum target)
        {
            if ((flag & target) == target)
                return true;
            else
                return false;
        }

    }

    internal class ArrayOrClassEnumerator
    {
        string[] memberNames;
        InternalPrimitiveTypeE[] primitiveTypes;
        int currentItem;
        internal ArrayOrClassEnumerator(BinaryArray arrayRecord)
        {
            currentItem = 0;
            int count = 1;
            for (int i = 0; i < arrayRecord.lengthA.Length; i++)
            {
                count *= (arrayRecord.lengthA[i] - arrayRecord.lowerBoundA[i]);
            }
            memberNames = new string[count];
            primitiveTypes = new InternalPrimitiveTypeE[count];

            IndexCounter indexCounter = new IndexCounter(arrayRecord.lengthA, arrayRecord.lowerBoundA);
            InternalPrimitiveTypeE arrayElementPrimitiveType = InternalPrimitiveTypeE.Invalid;
            if (arrayRecord.typeInformation is InternalPrimitiveTypeE)
                arrayElementPrimitiveType = (InternalPrimitiveTypeE)arrayRecord.typeInformation;

            for (int i = 0; i < count; i++, indexCounter.Next())
            {
                memberNames[i] = indexCounter.ToString();
                primitiveTypes[i] = arrayElementPrimitiveType;
            }
        }
        internal ArrayOrClassEnumerator(BinaryObjectWithMapTyped bowm)
        {
            currentItem = 0;
            int count = bowm.memberNames.Length;
            memberNames = new string[count];
            primitiveTypes = new InternalPrimitiveTypeE[count];
            for (int i = 0; i < count; i++)
            {
                memberNames[i] = bowm.name + "." + bowm.memberNames[i];
                primitiveTypes[i] = (bowm.typeInformationA[i] is InternalPrimitiveTypeE) ? (InternalPrimitiveTypeE)bowm.typeInformationA[i] : InternalPrimitiveTypeE.Invalid;
            }
        }
        internal ArrayOrClassEnumerator(BinaryObjectWithMap bowm)
        {
            currentItem = 0;
            int count = bowm.memberNames.Length;
            memberNames = new string[count];
            primitiveTypes = new InternalPrimitiveTypeE[count];
            for (int i = 0; i < count; i++)
            {
                memberNames[i] = bowm.name + "." + bowm.memberNames[i];
                primitiveTypes[i] = InternalPrimitiveTypeE.Invalid;
            }
        }


        internal bool NextItem()
        {
            if (currentItem == (memberNames.Length - 1))
                return false;
            currentItem++;
            return true;
        }

        internal InternalPrimitiveTypeE ItemType { get { return primitiveTypes[currentItem]; } }
        internal string ItemName { get { return memberNames[currentItem]; } }

        class IndexCounter
        {
            int[] indexCounter;
            int[] lowerBounds;
            int[] lengths;
            internal IndexCounter(int[] lengths, int[] lowerBounds)
            {
                this.lengths = lengths;
                this.lowerBounds = lowerBounds;
                this.indexCounter = new int[lengths.Length];
                Array.Copy(lowerBounds, 0, indexCounter, 0, indexCounter.Length);
            }
            internal bool Next()
            {
                for (int i = 0; i < indexCounter.Length; i++)
                {
                    if (indexCounter[i] < (lengths[i] - 1))
                    {
                        indexCounter[i]++;
                        break;
                    }
                    else
                    {
                        if (i == (indexCounter.Length - 1))
                            return false;
                        indexCounter[i] = lowerBounds[i];
                    }
                }
                return true;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                sb.Append(indexCounter[0].ToString());
                for (int i = 1; i < indexCounter.Length; i++)
                {
                    sb.Append(",");
                    sb.Append(indexCounter[i].ToString());
                }
                sb.Append("]");
                return sb.ToString();
            }
        }
    }
}





