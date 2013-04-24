using System.Collections.Generic;
using System.Xml;

/* 
 * AutoResxTranslator
 * by Salar Khalilzadeh
 * 
 * https://AutoResxTranslator.codeplex.com/
 * Mozilla Public License v2
 */
namespace AutoResxTranslator
{
	public static class ResxTranslator
	{
		public static List<XmlNode> ReadResxData(XmlDocument doc)
		{
			var root = doc.SelectSingleNode("root");
			var dataList = new List<XmlNode>();
			foreach (XmlNode node in root.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element)
					continue;
				if (node.Name != "data")
					continue;
				dataList.Add(node);
			}
			// node.Attributes["name"].Value
			return dataList;
		}

		public static XmlNode GetDataValueNode(XmlNode dataNode)
		{
			for (int i = 0; i < dataNode.ChildNodes.Count; i++)
			{
				var node = dataNode.ChildNodes[i];
				if (node.NodeType != XmlNodeType.Element)
					continue;
				if (node.Name == "value")
					return node;
			}
			return null;
		}

		public static string GetDataKeyName(XmlNode dataNode)
		{
			if (dataNode == null)
				return string.Empty;
			return dataNode.Attributes["name"].Value;
		}
		public static string GetDataValueNodeContent(XmlNode dataNode)
		{
			for (int i = 0; i < dataNode.ChildNodes.Count; i++)
			{
				var node = dataNode.ChildNodes[i];
				if (node.NodeType != XmlNodeType.Element)
					continue;
				if (node.Name == "value")
					return node.InnerText;
			}
			return null;
		}

	}
}
