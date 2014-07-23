create proc get_timestamp 
@time datetime
as 
SELECT DATEDIFF(S,'1970-01-01 00:00:00', @time) - 8 * 3600 as timestamp;