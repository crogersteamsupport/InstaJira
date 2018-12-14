--INSERT INTO ReportTableFields (ReportTableID, FieldName, Alias, DataType, size, IsVisible)

select rt.ReportTableID, sc.name as FieldName, sc.name as Alias, st.name as DataType, sc.length as Size, '1'
from sysobjects so 
left join syscolumns sc on sc.id = object_id(so.name)
left join systypes st on sc.xtype = st.xtype
left join ReportTables rt on rt.TableName = so.name

where so.name IN (SELECT TableName FROM ReportTables)
and not exists (select * from ReportTableFields rtf WHERE rtf.FieldName = sc.name and rtf.ReportTableID = rt.ReportTableID)
and st.name <> 'sysname'
and rt.ReportTableID=27
order by rt.reporttableid, sc.name