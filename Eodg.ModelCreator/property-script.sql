SELECT 
'public ' + a1.NewType + ' ' + a1.ColumnName + ';' as PropertyDefinition
,*
 FROM 
(
/*using top because i'm putting an order by ordinal_position on it. 
putting a top on it is the only way for a subquery to be ordered*/
SELECT TOP 100 PERCENT
COLUMN_NAME as ColumnName,
DATA_TYPE as DataType,
IS_NULLABLE as IsNullable,
CASE 
    WHEN DATA_TYPE = 'uniqueidentifier' THEN 'Guid'
    WHEN DATA_TYPE = 'varchar' THEN 'string'
    WHEN DATA_TYPE = 'nvarchar' THEN 'string'
    WHEN DATA_TYPE = 'datetime' AND IS_NULLABLE = 'NO' THEN 'DateTime'
    WHEN DATA_TYPE = 'datetime' AND IS_NULLABLE = 'YES' THEN 'DateTime?'
    WHEN DATA_TYPE = 'date' AND IS_NULLABLE = 'NO' THEN 'DateTime'
    WHEN DATA_TYPE = 'date' AND IS_NULLABLE = 'YES' THEN 'DateTime?'
    WHEN DATA_TYPE = 'int' AND IS_NULLABLE = 'YES' THEN 'int?'
    WHEN DATA_TYPE = 'int' AND IS_NULLABLE = 'NO' THEN 'int'
    WHEN DATA_TYPE = 'smallint' AND IS_NULLABLE = 'NO' THEN 'Int16'
    WHEN DATA_TYPE = 'smallint' AND IS_NULLABLE = 'YES' THEN 'Int16?'
    WHEN DATA_TYPE = 'decimal' AND IS_NULLABLE = 'NO' THEN 'decimal'
    WHEN DATA_TYPE = 'decimal' AND IS_NULLABLE = 'YES' THEN 'decimal?'
    WHEN DATA_TYPE = 'numeric' AND IS_NULLABLE = 'NO' THEN 'decimal'
    WHEN DATA_TYPE = 'numeric' AND IS_NULLABLE = 'YES' THEN 'decimal?'
    WHEN DATA_TYPE = 'money' AND IS_NULLABLE = 'NO' THEN 'decimal'
    WHEN DATA_TYPE = 'money' AND IS_NULLABLE = 'YES' THEN 'decimal?'
    WHEN DATA_TYPE = 'bigint' AND IS_NULLABLE = 'NO' THEN 'Int64'
    WHEN DATA_TYPE = 'bigint' AND IS_NULLABLE = 'YES' THEN 'Int64?'
    WHEN DATA_TYPE = 'tinyint' AND IS_NULLABLE = 'NO' THEN 'byte'
    WHEN DATA_TYPE = 'tinyint' AND IS_NULLABLE = 'YES' THEN 'byte?'
    WHEN DATA_TYPE = 'char' THEN 'string'
    WHEN DATA_TYPE = 'timestamp' THEN 'byte[]'
    WHEN DATA_TYPE = 'varbinary' THEN 'byte[]'
    WHEN DATA_TYPE = 'bit' AND IS_NULLABLE = 'NO' THEN 'bool'
    WHEN DATA_TYPE = 'bit' AND IS_NULLABLE = 'YES' THEN 'bool?'
    WHEN DATA_TYPE = 'xml' THEN 'string'
END AS NewType
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = '@TableName'
ORDER BY ORDINAL_POSITION
) AS a1