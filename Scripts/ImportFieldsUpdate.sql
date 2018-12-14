--INSERT INTO ImportFields (TableName, FieldName, Alias, DataType, size, IsVisible, Description)

select 'Organizations' as TableName, sc.name as FieldName, sc.name as Alias, st.name as DataType, sc.length as Size, 1, ''
from sysobjects so 
left join syscolumns sc on sc.id = object_id(so.name)
left join systypes st on sc.xtype = st.xtype

where so.name = 'Organizations'
and st.name <> 'sysname'
order by sc.name