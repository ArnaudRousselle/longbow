using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using LongBow.Dom;

namespace LongBow.Dal.Utilities
{
	public class SerializableObject<T> where T : IDomBase, new()
	{
		protected readonly List<T> Data = new List<T>();
		public bool IsDurty { get; protected set; }

		public void Add(T item)
		{
			var newItem = item.Clone();
			newItem.Id = Data.Any() ? Data.Max(n => n.Id) + 1 : 1;
			item.Id = newItem.Id;
			Data.Add(newItem);

			IsDurty = true;
		}

		public void Remove(int itemId)
		{
			Data.RemoveAll(n => n.Id == itemId);

			IsDurty = true;
		}

		public void Update(T item)
		{
			var itemToUpdate = Data.First(n => n.Id == item.Id);
			var index = Data.IndexOf(itemToUpdate);
			Data.RemoveAt(index);
			Data.Insert(index, item.Clone());

			IsDurty = true;
		}

		public List<T> GetAll()
		{
			return Data.Select(n => n.Clone()).ToList();
		}

		public T Get(int id)
		{
			return Data.FirstOrDefault(n => n.Id == id).Clone();
		}

		public void Save(XmlTextWriter writer)
		{
			var serializer = new XmlSerializer(typeof(List<T>));
			serializer.Serialize(writer, Data);

			IsDurty = false;
		}

		public void Load(XmlDocument xmlDocument)
		{
			Data.Clear();

			if (xmlDocument.DocumentElement == null)
				return;

			var node = xmlDocument.DocumentElement.SelectSingleNode("ArrayOf" + typeof(T).Name);

			if (node == null)
			{
				IsDurty = false;
				return;
			}

			using (var memoryStream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(memoryStream))
				{
					streamWriter.Write(node.OuterXml);
					streamWriter.Flush();

					memoryStream.Position = 0;

					var serializer = new XmlSerializer(typeof(List<T>));
					Data.AddRange((List<T>)serializer.Deserialize(memoryStream));
				}
			}

			IsDurty = false;
		}
	}
}
