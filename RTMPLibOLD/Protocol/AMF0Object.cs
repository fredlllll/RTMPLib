using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMPLib
{
	public class AMF0Object
	{
		private List<AMF0ObjectProperty> properties = new List<AMF0ObjectProperty>();

		public AMF0Object(RTMPMessageBody body)
		{
			byte first = body.BinaryReader.ReadByte();
			if (first != 3)
			{
				throw new Exception("this is no AMF0Object");
			}
			AMF0ObjectProperty prop;
			while ((prop = FromBody(body)) != null)
			{
				properties.Add(prop);
			}
		}

		private static AMF0ObjectProperty FromBody(RTMPMessageBody body)
		{
			BinaryReader br = body.BinaryReader;
			ushort namelen = br.ReadUShort();
			byte next = br.ReadByte();
			if (namelen == 0 && next == 9)
			{
				return null;
			}
			br.BaseStream.Position--;
			string name = body.ReadString(namelen);//Encoding.UTF8.GetString(bytes, index, namelen);
			byte type = br.ReadByte();
			switch (type)
			{
				case 0://number
					double var = br.ReadDouble();//BitConverter.ToDouble(bytes, index);
					return new AMF0ObjectProperty(name, var);
				case 2://string
					ushort strlen = br.ReadUShort();//BitConverter.ToUInt16(bytes, index);
					string value = body.ReadString(strlen);//Encoding.UTF8.GetString(bytes, index, strlen);
					return new AMF0ObjectProperty(name, value);
				case 8://ECMA array
					uint arrayLength = br.ReadUInt();//BitConverter.ToUInt32(bytes, index);
					AMF0ECMAArray array = new AMF0ECMAArray();
					AMF0ObjectProperty arrayprop;
					while ((arrayprop = FromBody(body)) != null)
					{
						array.props.Add(arrayprop);
					}
					return new AMF0ObjectProperty(name, array);
				default:
					throw new Exception("not yet implemented type");
			}
		}

		public AMF0ObjectProperty this[int i]
		{
			get { return properties[i]; }
		}

		public void AddProperty(AMF0ObjectProperty prop)
		{
			properties.Add(prop);
		}

		public override string ToString()
		{
			String tmp = "";
			foreach (AMF0ObjectProperty prop in properties)
			{
				tmp += prop.ToString() + Environment.NewLine;
			}
			return tmp;
		}
	}

	public class AMF0ObjectProperty
	{
		public object prop;
		public string name;
		public AMF0ObjectProperty(String name, object prop)
		{
			this.name = name;
			this.prop = prop;
		}

		public override string ToString()
		{
			return name + ": " + prop.ToString();
		}
	}

	public class AMF0ECMAArray
	{
		public List<AMF0ObjectProperty> props = new List<AMF0ObjectProperty>();
		public override string ToString()
		{
			return "ECMA Array with " + props.Count + " props";
		}
	}
}
