using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;//vv
using System.Globalization;//vv
using System.Data;


//Taken from:
//https://github.com/exceptionnotfound/DataNamesMappingDemo
namespace TeamSupport.Data.Classes
{
	public class DataNamesMapper<TEntity> where TEntity : class, new()
	{
		public TEntity Map(DataRow row)
		{
			TEntity entity = new TEntity();
			return Map(row, entity);
		}

		public TEntity Map(DataRow row, TEntity entity)
		{
			var columnNames = row.Table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
			var properties = (typeof(TEntity)).GetProperties()
											  .Where(x => x.GetCustomAttributes(typeof(DataNamesAttribute), true).Any())
											  .ToList();
			foreach (var prop in properties)
			{
				PropertyMapHelper.Map(typeof(TEntity), row, prop, entity);
			}

			return entity;
		}

		public IEnumerable<TEntity> Map(DataTable table)
		{
			List<TEntity> entities = new List<TEntity>();
			var columnNames = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
			var properties = (typeof(TEntity)).GetProperties()
											  .Where(x => x.GetCustomAttributes(typeof(DataNamesAttribute), true).Any())
											  .ToList();
			foreach (DataRow row in table.Rows)
			{
				TEntity entity = new TEntity();
				foreach (var prop in properties)
				{
					PropertyMapHelper.Map(typeof(TEntity), row, prop, entity);
				}
				entities.Add(entity);
			}

			return entities;
		}
	}

	public static class AttributeHelper
	{
		public static List<string> GetDataNames(Type type, string propertyName)
		{
			var property = type.GetProperty(propertyName).GetCustomAttributes(false).Where(x => x.GetType().Name == "DataNamesAttribute").FirstOrDefault();
			if (property != null)
			{
				return ((DataNamesAttribute)property).ValueNames;
			}
			return new List<string>();
		}
	}

	public static class PropertyMapHelper
	{
		public static void Map(Type type, DataRow row, PropertyInfo prop, object entity)
		{
			List<string> columnNames = AttributeHelper.GetDataNames(type, prop.Name);

			foreach (var columnName in columnNames)
			{
				if (!String.IsNullOrWhiteSpace(columnName) && row.Table.Columns.Contains(columnName))
				{
					var propertyValue = row[columnName];
					if (propertyValue != DBNull.Value)
					{
						ParsePrimitive(prop, entity, row[columnName]);
						break;
					}
				}
			}
		}

		private static void ParsePrimitive(PropertyInfo prop, object entity, object value)
		{
			if (prop.PropertyType == typeof(string))
			{
				prop.SetValue(entity, value.ToString().Trim(), null);
			}
			else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
			{
				if (value == null)
				{
					prop.SetValue(entity, null, null);
				}
				else
				{
					prop.SetValue(entity, ParseBoolean(value.ToString()), null);
				}
			}
			else if (prop.PropertyType == typeof(long))
			{
				prop.SetValue(entity, long.Parse(value.ToString()), null);
			}
			else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
			{
				if (value == null)
				{
					prop.SetValue(entity, null, null);
				}
				else
				{
					prop.SetValue(entity, int.Parse(value.ToString()), null);
				}
			}
			else if (prop.PropertyType == typeof(decimal))
			{
				prop.SetValue(entity, decimal.Parse(value.ToString()), null);
			}
			else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
			{
				double number;
				bool isValid = double.TryParse(value.ToString(), out number);
				if (isValid)
				{
					prop.SetValue(entity, double.Parse(value.ToString()), null);
				}
			}
			else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(Nullable<DateTime>))
			{
				DateTime date;
				bool isValid = DateTime.TryParse(value.ToString(), out date);
				if (isValid)
				{
					prop.SetValue(entity, date, null);
				}
				else
				{
					isValid = DateTime.TryParseExact(value.ToString(), "MMddyyyy", new CultureInfo("en-US"), DateTimeStyles.AssumeLocal, out date);
					if (isValid)
					{
						prop.SetValue(entity, date, null);
					}
				}
			}
			else if (prop.PropertyType == typeof(Guid))
			{
				Guid guid;
				bool isValid = Guid.TryParse(value.ToString(), out guid);
				if (isValid)
				{
					prop.SetValue(entity, guid, null);
				}
				else
				{
					isValid = Guid.TryParseExact(value.ToString(), "B", out guid);
					if (isValid)
					{
						prop.SetValue(entity, guid, null);
					}
				}
			}
		}

		public static bool ParseBoolean(object value)
		{
			if (value == null || value == DBNull.Value) return false;

			switch (value.ToString().ToLowerInvariant())
			{
				case "1":
				case "y":
				case "yes":
				case "true":
					return true;

				case "0":
				case "n":
				case "no":
				case "false":
				default:
					return false;
			}
		}
	}
}
