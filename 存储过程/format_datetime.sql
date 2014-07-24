create proc format_datetime
@timestamp bigint
as
select DATEADD(s, @timestamp, '1970-01-01 0:0:0');