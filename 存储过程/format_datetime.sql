create proc format_datetime
@timestamp bigint
as
select DATEADD(s, @timestamp+8*3600, '1970-01-01 0:0:0');
